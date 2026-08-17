using Unity.VisualScripting;
using UnityEngine;

public partial class SentryFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();
        
        
        var toPlayer = GetPlayerPosition() - eye.position;
        if (Physics.Raycast(eye.position, toPlayer, out var hit, toPlayer.magnitude, GetEnvironmentalLayermask(),
                QueryTriggerInteraction.Ignore))
        {
            Machine.Fire(SentryFsmTrigger.PlayerOutOfView);
            return;
        }
        
        if (toPlayer.magnitude > 50f) 
        {
            Machine.Fire(SentryFsmTrigger.PlayerOutOfView);
            return;
        }

        _obstructionTimer = 0f;
        Machine.Fire(SentryFsmTrigger.PlayerInView);
        
        
        if (_blinking && _blinkTimer > 1.5f)
        {
            Machine.Fire(SentryFsmTrigger.Shoot);
        }
    }
    
}