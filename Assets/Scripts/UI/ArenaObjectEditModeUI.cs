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
public class ArenaObjectEditModeUI : MonoBehaviour, IArenaObjectEditModeUI
{
    [SerializeField] Button doneButton;

    IArenaObjectSelect arenaObjectSelect;

    void Awake()
    {
        ApiProvider.Register<IArenaObjectEditModeUI>(this);
        doneButton.onClick.AddListener(OnDoneButtonClick);
    }

    void Start()
    {
        arenaObjectSelect = ApiProvider.Get<IArenaObjectSelect>();
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