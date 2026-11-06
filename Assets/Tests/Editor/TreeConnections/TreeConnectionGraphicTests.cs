namespace UI.Tests
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.UI;

    public class InspectableTreeConnectionGraphic : TreeConnectionGraphic
    {
        public readonly List<UIVertex> Vertices = new List<UIVertex>();
        public int PopulateCount { get; private set; }

        protected override void OnPopulateMesh(VertexHelper helper)
        {
            base.OnPopulateMesh(helper);
            PopulateCount++;
            Vertices.Clear();
            for (int i = 0; i < helper.currentVertCount; i++)
            {
                var vertex = new UIVertex();
                helper.PopulateUIVertex(ref vertex, i);
                Vertices.Add(vertex);
            }
        }
    }

    public class TreeConnectionGraphicTests
    {
        GameObject root;
        Canvas canvas;
        InspectableTreeConnectionGraphic graphic;
        Button source;
        Button destination;

        [SetUp]
        public void SetUp()
        {
            root = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas));
            canvas = root.GetComponent<Canvas>();
            var line = new GameObject("Lines", typeof(RectTransform), typeof(CanvasRenderer));
            line.transform.SetParent(root.transform, false);
            graphic = line.AddComponent<InspectableTreeConnectionGraphic>();
            source = CreateButton("Source", new Vector2(-60f, 20f));
            destination = CreateButton("Destination", new Vector2(90f, -30f));
        }

        [TearDown]
        public void TearDown()
        {
            UnityEngine.Object.DestroyImmediate(root);
        }

        Button CreateButton(string name, Vector2 position)
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            obj.transform.SetParent(root.transform, false);
            var rect = (RectTransform)obj.transform;
            rect.sizeDelta = new Vector2(80f, 40f);
            rect.anchoredPosition = position;
            return obj.GetComponent<Button>();
        }

        Vector2 CenterOfPair(int first)
        {
            return (graphic.Vertices[first].position + graphic.Vertices[first + 1].position) * 0.5f;
        }

        void AssertPoint(Vector2 actual, Vector2 expected)
        {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(0.001f));
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(0.001f));
        }

        [TestCase(RenderMode.ScreenSpaceOverlay)]
        [TestCase(RenderMode.ScreenSpaceCamera)]
        [TestCase(RenderMode.WorldSpace)]
        public void DrawsCentersAndUpdatesAfterMovement(RenderMode mode)
        {
            var cameraObject = new GameObject("Camera", typeof(Camera));
            cameraObject.transform.SetParent(root.transform, false);
            canvas.worldCamera = cameraObject.GetComponent<Camera>();
            canvas.renderMode = mode;
            graphic.AddConnection(source, destination);
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.EqualTo(4));
            AssertPoint(CenterOfPair(0), new Vector2(-60f, 20f));
            AssertPoint(CenterOfPair(2), new Vector2(90f, -30f));

            int count = graphic.PopulateCount;
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.PopulateCount, Is.EqualTo(count));
            ((RectTransform)destination.transform).anchoredPosition = new Vector2(20f, 70f);
            Canvas.ForceUpdateCanvases();
            AssertPoint(CenterOfPair(2), new Vector2(20f, 70f));
            Assert.That(graphic.PopulateCount, Is.GreaterThan(count));
        }

        [Test]
        public void SupportsBranchingVisibilityColorAndRemoval()
        {
            Button third = CreateButton("Third", new Vector2(0f, 100f));
            TreeConnection first = graphic.AddConnection(source, destination);
            TreeConnection second = graphic.AddConnection(source, third);
            graphic.SetConnectionColor(second, Color.red);
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.EqualTo(8));
            Assert.That(graphic.Vertices[4].color, Is.EqualTo((Color32)Color.red));

            graphic.SetConnectionVisible(first, false);
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.EqualTo(4));
            Assert.That(graphic.RemoveConnection(second), Is.True);
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.Zero);
            Assert.Throws<ArgumentException>(() => graphic.SetConnectionColor(second, Color.white));
        }

        [Test]
        public void TracksInactiveAndDestroyedButtons()
        {
            graphic.AddConnection(source, destination);
            Canvas.ForceUpdateCanvases();
            destination.gameObject.SetActive(false);
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.Zero);
            destination.gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.EqualTo(4));
            UnityEngine.Object.DestroyImmediate(destination.gameObject);
            LogAssert.Expect(LogType.Warning, "TreeConnectionGraphic: A connection has a missing source or destination Button. (index 0)");
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.Zero);
        }

        [Test]
        public void UsesLayoutResultOnSameCanvasUpdate()
        {
            var groupObject = new GameObject("Layout", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            groupObject.transform.SetParent(root.transform, false);
            var groupRect = (RectTransform)groupObject.transform;
            groupRect.sizeDelta = new Vector2(400f, 100f);
            var group = groupObject.GetComponent<HorizontalLayoutGroup>();
            group.childForceExpandWidth = false;
            group.childForceExpandHeight = false;
            source.transform.SetParent(groupObject.transform, false);
            destination.transform.SetParent(groupObject.transform, false);
            graphic.AddConnection(source, destination);
            Canvas.ForceUpdateCanvases();

            group.spacing = 50f;
            LayoutRebuilder.MarkLayoutForRebuild(groupRect);
            Canvas.ForceUpdateCanvases();
            var destinationRect = (RectTransform)destination.transform;
            Vector2 expected = graphic.rectTransform.InverseTransformPoint(
                destinationRect.TransformPoint(destinationRect.rect.center));
            AssertPoint(CenterOfPair(2), expected);
        }

        [Test]
        public void UsesLocalCentersWithTransformedParentsAndChangesWidth()
        {
            root.transform.SetPositionAndRotation(new Vector3(12f, -9f, 4f), Quaternion.Euler(20f, 30f, 40f));
            root.transform.localScale = new Vector3(2f, 2f, 2f);
            var sourceRect = (RectTransform)source.transform;
            sourceRect.pivot = Vector2.zero;
            graphic.AddConnection(source, destination);
            graphic.LineWidth = 12f;
            Canvas.ForceUpdateCanvases();

            Vector2 expected = graphic.rectTransform.InverseTransformPoint(
                sourceRect.TransformPoint(sourceRect.rect.center));
            AssertPoint(CenterOfPair(0), expected);
            Assert.That(Vector3.Distance(graphic.Vertices[0].position, graphic.Vertices[1].position),
                Is.EqualTo(12f).Within(0.001f));
            Assert.Throws<ArgumentOutOfRangeException>(() => graphic.LineWidth = -1f);
        }

        [Test]
        public void WarnsAndSkipsDifferentCanvasAndPlane()
        {
            graphic.AddConnection(source, destination);
            Canvas.ForceUpdateCanvases();
            var nestedCanvas = new GameObject("Nested Canvas", typeof(RectTransform), typeof(Canvas));
            nestedCanvas.transform.SetParent(root.transform, false);
            destination.transform.SetParent(nestedCanvas.transform, false);
            LogAssert.Expect(LogType.Warning, "TreeConnectionGraphic: Both Buttons and the connection graphic must belong to the same Canvas. (index 0)");
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.Zero);

            destination.transform.SetParent(root.transform, false);
            destination.transform.localPosition = new Vector3(90f, -30f, 2f);
            LogAssert.Expect(LogType.Warning, "TreeConnectionGraphic: Button centers must lie on the connection graphic's plane. (index 0)");
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.Zero);
            destination.transform.localPosition = new Vector3(90f, -30f, 0f);
            Canvas.ForceUpdateCanvases();
            Assert.That(graphic.Vertices.Count, Is.EqualTo(4));
        }
    }
}
