using UnityEngine;

// Activate報酬としてCriticalDamageだけを上げるノード。
public class CriticalDamageNode : NodeBase
{
    // Activate 1回につきCriticalDamageを15ポイント加算する。
    const float CriticalDamagePerActivation = 0.15f;

    // 目記号のCriticalDamageを既定上限まで増加させる。
    public override void ApplyActivationEffect(SymbolEyeData eyeSymbol)
    {
        if (eyeSymbol == null)
            throw new System.ArgumentNullException(nameof(eyeSymbol));
        if (eyeSymbol.CriticalStats == null)
            throw new System.InvalidOperationException("The selected eye symbol has no CriticalStats.");

        CriticalStats stats = eyeSymbol.CriticalStats;
        stats.CriticalDamage = Mathf.Clamp(
            stats.CriticalDamage + CriticalDamagePerActivation,
            CriticalStats.Min_CriticalDamage,
            CriticalStats.Max_CriticalDamage);
    }
}