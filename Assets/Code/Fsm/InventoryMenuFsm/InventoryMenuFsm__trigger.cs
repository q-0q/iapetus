using Code.TriggerParams;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public partial class InventoryMenuFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        
        // if ()
        var moveValueX = _playerInput.actions["Move"].ReadValue<Vector2>().x;
        var held = Mathf.Abs(moveValueX) > 0.1f;
        if ((TimeInCurrentState() > 0.3f || !_xMoveInput) && held)
        {
            var t = moveValueX > 0 ? InventoryMenuFsmTrigger.Right : InventoryMenuFsmTrigger.Left;
            Machine.Fire(t);
        }

        _xMoveInput = held;
    }

    private void Open()
    {
        Machine.Fire(InventoryMenuFsmTrigger.Opened);
    }

    private void Close()
    {
        Machine.Fire(InventoryMenuFsmTrigger.Closed);
    }
    
    public void Back()
    {
        Machine.Fire(InventoryMenuFsmTrigger.Back);
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
        
        Machine.Fire(InventoryMenuFsmTrigger.Use);
    }
}