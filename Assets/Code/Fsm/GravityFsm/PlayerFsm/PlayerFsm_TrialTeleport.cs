using Code.Misc;
using UnityEngine;

public partial class PlayerFsm
{
    

    private void TrialTeleportOnUpdate()
    {
        var w = Mathf.InverseLerp(TrialTeleportStartupDuration, TrialTeleportDuration - TrialTeleportStartupDuration, TimeInCurrentState());
        w = Util.SmoothLerp01(w);
        transform.position = Vector3.Lerp(_teleportOrigin, _teleportDestination, w);
        
        if (TimeInCurrentState() > TrialTeleportStartupDuration && PreviousTimeInCurrentState() < TrialTeleportStartupDuration)
        {
            PlayerCinemachineFreeLook.Singleton.OnPlayerCinemachineFreeLookScript(_teleportDirection, TrialTeleportDuration - TrialTeleportStartupDuration * 2f);
        }
    }
    
    private void TrialTeleportConfigure()
    {
        Machine.Configure(PlayerFsmState.TrialTeleport)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.GroundMove)
            .SubstateOf(GravityFsmState.IgnoreDepenetration)
            .OnEntry(_ =>
            {
                _teleportOrigin = transform.position;
                foreach (var r in _renderers)
                {
                    r.enabled = false;
                }
            })
            .OnExit(_ =>
            {
                YVelocity = 0;
                _momentum = 0;
                foreach (var r in _renderers)
                {
                    r.enabled = true;
                }
            });
    }
}