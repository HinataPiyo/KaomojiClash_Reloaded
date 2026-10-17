namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    /// <summary>
    /// 確認画面のパネルを管理するクラス
    /// </summary>
    public class ConfirmationPanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI message;
        [SerializeField] Button yes;
        [SerializeField] Button no;

        void Awake()
        {
            Initialize();
        }

        void Initialize()
        {
            no.onClick.AddListener(() => { Hide(); });
            Hide();
        }

        public void Show(string messageText, System.Action onYes)
        {
            yes.onClick.RemoveAllListeners();
            yes.onClick.AddListener(() => { onYes?.Invoke(); });
            message.text = messageText;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            message.text = "";
            gameObject.SetActive(false);
        }
    }
}