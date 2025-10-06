using System;

public partial class PlayerFsm
{
    private void AirControlOnUpdate()
    {
        HandleTurning(0.8f, true, 0.15f);
        HandleInputMomentumChange(0f, 0.5f);
    }
}