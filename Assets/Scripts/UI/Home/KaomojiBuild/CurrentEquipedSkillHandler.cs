using System.Collections.Generic;
using UI;
using UnityEngine;


/// <summary>
/// 現在装着してるスキル一覧を管理するクラス
/// </summary>
public class CurrentEquipedSkillHandler : CurrentKaomojiEventListener
{
    [SerializeField] Transform container;
    [SerializeField] SkillNameButton skillNamePrefab;

    List<SkillNameButton> buttons = new List<SkillNameButton>();

    // [設計変更] 共通リスナーの通知から口データを取り出し、装備一覧だけを更新する。
    protected override void OnCurrentKaomojiChanged(CurrentKaomojiUpdate update)
    {
        if (update.Data == null)
        {
            UpdateSkillList(new SkillData[0]);
            return;
        }

        SymbolMouthData mouth = update.Data.GetSymbolDataByType(SymbolType.Mouth) as SymbolMouthData;
        UpdateSkillList(mouth == null ? new SkillData[0] : mouth.SkillDatas.ToArray());
    }

    void Start()
    {
        SkillNameButton[] btns = container.GetComponentsInChildren<SkillNameButton>();
        for(int i = 0; i < btns.Length; i++)
        {
            Destroy(btns[i].gameObject);
        }
    }


    public void UpdateSkillList(SkillData[] skills)
    {
        ClearButtons();
        foreach(var skill in skills)
        {
            SkillNameButton btn = Instantiate(skillNamePrefab, container);
            btn.SetSkill(skill);
            buttons.Add(btn);
        }
    }

    void ClearButtons()
    {
        foreach(var btn in buttons)
            Destroy(btn.gameObject);
        buttons.Clear();
    }
}