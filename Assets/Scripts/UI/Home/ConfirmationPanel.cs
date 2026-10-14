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

        public void Initialize(System.Action onYes, System.Action onNo)
        {
            message.text = "";
            yes.onClick.AddListener(() => onYes?.Invoke());
            no.onClick.AddListener(() => onNo?.Invoke());
        }

        public void Show(string messageText)
        {
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