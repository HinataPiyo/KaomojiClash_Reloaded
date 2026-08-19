using TMPro;
using UnityEngine;

public class KaomojiSetUp : MonoBehaviour
{
    [SerializeField] TextMeshPro kaomojiBody;
    static readonly char default_FaceLineLeft = '(';
    static readonly char default_FaceLineRight = ')';

    public void SetUp(KaomojiData data)
    {
        string kaomoji = GetKaomojiCoupling(data);
        Debug.Log($"Setting up Kaomoji: {kaomoji}");
        kaomojiBody.text = kaomoji;
    }

    /// <summary>
    /// 指定されたKaomojiDataから顔文字を生成して返す
    /// </summary>
    public static string GetKaomojiCoupling(KaomojiData data)
    {
        if (data == null) return string.Empty;

        SymbolData leftHand = data.GetSymbolDataByType(SymbolType.LeftHand);
        SymbolData leftEye = data.GetSymbolDataByType(SymbolType.LeftEye);
        SymbolData mouth = data.GetSymbolDataByType(SymbolType.Mouth);
        SymbolData rightEye = data.GetSymbolDataByType(SymbolType.RightEye);
        SymbolData rightHand = data.GetSymbolDataByType(SymbolType.RightHand);

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