using System.Collections.Generic;
using UnityEngine.Serialization;

public partial class RaceFsm
{
    public List<RaceTrigger> Triggers;
    private int _currentTriggerId;
    public bool RequireReturnToStart = true;

    private void OnPlayerReset()
    {
        Machine.Fire(RaceFsmTrigger.Reset);
    }
}