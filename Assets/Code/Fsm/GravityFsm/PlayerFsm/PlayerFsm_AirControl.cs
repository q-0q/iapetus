using System;
using UnityEngine;

public partial class PlayerFsm
{
    private void AirControlOnUpdate()
    {
        
        // ANTI-PATTERN!
        var increaseMultiplier = 0.1f;
        var decreaseMultiplier = AirControlMomentumDecayModifier;
        var turningMultiplier = AirControlTurningMultiplier;
        var forceForwardInput = true;
        if (Machine.IsInState(PlayerFsmState.Updraft))
        {
            increaseMultiplier = Mathf.Lerp(0.1f, 0.8f, Mathf.InverseLerp(60f, 0f, YVelocity));
            // turningMultiplier = Mathf.Lerp(AirControlTurningMultiplier * 1.5f, AirControlTurningMultiplier, Mathf.InverseLerp(60f, 20f, YVelocity));
            decreaseMultiplier *= 1.5f;
            forceForwardInput = false;
        }
        
        if (Machine.IsInState(PlayerFsmState.PitonFlip))
        {
            // increaseMultiplier = Mathf.Lerp(0.1f, 0.8f, Mathf.InverseLerp(60f, 0f, YVelocity));
            // turningMultiplier = Mathf.Lerp(AirControlTurningMultiplier * 1.5f, AirControlTurningMultiplier, Mathf.InverseLerp(60f, 20f, YVelocity));
            forceForwardInput = false;
        }

        HandleTurning(turningMultiplier, forceForwardInput, AirControlTurningMomentumDecayModifier);
        HandleInputMomentumChange(increaseMultiplier, decreaseMultiplier);
    }
}