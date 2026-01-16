using System;
using UnityEngine;

public partial class PlayerFsm
{
    private void AirControlOnUpdate()
    {
        
        // ANTI-PATTERN!
        var increaseMultiplier = 0.1f;
        var turningMultiplier = AirControlTurningMultiplier;
        if (Machine.IsInState(PlayerFsmState.Updraft))
        {
            increaseMultiplier = Mathf.Lerp(0.1f, 0.75f, Mathf.InverseLerp(60f, 0f, YVelocity));
            turningMultiplier = Mathf.Lerp(AirControlTurningMultiplier * 1.5f, AirControlTurningMultiplier, Mathf.InverseLerp(60f, 20f, YVelocity));
        }
        
        HandleTurning(turningMultiplier, true, AirControlTurningMomentumDecayModifier);
        HandleInputMomentumChange(increaseMultiplier, AirControlMomentumDecayModifier);
    }
}