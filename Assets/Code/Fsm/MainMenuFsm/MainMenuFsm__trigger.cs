using Code.TriggerParams;
using Unity.VisualScripting;
using UnityEngine;

public partial class MainMenuFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }
    
    public void OnBackClicked()
    {
        Machine.Fire(MainMenuFsmTrigger.BackClicked);
    }

    public void OnHomePlayClicked()
    {
        Machine.Fire(MainMenuFsmTrigger.HomePlayClicked);
    }
    
    public void OnHomeOptionsClicked()
    {
        Machine.Fire(MainMenuFsmTrigger.HomeOptionsClicked);
    }
    
    public void OnDiscordClicked()
    {
        Application.OpenURL("https://discord.gg/ea8Rys6cgR");
    }
    
    public void OnSaveClicked(int saveId)
    {
        MetaSaveSystem.WriteSaveId(saveId);
        var saveData = SaveSystem.LoadSaveDataFromId(saveId);
        if (saveData != null)
        {
            SceneLoader.Singleton.LoadScene(saveData.scene);
        }
        else
        {
            SaveSystem.ClearCache();
        }
        
        Machine.Fire(MainMenuFsmTrigger.SaveClicked, new IntParam() {i = saveId});
    }
}