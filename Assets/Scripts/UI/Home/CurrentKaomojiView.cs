namespace UI
{
    using TMPro;
    using UnityEngine;
    
    public class CurrentKaomojiView : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI kaomojiText;
        [SerializeField] KaomojiData player_KaomojiData;

        void OnEnable()
        {
            kaomojiText.text = KaomojiSetUp.GetKaomojiCoupling(player_KaomojiData);
        }
    }
}