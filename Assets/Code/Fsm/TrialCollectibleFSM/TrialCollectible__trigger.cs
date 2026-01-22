using Unity.VisualScripting;
using UnityEngine;

public partial class TrialCollectibleFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        if (!PlayerFsm.Singleton.Machine.IsInState(PlayerFsm.PlayerFsmState.TrialTeleport))
        {
            Machine.Fire(Physics.CheckSphere(_keyframes[0].transform.position, 3f, LayerMask.GetMask("Player"))
                ? TrialCollectibleFsmTrigger.PlayerEnteredStartingZone
                : TrialCollectibleFsmTrigger.PlayerExitedStartingZone);
        }

        if (_currentKeyframeIndex >= _keyframes.Count)
        {
            _completionTime = TimeInCurrentState();
            Machine.Fire(TrialCollectibleFsmTrigger.PlayerEnteredEndingZone);
        }
        
        else if (_timeOnCurrentKeyframe > _keyframes[_currentKeyframeIndex].duration)
        {
            Machine.Fire(TrialCollectibleFsmTrigger.KeyframeTimeout);
        }
    }
    
    
}