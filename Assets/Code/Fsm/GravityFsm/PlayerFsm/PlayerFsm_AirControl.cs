using System;

public partial class PlayerFsm
{
    private void AirControlOnUpdate()
    {
        HandleTurning(AirControlTurningMultiplier, true, AirControlTurningMomentumDecayModifier);
        HandleInputMomentumChange(0f, AirControlMomentumDecayModifier);
    }
}