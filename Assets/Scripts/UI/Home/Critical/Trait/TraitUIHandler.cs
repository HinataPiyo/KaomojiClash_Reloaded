using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TraitUIHandler : MonoBehaviour
{
        [Header("特性")]
    [SerializeField] TextMeshProUGUI trait_Level_Text;
    [SerializeField] TextMeshProUGUI trait_Description_Text;
    [SerializeField] Button trait_Select_Again_Button;

    void Reset()
    {
        trait_Level_Text.text = "trait level : -";
        trait_Description_Text.text = "";
    }
}