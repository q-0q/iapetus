using UnityEngine;

public abstract partial class GravityFsm
{
    private void AerialOnUpdate()
    {
        if (Machine.IsInState(GravityFsmState.DontApplyYVelocity)) return;
        var v3 = new Vector3(0, YVelocity * Time.deltaTime, 0);
        transform.position += v3;
        YVelocity -= (GravityStrength * GravityStrength * Time.deltaTime * StateMapConfig.GravityStrengthMod.Get(this));
        TimeInAir += Time.deltaTime;
        UpdateYVelocityMetadata();
    }

    private void AerialConfigure()
    {
        Machine.Configure(GravityFsmState.Aerial)
            .OnEntryFrom(GravityFsmTrigger.StartFrameAerial, _ => { TimeInAir = 0; });
    }
}