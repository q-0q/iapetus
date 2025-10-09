public partial class RaceFsm
{

    public override void OnFireTriggers()
    {
        base.OnFireTriggers();


    }

    private void OnRaceTrigger(RaceTrigger trigger)
    {
        if (Machine.IsInState(RaceFsmState.Disabled)) return;
        
        var id = -1;
        for (var i = 0; i < Triggers.Count; i++)
        {
            if (Triggers[i] == trigger)
            {
                id = i;
                break;
            }
        }
        
        if (_currentTriggerId == id - 1 || (_currentTriggerId == Triggers.Count - 1 && id == 0))
        {
            if (id == 0)
            {
                Machine.Fire(RaceFsmTrigger.StartTriggered);
            }
            _currentTriggerId = id;

            var next = _currentTriggerId == Triggers.Count - 1 ? 0 : _currentTriggerId + 1;
            Triggers[_currentTriggerId].Hide();
            Triggers[next].MarkNext();
        }
        
    }
    
    private void OnNotRaceTrigger(RaceTrigger trigger)
    {
        Machine.Fire(RaceFsmTrigger.StartNotTriggered);
    }
}