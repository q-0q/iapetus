using DG.Tweening;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class SnailMerchantFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(SnailMerchantFsmState.Idle)
            .Permit(SnailMerchantFsmTrigger.OnInteracted, SnailMerchantFsmState.Speaking);
        
        Machine.Configure(SnailMerchantFsmState.Speaking)
            .Permit(SnailMerchantFsmTrigger.OnDialogueCompleted, SnailMerchantFsmState.Idle);

    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.Idle, "Eating");
        StateMapConfig.AnimationTrigger.Add(SnailMerchantFsmState.Speaking, "Stand");
    }
}