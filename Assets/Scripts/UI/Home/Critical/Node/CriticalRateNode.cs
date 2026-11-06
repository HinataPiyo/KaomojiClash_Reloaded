using UnityEngine;

// Activate報酬としてCriticalRateだけを上げるノード。
public class CriticalRateNode : NodeBase
{
    // Activate 1回につきCriticalRateを2.5ポイント加算する。
    const float CriticalRatePerActivation = 0.025f;

    // 目記号のCriticalRateを既定上限まで増加させる。
    public override void ApplyActivationEffect(SymbolEyeData eyeSymbol)
    {
        if (eyeSymbol == null)
            throw new System.ArgumentNullException(nameof(eyeSymbol));
        if (eyeSymbol.CriticalStats == null)
            throw new System.InvalidOperationException("The selected eye symbol has no CriticalStats.");

        CriticalStats stats = eyeSymbol.CriticalStats;
        stats.CriticalRate = Mathf.Clamp(
            stats.CriticalRate + CriticalRatePerActivation,
            CriticalStats.Min_CriticalRate,
            CriticalStats.Max_CriticalRate);
    }
}