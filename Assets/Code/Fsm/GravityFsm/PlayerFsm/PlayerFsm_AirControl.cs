using System;

public partial class PlayerFsm
{
    private void AirControlOnUpdate()
    {
        HandleTurning(AirControlTurningMultiplier, true, AirControlTurningMomentumDecayModifier);
        
        // ANTI-PATTERN!
        var increaseMultiplier = 0.1f;
        if (Machine.IsInState(PlayerFsmState.Updraft))
        {
            increaseMultiplier = 0.4f;
        }
        HandleInputMomentumChange(increaseMultiplier, AirControlMomentumDecayModifier);
    }
}