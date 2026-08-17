using System.Collections;
using UnityEngine;

public partial class SentryFsm
{

    public override void SetupMachine()
    {
        base.SetupMachine();

        Machine.Configure(SentryFsmState.Idle)
            .Permit(SentryFsmTrigger.PlayerInView, SentryFsmState.Wake)
            .OnEntry(_ =>
            {
                _blinking = false;
                _laserEnd.SetActive(false);
            });

        
        Machine.Configure(SentryFsmState.Wake)
            .Permit(FsmTrigger.Timeout, SentryFsmState.Tracking)
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
                
            });;

        Machine.Configure(SentryFsmState.Tracking)
            .Permit(SentryFsmTrigger.PlayerOutOfView, SentryFsmState.Extrapolating)
            .Permit(SentryFsmTrigger.Shoot, SentryFsmState.Firing)
            .OnEntry(_ =>
            {
                _laserEnd.SetActive(true);
            });
        
        Machine.Configure(SentryFsmState.Extrapolating)
            .PermitIf(SentryFsmTrigger.PlayerOutOfView, SentryFsmState.Searching, _ => _obstructionTimer >= MaxObstructionDuration)
            .Permit(FsmTrigger.Timeout, SentryFsmState.Searching)
            .Permit(SentryFsmTrigger.Shoot, SentryFsmState.Firing)
            .Permit(SentryFsmTrigger.PlayerInView, SentryFsmState.Tracking)
            .OnEntry(_ =>
            {

                _obstructionTimer = 0f;
            });
        
        Machine.Configure(SentryFsmState.Searching)
            .Permit(SentryFsmTrigger.PlayerInView, SentryFsmState.Tracking)
            .Permit(SentryFsmTrigger.Shoot, SentryFsmState.Firing)
            .Permit(FsmTrigger.Timeout, SentryFsmState.Idle)
            .OnEntry(_ =>
            {
                _searchEnterSpeed = currentAngularVelocity;
            });
        
        Machine.Configure(SentryFsmState.Firing)
            .Permit(FsmTrigger.Timeout, SentryFsmState.Tracking)
            .OnEntry(_ =>
            {
                _laserEnd.SetActive(false);
                _blinking = false;
                StartCoroutine(FiringCoroutine());
            });
        

    }

    private IEnumerator FiringCoroutine()
    {
        var prefab = Resources.Load("Prefab/SentryProjectile") as GameObject;
        var count = 3;
        yield return new WaitForSeconds(0.125f);
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(prefab, eye.position, Quaternion.identity);
            obj.GetComponent<SentryProjectile>().SetDirection(eye.forward);
            yield return new WaitForSeconds(0.15f);
        }
        yield return null;
        
    }

    public override void SetupStateMaps()
    {
        base.SetupStateMaps();
        
        StateMapConfig.Duration.Add(SentryFsmState.Wake, 0.5f);
        StateMapConfig.Duration.Add(SentryFsmState.Searching, 2.75f);
        StateMapConfig.Duration.Add(SentryFsmState.Tracking, 2f);
        StateMapConfig.Duration.Add(SentryFsmState.Extrapolating, 1f);
        
    }
}