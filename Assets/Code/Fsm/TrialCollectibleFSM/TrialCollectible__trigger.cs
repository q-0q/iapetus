using Unity.VisualScripting;
using UnityEngine;

public partial class TrialCollectibleFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();

        Machine.Fire(Physics.CheckSphere(_keyframes[0].transform.position, 2f, LayerMask.GetMask("Player"))
            ? TrialCollectibleFsmTrigger.PlayerEnteredStartingZone
            : TrialCollectibleFsmTrigger.PlayerExitedStartingZone);

        if (_currentKeyframeIndex == _keyframes.Count)
        {
            Machine.Fire(TrialCollectibleFsmTrigger.PlayerEnteredEndingZone);
        }
        
        else if (_timeOnCurrentKeyframe > _keyframes[_currentKeyframeIndex].duration)
        {
            Machine.Fire(TrialCollectibleFsmTrigger.KeyframeTimeout);
        }
    }
    
    
}