using DG.Tweening;
using UnityEngine;
using Wasp;

public partial class PlayerFsm
{
    private void PlungeOnUpdate()
    {
        
    }

    private void SwimOnUpdate()
    {
        
    }

    private void SwimConfigure()
    {
        Machine.Configure(PlayerFsmState.Swim);
        
        Machine.Configure(PlayerFsmState.SwimSurfaceRise);
        
        Machine.Configure(PlayerFsmState.SwimSurface);
    }


    private bool IsSwimTrigger(TriggerParams triggerParams)
    {
        if (triggerParams is not RaycastHitParam raycastHitParam) return false;
        var distance = raycastHitParam.Hit.distance;
        return distance < 2f;
    }
}