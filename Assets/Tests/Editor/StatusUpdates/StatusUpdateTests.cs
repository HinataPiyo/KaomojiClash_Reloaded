using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class StatusUpdateTests
{
    const BindingFlags Instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    GameObject root;
    readonly System.Collections.Generic.List<Object> assets = new System.Collections.Generic.List<Object>();

    static Type GameType(string name) => Type.GetType(name + ", Assembly-CSharp", true);
    static object Get(object target, string name) => target.GetType().GetProperty(name, Instance).GetValue(target);
    static void Set(object target, string name, object value) => target.GetType().GetProperty(name, Instance).SetValue(target, value);
    static void SetField(object target, string name, object value) => target.GetType().GetField(name, Instance).SetValue(target, value);
    static void Invoke(object target, string name) => target.GetType().GetMethod(name, Instance).Invoke(target, null);
    static void Invoke(object target, string name, object value) => target.GetType().GetMethod(name, Instance).Invoke(target, new[] { value });

    [SetUp]
    public void SetUp()
    {
        root = new GameObject("Status update tests");
        root.SetActive(false);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(root);
        foreach (Object asset in assets)
            Object.DestroyImmediate(asset);
        assets.Clear();
    }

    Object CreateAsset(string name)
    {
        Object asset = ScriptableObject.CreateInstance(GameType(name));
        assets.Add(asset);
        return asset;
    }

    object CreateSymbol(string type, string symbolType, string statsField, object stats)
    {
        Object symbol = CreateAsset(type);
        GameType("SymbolData").GetField("symbolType", Instance)
            .SetValue(symbol, Enum.Parse(GameType("SymbolType"), symbolType));
        SetField(symbol, statsField, stats);
        return symbol;
    }

    Component CreateText(string name)
    {
        var textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer));
        textObject.transform.SetParent(root.transform, false);
        return textObject.AddComponent(Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro", true));
    }

    static void Equip(object data, object symbol)
    {
        data.GetType().GetMethod("SetSymbolDataByType").Invoke(data, new[] { symbol });
    }

    [TestCase("CriticalStats", "CriticalRate")]
    [TestCase("CriticalStats", "CriticalDamage")]
    [TestCase("CoreStats", "Stamina")]
    [TestCase("CoreStats", "Power")]
    [TestCase("CoreStats", "Speed")]
    [TestCase("CoreStats", "Guard")]
    public void StatsNotifyOnlyWhenValueChanges(string typeName, string property)
    {
        object stats = Activator.CreateInstance(GameType(typeName));
        int notifications = 0;
        Action changed = () => notifications++;
        stats.GetType().GetEvent("Changed").AddEventHandler(stats, changed);

        Set(stats, property, 0.1f);
        Set(stats, property, 0.1f);
        Set(stats, property, 0.2f);

        Assert.That(notifications, Is.EqualTo(2));
        Assert.That(Get(stats, property), Is.EqualTo(0.2f));
    }

    [TestCase("CriticalStats", "CriticalRate")]
    [TestCase("CriticalStats", "CriticalDamage")]
    [TestCase("CoreStats", "Stamina")]
    [TestCase("CoreStats", "Power")]
    [TestCase("CoreStats", "Speed")]
    [TestCase("CoreStats", "Guard")]
    public void StatsRetainValuesSerializedWithPreviousFieldNames(string typeName, string property)
    {
        object stats = JsonUtility.FromJson("{\"" + property + "\":0.37}", GameType(typeName));
        Assert.That(Get(stats, property), Is.EqualTo(0.37f).Within(0.00001f));
    }

    [Test]
    public void EquippedStatusChangesRefreshViewsAndEquipmentSwitchUpdatesSubscriptions()
    {
        object data = CreateAsset("KaomojiData");
        object leftStats = Activator.CreateInstance(GameType("CriticalStats"));
        object rightStats = Activator.CreateInstance(GameType("CriticalStats"));
        object replacementStats = Activator.CreateInstance(GameType("CriticalStats"));
        object coreStats = Activator.CreateInstance(GameType("CoreStats"));
        object left = CreateSymbol("SymbolEyeData", "LeftEye", "criticalStats", leftStats);
        object right = CreateSymbol("SymbolEyeData", "RightEye", "criticalStats", rightStats);
        object replacement = CreateSymbol("SymbolEyeData", "LeftEye", "criticalStats", replacementStats);
        object hand = CreateSymbol("SymbolHandData", "LeftHand", "coreStats", coreStats);
        Equip(data, left);
        Equip(data, right);
        Equip(data, hand);

        Component handler = root.AddComponent(GameType("UI.CurrentKaomojiHandler"));
        Component calculator = root.AddComponent(GameType("Player.PlayerStatsCalculator"));
        Component text = CreateText("Current face");
        SetField(calculator, "statsConfig", CreateAsset("Player.PlayerCoreStatsConfig"));
        SetField(handler, "playerData", data);
        SetField(handler, "statsCalc", calculator);
        SetField(handler, "currentKaomojiText", text);

        var updates = new UpdateCapture();
        EventInfo statusEvent = handler.GetType().GetEvent("OnStatusUpdated");
        MethodInfo capture = typeof(UpdateCapture).GetMethod(nameof(UpdateCapture.Capture))
            .MakeGenericMethod(statusEvent.EventHandlerType.GetGenericArguments()[0]);
        Delegate subscriber = Delegate.CreateDelegate(statusEvent.EventHandlerType, updates, capture);
        statusEvent.AddEventHandler(handler, subscriber);
        Invoke(handler, "SubscribeToStatusChanges");
        Invoke(handler, "UpdateStatusViews");

        Set(leftStats, "CriticalRate", 0.1f);
        Set(rightStats, "CriticalRate", 0.2f);
        Set(coreStats, "Speed", 0.5f);
        Assert.That(updates.Count, Is.EqualTo(3));
        Assert.That(updates.Value("CriticalRate"), Is.EqualTo(0.3f).Within(0.00001f));
        Assert.That(updates.Value("Speed"), Is.EqualTo(0.5f));

        Equip(data, replacement);
        Invoke(handler, "SubscribeToStatusChanges");
        Set(leftStats, "CriticalRate", 0.15f);
        Assert.That(updates.Count, Is.EqualTo(3));
        Set(replacementStats, "CriticalRate", 0.05f);
        Assert.That(updates.Count, Is.EqualTo(4));
        Assert.That(updates.Value("CriticalRate"), Is.EqualTo(0.25f).Within(0.00001f));

        Invoke(handler, "UnsubscribeFromStatusChanges");
        Set(replacementStats, "CriticalRate", 0.1f);
        Assert.That(updates.Count, Is.EqualTo(4));
    }

    [Test]
    public void CriticalViewTracksSelectedEyeInsteadOfOtherEyes()
    {
        object firstStats = Activator.CreateInstance(GameType("CriticalStats"));
        object secondStats = Activator.CreateInstance(GameType("CriticalStats"));
        object first = CreateSymbol("SymbolEyeData", "LeftEye", "criticalStats", firstStats);
        object second = CreateSymbol("SymbolEyeData", "RightEye", "criticalStats", secondStats);
        Component view = root.AddComponent(GameType("CriticalUIHandler"));
        Component symbolText = CreateText("Selected symbol");
        Component parameterText = CreateText("Critical parameters");
        SetField(view, "select_Symbol_Text", symbolText);
        SetField(view, "critical_Node_Parameter", parameterText);

        Invoke(view, "HandleSymbolSelected", first);
        Set(firstStats, "CriticalRate", 0.1f);
        Assert.That(Get(parameterText, "text").ToString(), Does.Contain(0.1f.ToString("P1")));

        Invoke(view, "HandleSymbolSelected", second);
        string secondText = Get(parameterText, "text").ToString();
        Set(firstStats, "CriticalRate", 0.2f);
        Assert.That(Get(parameterText, "text"), Is.EqualTo(secondText));
        Set(secondStats, "CriticalDamage", 0.3f);
        Assert.That(Get(parameterText, "text").ToString(), Does.Contain(0.3f.ToString("P1")));

        Invoke(view, "OnDisable");
        string disabledText = Get(parameterText, "text").ToString();
        Set(secondStats, "CriticalDamage", 0.4f);
        Assert.That(Get(parameterText, "text"), Is.EqualTo(disabledText));
    }

    public class UpdateCapture
    {
        public int Count { get; private set; }
        object update;

        public void Capture<T>(T value)
        {
            Count++;
            update = value;
        }

        public float Value(string type)
        {
            foreach (object status in (IEnumerable)Get(update, "Statuses"))
            {
                if (Get(status, "Type").ToString() == type)
                    return (float)Get(status, "Value");
            }
            throw new InvalidOperationException("Status not found: " + type);
        }
    }
}
