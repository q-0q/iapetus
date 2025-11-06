using System;

public partial class PlayerFsm
{
    private void AirControlOnUpdate()
    {
        HandleTurning(AirControlTurningMultiplier, true, AirControlTurningMomentumDecayModifier);
        HandleInputMomentumChange(0.1f, AirControlMomentumDecayModifier);
    }
}