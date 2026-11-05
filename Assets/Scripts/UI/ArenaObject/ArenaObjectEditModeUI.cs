using UnityEngine;
using UnityEngine.UI;

interface IArenaObjectEditModeUI
{
    void Show();
    void Hide();
}

/// <summary>
/// ここではEditModeが終了したときに押下するButtonの処理を行う
/// </summary>
public class ArenaObjectEditModeUI : SceneSingleton<ArenaObjectEditModeUI>, IArenaObjectEditModeUI
{
    [SerializeField] Button doneButton;

    IArenaObjectSelect arenaObjectSelect;

    protected override void OnSingletonAwake()
    {
        doneButton.onClick.AddListener(OnDoneButtonClick);
    }

    void Start()
    {
        arenaObjectSelect = ArenaObjectController.Instance;
        Hide();
    }

    void OnDoneButtonClick()
    {
        arenaObjectSelect.SetEditDone(true); // ArenaObjectの編集が完了したことを通知
        Hide(); // EditMode UIを非表示にする
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}