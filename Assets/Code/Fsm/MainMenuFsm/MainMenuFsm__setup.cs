using Code.TriggerParams;
using UnityEngine.UI;

public partial class MainMenuFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(MainMenuFsmState.Home)
            .Permit(MainMenuFsmTrigger.HomePlayClicked, MainMenuFsmState.Saves)
            .Permit(MainMenuFsmTrigger.HomeOptionsClicked, MainMenuFsmState.Options)
            .OnEntry(_ =>
            {
                _homeObject.SetActive(true);
                _backButtonObject.SetActive(false);
                _homeObject.transform.Find("Buttons").Find("Play").GetComponent<Button>().Select();
            })
            .OnExit(_ =>
            {
                _homeObject.SetActive(false);
                
            });

        Machine.Configure(MainMenuFsmState.Saves)
            .Permit(MainMenuFsmTrigger.BackClicked, MainMenuFsmState.Home)
            .PermitIf(MainMenuFsmTrigger.SaveClicked, MainMenuFsmState.NewGame, @params =>
            {
                if (@params is not IntParam intParam) return false;
                var saveData = SaveSystem.LoadSaveDataFromDisk(intParam.i);
                return (saveData == null);
            })
            .OnEntry(_ =>
            {
                _savesObject.SetActive(true);
                _backButtonObject.SetActive(true);
                _savesObject.transform.Find("Buttons").Find("SaveSlot1").GetComponent<Button>().Select();
            })
            .OnExit(_ =>
            {
                _savesObject.SetActive(false);
            });
        
        Machine.Configure(MainMenuFsmState.Options)
            .Permit(MainMenuFsmTrigger.BackClicked, MainMenuFsmState.Home)
            .OnEntry(_ =>
            {
                _optionsObject.SetActive(true);
            })
            .OnExit(_ =>
            {
                _optionsObject.SetActive(false);
            });
        
        Machine.Configure(MainMenuFsmState.NewGame)
            .Permit(MainMenuFsmTrigger.BackClicked, MainMenuFsmState.Saves)
            .OnEntry(_ =>
            {
                _newGameObject.SetActive(true);
                _backButtonObject.SetActive(false);
            })
            .OnExit(_ =>
            {
                _newGameObject.SetActive(false);
            });

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
    }
}