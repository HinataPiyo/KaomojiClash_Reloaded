using TMPro;
using UnityEngine;

public class KaomojiSetUp : MonoBehaviour
{
    KaomojiData kaomojiData;
    [SerializeField] TextMeshPro kaomojiBody;
    [SerializeField] KaomojiData test_data;
    static readonly char default_FaceLineLeft = '(';
    static readonly char default_FaceLineRight = ')';

    void Start()
    {
        kaomojiData = test_data;
        SetUp(kaomojiData);
    }

    public void SetUp(KaomojiData data)
    {
        string kaomoji = GetKaomojiCoupling(data);
        kaomojiBody.text = kaomoji;
    }

    /// <summary>
    /// 指定されたKaomojiDataから顔文字を生成して返す
    /// </summary>
    public static string GetKaomojiCoupling(KaomojiData data)
    {
        if (data == null) return string.Empty;

        SymbolData leftHand = data.GetSymbolDataByType(SymbolTyp.LeftHand);
        SymbolData leftEye = data.GetSymbolDataByType(SymbolTyp.LeftEye);
        SymbolData mouth = data.GetSymbolDataByType(SymbolTyp.Mouth);
        SymbolData rightEye = data.GetSymbolDataByType(SymbolTyp.RightEye);
        SymbolData rightHand = data.GetSymbolDataByType(SymbolTyp.RightHand);

        int length = 0;
        if (leftHand != null) length++;
        if (leftEye != null) length++;
        if (mouth != null) length++;
        if (rightEye != null) length++;
        if (rightHand != null) length++;

        if (length == 0) return string.Empty;

        length += 2;    // 両端の括弧分を追加

        char[] buffer = new char[length];
        int index = 0;

        if (leftHand != null) buffer[index++] = leftHand.Symbol;
        buffer[index++] = default_FaceLineLeft;         // 左側の括弧を追加
        if (leftEye != null) buffer[index++] = leftEye.Symbol;
        if (mouth != null) buffer[index++] = mouth.Symbol;
        if (rightEye != null) buffer[index++] = rightEye.Symbol;
        buffer[index++] = default_FaceLineRight;        // 右側の括弧を追加
        if (rightHand != null) buffer[index++] = rightHand.Symbol;

        return new string(buffer);
    }
}