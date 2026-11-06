namespace UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    // 接続の表示情報だけを保持し、スキルの解放状態とは分離する。
    [Serializable]
    public class TreeConnection
    {
        [SerializeField] Button source;
        [SerializeField] Button destination;
        [SerializeField] bool visible = true;
        [SerializeField] Color color = Color.white;

        public Button Source => source;
        public Button Destination => destination;
        public bool Visible => visible;
        public Color Color => color;

        public TreeConnection(Button source, Button destination)
        {
            this.source = source != null ? source : throw new ArgumentNullException(nameof(source));
            this.destination = destination != null ? destination : throw new ArgumentNullException(nameof(destination));
        }

        internal void SetVisible(bool value) => visible = value;
        internal void SetColor(Color value) => color = value;
    }

    // 複数の接続を1つのUGUIメッシュにまとめ、マスクにも対応する。
    [AddComponentMenu("UI/Tree Connection Graphic")]
    [RequireComponent(typeof(CanvasRenderer))]
    public class TreeConnectionGraphic : MaskableGraphic
    {
        // 1接続につき4頂点を使い、UGUIメッシュの頂点上限を超えないようにする。
        const int MaxConnections = 16000;
        // 別平面への投影を避けつつ、座標変換の浮動小数点誤差を許容する。
        const float PlaneTolerance = 0.01f;

        [SerializeField, Min(0f)] float lineWidth = 4f;
        [SerializeField] List<TreeConnection> connections = new List<TreeConnection>();

        // 描画結果に影響する値だけを比較し、変化がなければメッシュを再生成しない。
        readonly List<ConnectionSnapshot> snapshots = new List<ConnectionSnapshot>();
        // 同じ不正設定に対する警告を描画のたびに繰り返さない。
        readonly List<string> warnings = new List<string>();

        struct ConnectionSnapshot : IEquatable<ConnectionSnapshot>
        {
            public bool draw;
            public Vector2 start;
            public Vector2 end;
            public Color color;

            public bool Equals(ConnectionSnapshot other)
            {
                return draw == other.draw && start.Equals(other.start) &&
                    end.Equals(other.end) && color.Equals(other.color);
            }
        }

        public int ConnectionCount => connections.Count;

        // 接続定義の変更を進行管理側へ通知し、同じ接続情報を二重管理しない。
        public event Action ConnectionsChanged;

        public float LineWidth
        {
            get => lineWidth;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value) || value < 0f)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (lineWidth == value)
                    return;
                lineWidth = value;
                SetVerticesDirty();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            // 線がButtonへの入力を遮らないようにする。
            raycastTarget = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            // base.OnEnableでUGUIの再構築処理を先に登録し、その後に位置を確認する。
            Canvas.willRenderCanvases += RefreshAfterLayout;
        }

        protected override void OnDisable()
        {
            Canvas.willRenderCanvases -= RefreshAfterLayout;
            snapshots.Clear();
            warnings.Clear();
            base.OnDisable();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            lineWidth = Mathf.Max(0f, lineWidth);
            raycastTarget = false;
            SetVerticesDirty();
        }
#endif

        public TreeConnection GetConnection(int index) => connections[index];

        // 実行時の接続変更もInspector設定と同じリストに反映する。
        public TreeConnection AddConnection(Button source, Button destination)
        {
            if (connections.Count >= MaxConnections)
                throw new InvalidOperationException($"A graphic supports at most {MaxConnections} connections.");

            var connection = new TreeConnection(source, destination);
            connections.Add(connection);
            SetVerticesDirty();
            ConnectionsChanged?.Invoke();
            return connection;
        }

        public bool RemoveConnection(TreeConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (!connections.Remove(connection))
                return false;
            SetVerticesDirty();
            ConnectionsChanged?.Invoke();
            return true;
        }

        public void SetConnectionVisible(TreeConnection connection, bool visible)
        {
            RequireConnection(connection);
            if (connection.Visible == visible)
                return;
            connection.SetVisible(visible);
            SetVerticesDirty();
        }

        public void SetConnectionColor(TreeConnection connection, Color color)
        {
            RequireConnection(connection);
            if (connection.Color.Equals(color))
                return;
            connection.SetColor(color);
            SetVerticesDirty();
        }

        void RequireConnection(TreeConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));
            if (!connections.Contains(connection))
                throw new ArgumentException("The connection does not belong to this graphic.", nameof(connection));
        }

        void RefreshAfterLayout()
        {
            if (!isActiveAndEnabled || canvas == null || !canvas.isActiveAndEnabled)
                return;

            if (CaptureSnapshots())
            {
                // 通常のGraphic再構築後にレイアウト変更を検出した場合、今回の描画に反映する。
                UpdateGeometry();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vertexHelper)
        {
            vertexHelper.Clear();
            CaptureSnapshots();
            if (lineWidth <= 0f)
                return;

            foreach (ConnectionSnapshot snapshot in snapshots)
            {
                if (!snapshot.draw)
                    continue;

                Vector2 direction = snapshot.end - snapshot.start;
                if (direction.sqrMagnitude < 0.000001f)
                    continue;

                // 線分に垂直な幅を持つ四角形を2つの三角形で描画する。
                Vector2 offset = new Vector2(-direction.y, direction.x).normalized * (lineWidth * 0.5f);
                int firstVertex = vertexHelper.currentVertCount;
                Color32 vertexColor = color * snapshot.color;
                vertexHelper.AddVert(snapshot.start - offset, vertexColor, Vector2.zero);
                vertexHelper.AddVert(snapshot.start + offset, vertexColor, Vector2.up);
                vertexHelper.AddVert(snapshot.end + offset, vertexColor, Vector2.one);
                vertexHelper.AddVert(snapshot.end - offset, vertexColor, Vector2.right);
                vertexHelper.AddTriangle(firstVertex, firstVertex + 1, firstVertex + 2);
                vertexHelper.AddTriangle(firstVertex + 2, firstVertex + 3, firstVertex);
            }
        }

        bool CaptureSnapshots()
        {
            bool changed = snapshots.Count != Mathf.Min(connections.Count, MaxConnections);
            int count = Mathf.Min(connections.Count, MaxConnections);
            if (connections.Count > MaxConnections)
                SetWarning(MaxConnections, $"Only the first {MaxConnections} connections can be drawn by one graphic.");
            else if (warnings.Count > MaxConnections)
                SetWarning(MaxConnections, null);

            while (snapshots.Count > count)
                snapshots.RemoveAt(snapshots.Count - 1);
            while (snapshots.Count < count)
                snapshots.Add(default);

            for (int i = 0; i < count; i++)
            {
                ConnectionSnapshot current = CaptureConnection(connections[i], out string warning);
                SetWarning(i, warning);
                if (!snapshots[i].Equals(current))
                {
                    snapshots[i] = current;
                    changed = true;
                }
            }
            return changed;
        }

        ConnectionSnapshot CaptureConnection(TreeConnection connection, out string warning)
        {
            warning = null;
            var snapshot = new ConnectionSnapshot();
            if (connection == null || connection.Source == null || connection.Destination == null)
            {
                warning = "A connection has a missing source or destination Button.";
                return snapshot;
            }
            if (!connection.Visible || !connection.Source.isActiveAndEnabled || !connection.Destination.isActiveAndEnabled)
                return snapshot;

            Canvas nearestCanvas = GetComponentInParent<Canvas>();
            if (nearestCanvas != null && !nearestCanvas.isActiveAndEnabled)
                return snapshot;

            if (canvas == null || connection.Source.GetComponentInParent<Canvas>() != canvas ||
                connection.Destination.GetComponentInParent<Canvas>() != canvas)
            {
                warning = "Both Buttons and the connection graphic must belong to the same Canvas.";
                return snapshot;
            }

            RectTransform sourceRect = connection.Source.transform as RectTransform;
            RectTransform destinationRect = connection.Destination.transform as RectTransform;
            if (sourceRect == null || destinationRect == null)
            {
                warning = "Both Buttons must have a RectTransform.";
                return snapshot;
            }

            // CanvasのRender Modeに依存せず、Buttonの中心を線のローカル座標に変換する。
            Vector3 start = rectTransform.InverseTransformPoint(sourceRect.TransformPoint(sourceRect.rect.center));
            Vector3 end = rectTransform.InverseTransformPoint(destinationRect.TransformPoint(destinationRect.rect.center));
            if (Mathf.Abs(start.z) > PlaneTolerance || Mathf.Abs(end.z) > PlaneTolerance)
            {
                warning = "Button centers must lie on the connection graphic's plane.";
                return snapshot;
            }

            snapshot.draw = true;
            snapshot.start = start;
            snapshot.end = end;
            snapshot.color = connection.Color;
            return snapshot;
        }

        void SetWarning(int index, string warning)
        {
            while (warnings.Count <= index)
                warnings.Add(null);
            if (warnings[index] == warning)
                return;
            warnings[index] = warning;
            if (warning != null)
                Debug.LogWarning($"TreeConnectionGraphic: {warning} (index {index})", this);
        }
    }
}
