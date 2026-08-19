using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    IEnumerator Start()
    {
        var task = SymbolDataCollection.SymbolDataLoadAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        
        Debug.Log("SymbolData loaded successfully!");
        SceneManager.LoadScene("GameScene");
    }
}