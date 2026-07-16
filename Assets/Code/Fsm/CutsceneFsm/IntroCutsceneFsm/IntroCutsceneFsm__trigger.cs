using Unity.VisualScripting;
using UnityEngine;

public partial class IntroCutsceneFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        if (PlayerFsm.Singleton.GetInputMovementVector3().magnitude > PlayerFsm.InputMagnitudeThreshhold)
        {
            Machine.Fire(IntroCutsceneFsmTrigger.OnPlayerMoveInput);
        };
    }

    private void OnInteracted()
    {
        Machine.Fire(IntroCutsceneFsmTrigger.OnInteracted);
    }
}