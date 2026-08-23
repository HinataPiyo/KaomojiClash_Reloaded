using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    IEnumerator Start()
    {
        var symboltask = SymbolDataCollection.SymbolDataLoadAsync();
        var arenaTask = ArenaObjectDataCollection.ArenaObjectDataLoadAsync();

        yield return new WaitUntil(() => symboltask.IsCompleted && arenaTask.IsCompleted);

        Debug.Log("SymbolData and ArenaObjectData loaded successfully!");
        SceneManager.LoadScene("GameScene");
    }
}