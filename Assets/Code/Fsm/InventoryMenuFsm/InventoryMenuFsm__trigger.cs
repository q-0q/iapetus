using Code.TriggerParams;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public partial class InventoryMenuFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
    }

    private void Open()
    {
        Machine.Fire(InventoryMenuFsmTrigger.Opened);
    }

    private void Close()
    {
        Machine.Fire(InventoryMenuFsmTrigger.Closed);
    }

    private void OnListItemUsed(InventoryListItem.InventoryListItemData data)
    {

        var keyItemRegistration = KeyItemRegistry.KeyItemRegistrations[data.id];
        
        
        if (!keyItemRegistration.GetCanUse())
        {
            _listSelectionUseDescription.transform.DOComplete();
            _listSelectionUseDescription.transform.DOPunchPosition(Vector3.right * 10f, 0.25f, 30, 1f);
            return;
        }
        
        // ConfirmationViewOpen();
        
        Machine.Fire(InventoryMenuFsmTrigger.Use);
    }
}