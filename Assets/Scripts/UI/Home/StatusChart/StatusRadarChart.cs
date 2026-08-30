using UnityEngine;
using UnityEngine.UI;

// AI参考

[System.Serializable]
public class RadarStatus
{
    public StatusType type;
    public float value;
    public float minValue;
    public float maxValue;
}

[RequireComponent(typeof(CanvasRenderer))]
public class StatusRadarChart : Graphic
{
    [Header("Chart Settings")]
    [SerializeField, Min(3)]
    private int axisCount = 6;

    [SerializeField, Min(0f)]
    private float radius = 100f;

    [Header("Grid")]
    [SerializeField, Range(1, 10)]
    private int gridCount = 4;

    [SerializeField, Min(0f)]
    private float gridLineWidth = 1f;

    [SerializeField]
    private Color gridColor = new Color(1f, 1f, 1f, 0.25f);

    [Header("Axis")]
    [SerializeField, Min(0f)]
    private float axisLineWidth = 1f;

    [SerializeField]
    private Color axisColor = new Color(1f, 1f, 1f, 0.35f);

    [Header("Status")]
    [SerializeField]
    private Color statusColor = new Color(0.2f, 0.7f, 1f, 0.45f);

    [SerializeField]
    private Color statusLineColor = new Color(0.2f, 0.7f, 1f, 1f);

    [SerializeField, Min(0f)]
    private float statusLineWidth = 2f;

    [Header("Animation Settings")]
    [SerializeField, Min(0.1f)]
    private float lerpSpeed = 10f;

    private float[] values;
    private float[] displayValues;

    protected override void OnEnable()
    {
        base.OnEnable();

        EnsureValueArray();
        SetVerticesDirty();
    }

    protected override void OnValidate()
    {
        base.OnValidate();

        axisCount = Mathf.Max(3, axisCount);
        radius = Mathf.Max(0f, radius);

        EnsureValueArray();
        SetVerticesDirty();
    }

    /// <summary>
    /// ステータス値を設定します。
    /// </summary>
    public void SetValues(params RadarStatus[] newRadarStatuses)
    {
        if (newRadarStatuses == null || newRadarStatuses.Length == 0)
            return;

        axisCount = Mathf.Max(3, newRadarStatuses.Length);

        EnsureValueArray();

        for (int i = 0; i < axisCount; i++)
        {
            float val = newRadarStatuses[i].value;
            float min = newRadarStatuses[i].minValue;
            float max = newRadarStatuses[i].maxValue;

            float clampedValue = Mathf.Clamp(val, min, max);
            float range = max - min;

            values[i] = range > 0f ? (clampedValue - min) / range : 0f;
        }

        SetVerticesDirty();
    }

    /// <summary>
    /// 現在の値を取得します。
    /// </summary>
    public float GetValue(int index)
    {
        if (index < 0 || index >= values.Length)
            return 0f;

        return values[index];
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        EnsureValueArray();

        DrawGrid(vh);
        DrawAxis(vh);
        DrawStatus(vh);
    }

    private void EnsureValueArray()
    {
        if (axisCount < 3)
            axisCount = 3;

        if (values == null || values.Length != axisCount)
        {
            float[] oldValues = values;
            values = new float[axisCount];

            if (oldValues != null)
            {
                int copyCount = Mathf.Min(oldValues.Length, values.Length);
                for (int i = 0; i < copyCount; i++)
                {
                    values[i] = oldValues[i];
                }
            }
        }

        if (displayValues == null || displayValues.Length != axisCount)
        {
            float[] oldDisplayValues = displayValues;
            displayValues = new float[axisCount];

            if (oldDisplayValues != null)
            {
                int copyCount = Mathf.Min(oldDisplayValues.Length, displayValues.Length);
                for (int i = 0; i < copyCount; i++)
                {
                    displayValues[i] = oldDisplayValues[i];
                }
            }
            else if (values != null)
            {
                values.CopyTo(displayValues, 0);
            }
        }
    }

    protected virtual void Update()
    {
        if (!Application.isPlaying)
        {
            if (values != null && displayValues != null && values.Length == displayValues.Length)
            {
                values.CopyTo(displayValues, 0);
            }
            return;
        }

        if (values == null || displayValues == null || values.Length != axisCount || displayValues.Length != axisCount)
            return;

        bool dirty = false;
        for (int i = 0; i < axisCount; i++)
        {
            if (Mathf.Abs(displayValues[i] - values[i]) > 0.0001f)
            {
                displayValues[i] = Mathf.Lerp(displayValues[i], values[i], Time.deltaTime * lerpSpeed);

                if (Mathf.Abs(displayValues[i] - values[i]) < 0.001f)
                {
                    displayValues[i] = values[i];
                }
                dirty = true;
            }
        }

        if (dirty)
        {
            SetVerticesDirty();
        }
    }

    //==================================================
    // Grid
    //==================================================

    private void DrawGrid(VertexHelper vh)
    {
        for (int i = 1; i <= gridCount; i++)
        {
            float normalized = (float)i / gridCount;
            float currentRadius = radius * normalized;

            for (int j = 0; j < axisCount; j++)
            {
                int next = (j + 1) % axisCount;

                Vector2 start =
                    GetPoint(j, axisCount, currentRadius);

                Vector2 end =
                    GetPoint(next, axisCount, currentRadius);

                AddLine(
                    vh,
                    start,
                    end,
                    gridLineWidth,
                    gridColor
                );
            }
        }
    }

    //==================================================
    // Axis
    //==================================================

    private void DrawAxis(VertexHelper vh)
    {
        for (int i = 0; i < axisCount; i++)
        {
            Vector2 start = Vector2.zero;

            Vector2 end =
                GetPoint(i, axisCount, radius);

            AddLine(
                vh,
                start,
                end,
                axisLineWidth,
                axisColor
            );
        }
    }

    //==================================================
    // Status
    //==================================================

    private void DrawStatus(VertexHelper vh)
    {
        if (displayValues == null || displayValues.Length < axisCount)
            return;

        Vector2[] points = new Vector2[axisCount];

        for (int i = 0; i < axisCount; i++)
        {
            float normalized = displayValues[i];

            points[i] = GetPoint(
                i,
                axisCount,
                radius * normalized
            );
        }

        // 内部を塗る
        AddFilledPolygon(
            vh,
            points,
            statusColor
        );

        // 外周線
        for (int i = 0; i < axisCount; i++)
        {
            int next = (i + 1) % axisCount;

            AddLine(
                vh,
                points[i],
                points[next],
                statusLineWidth,
                statusLineColor
            );
        }
    }

    //==================================================
    // Geometry
    //==================================================

    private Vector2 GetPoint(
        int index,
        int count,
        float currentRadius)
    {
        /*
         * 0番目を真上にする。
         *
         *       0
         *       ↑
         *
         *  5           1
         *
         *  4           2
         *
         *       3
         */

        float angle =
            90f - (360f / count) * index;

        float radians =
            angle * Mathf.Deg2Rad;

        return new Vector2(
            Mathf.Cos(radians),
            Mathf.Sin(radians)
        ) * currentRadius;
    }

    //==================================================
    // Mesh Utilities
    //==================================================

    private void AddFilledPolygon(
        VertexHelper vh,
        Vector2[] points,
        Color color)
    {
        if (points == null || points.Length < 3)
            return;

        int startIndex = vh.currentVertCount;

        // 中心
        AddVertex(
            vh,
            Vector2.zero,
            color
        );

        // 外周
        for (int i = 0; i < points.Length; i++)
        {
            AddVertex(
                vh,
                points[i],
                color
            );
        }

        // Triangle Fan
        for (int i = 0; i < points.Length; i++)
        {
            int current = startIndex + 1 + i;

            int next =
                startIndex + 1 +
                ((i + 1) % points.Length);

            vh.AddTriangle(
                startIndex,
                current,
                next
            );
        }
    }

    private void AddLine(
        VertexHelper vh,
        Vector2 start,
        Vector2 end,
        float width,
        Color color)
    {
        if (width <= 0f)
            return;

        Vector2 direction =
            (end - start).normalized;

        Vector2 normal =
            new Vector2(
                -direction.y,
                direction.x
            );

        float halfWidth =
            width * 0.5f;

        Vector2 v0 =
            start + normal * halfWidth;

        Vector2 v1 =
            start - normal * halfWidth;

        Vector2 v2 =
            end - normal * halfWidth;

        Vector2 v3 =
            end + normal * halfWidth;

        int index = vh.currentVertCount;

        AddVertex(vh, v0, color);
        AddVertex(vh, v1, color);
        AddVertex(vh, v2, color);
        AddVertex(vh, v3, color);

        vh.AddTriangle(
            index,
            index + 1,
            index + 2
        );

        vh.AddTriangle(
            index,
            index + 2,
            index + 3
        );
    }

    private void AddVertex(
        VertexHelper vh,
        Vector2 position,
        Color color)
    {
        UIVertex vertex = UIVertex.simpleVert;

        vertex.position = position;
        vertex.color = color;
        vertex.uv0 = Vector2.zero;

        vh.AddVert(vertex);
    }
}