using Code.Misc;
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
            .PermitIf(InventoryMenuFsmTrigger.Use, InventoryMenuFsmState.UseConfirmation, _ => true)
            .PermitIf(InventoryMenuFsmTrigger.Movelist, InventoryMenuFsmState.Movelist, _ => MovelistTabCondition())
            .PermitIf(InventoryMenuFsmTrigger.Right, InventoryMenuFsmState.Movelist, _ => MovelistTabCondition())
            .PermitIf(InventoryMenuFsmTrigger.Left, InventoryMenuFsmState.Movelist, _ => MovelistTabCondition())
            .SubstateOf(InventoryMenuFsmState.Open)
            .OnEntry(_ =>
            {
                _listCanvasGroup.GetComponentInChildren<ScrollRect>().content.anchoredPosition = new Vector2(0, 0);
                _listSelection.SetActive(true);
                PopulateBagData(SaveSystem.LoadCachedSaveData());
                OnInventoryTabSelected?.Invoke("Bag");
            });

        Machine.Configure(InventoryMenuFsmState.UseConfirmation)
            .Permit(InventoryMenuFsmTrigger.Back, InventoryMenuFsmState.Bag)
            .Permit(InventoryMenuFsmTrigger.Use, InventoryMenuFsmState.Closed)
            .SubstateOf(InventoryMenuFsmState.Open)
            .OnEntry(_ =>
            {
                _listCanvasGroup.blocksRaycasts = false;
                _useConfirmationCanvasGroup.blocksRaycasts = true;
                var btn = _useConfirmationCanvasGroup.transform.Find("Selection").Find("Buttons").Find("Back")
                    .GetComponent<Button>();
                StartCoroutine(DelayedSelect(btn));
            })
            .OnExit(_ =>
            {
                _listCanvasGroup.blocksRaycasts = true;
                _useConfirmationCanvasGroup.blocksRaycasts = false;
            });
        
        Machine.Configure(InventoryMenuFsmState.Movelist)
            .PermitIf(InventoryMenuFsmTrigger.Bag, InventoryMenuFsmState.Bag, _ => true)
            .PermitIf(InventoryMenuFsmTrigger.Right, InventoryMenuFsmState.Bag, _ => true)
            .PermitIf(InventoryMenuFsmTrigger.Left, InventoryMenuFsmState.Bag, _ => true)
            .SubstateOf(InventoryMenuFsmState.Open)
            .OnEntry(_ =>
            {
                _listCanvasGroup.GetComponentInChildren<ScrollRect>().content.anchoredPosition = new Vector2(0,0);
                _listSelection.SetActive(true);
                PopulateMovelistData(SaveSystem.LoadCachedSaveData());
                OnInventoryTabSelected?.Invoke("Movelist");
            });
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
    }

    public static bool MovelistTabCondition()
    {
        return SaveSystem.LoadCachedSaveData().tricks.Count >= 1;
    }
}