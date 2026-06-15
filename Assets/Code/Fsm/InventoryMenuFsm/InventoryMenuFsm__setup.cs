using Code.TriggerParams;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class InventoryMenuFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(InventoryMenuFsmState.Closed)
            .PermitIf(InventoryMenuFsmTrigger.Opened, InventoryMenuFsmState.Bag, _ => true)
            .OnEntryFrom(InventoryMenuFsmTrigger.Closed, _ =>
            {
                _canvasGroup.blocksRaycasts = false;
                _listCanvasGroup.blocksRaycasts = false;
                _useConfirmationCanvasGroup.blocksRaycasts = false;
        
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
        
                EventSystem.current.SetSelectedGameObject(null);
            });
        
        Machine.Configure(InventoryMenuFsmState.Open)
            .PermitIf(InventoryMenuFsmTrigger.Closed, InventoryMenuFsmState.Closed, _ => true)
            .OnEntryFrom(InventoryMenuFsmTrigger.Opened, _ =>
            {
                _canvasGroup.blocksRaycasts = true;
                _listCanvasGroup.blocksRaycasts = true;
                _useConfirmationCanvasGroup.blocksRaycasts = false;
                _useConfirmationCanvasGroup.alpha = 0;
                
                if (TutorialCanvas.Singleton.GetCurrentAction() == "Inventory") TutorialCanvas.Singleton.HideTutorialText();
            });

        Machine.Configure(InventoryMenuFsmState.Bag)
            .SubstateOf(InventoryMenuFsmState.Open)
            .OnEntry(_ =>
            {
                GetComponentsInChildren<Selectable>()[0].Select();
            });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
    }
}