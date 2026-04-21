using Unity.VisualScripting;
using UnityEngine;

public partial class TestCutsceneFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        if (_playerInput.actions["Jump"].WasPressedThisFrame())
        {
            Machine.Fire(TestCutsceneFsmTrigger.PlayerInputJump);
        }

        if (PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.Jump))
        {
            Machine.Fire(TestCutsceneFsmTrigger.PlayerInJumpState);
        }
    }

    private void OnInteracted()
    {
        Machine.Fire(TestCutsceneFsmTrigger.OnInteracted);
    }
}