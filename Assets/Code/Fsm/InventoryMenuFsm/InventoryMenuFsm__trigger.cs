using Code.TriggerParams;
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
}