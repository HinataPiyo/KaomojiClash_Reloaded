using System.Collections;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    IEnumerator Start()
    {
        var symboltask = SymbolDataCollection.SymbolDataLoadAsync();
        var arenaTask = ArenaObjectDataCollection.ArenaObjectDataLoadAsync();

        yield return new WaitUntil(() => symboltask.IsCompleted && arenaTask.IsCompleted);

        Debug.Log("SymbolData and ArenaObjectData loaded successfully!");
        ApiProvider.Get<ISceneChange>().ChangeScene(SceneName.Home);
    }
}