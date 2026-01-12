using UnityEngine;

public abstract partial class GravityFsm
{
    private void AerialOnUpdate()
    {
        GustYVelocityBonus -= (GravityStrength * GravityStrength * Time.deltaTime * 0.75f);
        if (Machine.IsInState(GravityFsmState.DontApplyYVelocity)) return;
        var total = Mathf.Min(YVelocity + GustYVelocityBonus, 45f);
        
        var v3 = new Vector3(0, total * Time.deltaTime, 0);
        transform.position += v3;
        YVelocity -= (GravityStrength * GravityStrength * Time.deltaTime * StateMapConfig.GravityStrengthMod.Get(this));
        TimeInAir += Time.deltaTime;
        UpdateYVelocityMetadata();
        HandleGust();
    }

    private void AerialConfigure()
    {
        Machine.Configure(GravityFsmState.Aerial)
            .OnEntryFrom(GravityFsmTrigger.StartFrameAerial, _ => { TimeInAir = 0; });
    }
}