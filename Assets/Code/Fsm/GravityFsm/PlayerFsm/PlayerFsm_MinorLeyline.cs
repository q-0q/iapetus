using Code.Misc;
using UnityEngine;

public partial class PlayerFsm
{
    private void MinorLeylineConfigure()
    {
        Machine.Configure(PlayerFsmState.MinorLeylineInteractable)
            .Permit(PlayerFsmTrigger.MinorLeylineTrigger, PlayerFsmState.MinorLeylineStartup);
        
        Machine.Configure(PlayerFsmState.MinorLeylineStartup)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.MinorLeylineActive)
            .OnEntry(_ =>
            {
                Util.InvokeSphereEffect(transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );

                _speedLinesParticlesDuration = -1f;
                _speedLinesParticles.Play();
                YVelocity = 0;
                _minorLeylineHalo.Show();
                MakeAllRenderersInvisible();
                _currentMinorLeylineDirection = _currentMinorLeyline.GetDirectionFromTrigger(_currentMinorLeylineTrigger);
                _currentMinorLeylineWeight = _currentMinorLeylineDirection ? 0f : 1f;
            })
            .OnExit(_ =>
            {

            })
            .SubstateOf(GravityFsmState.IgnoreDepenetration);
        
        Machine.Configure(PlayerFsmState.MinorLeylineActive)
            .Permit(PlayerFsmTrigger.Jump, PlayerFsmState.Jump)
            .OnEntry(_ =>
            {

            })
            .SubstateOf(GravityFsmState.IgnoreDepenetration)
            .OnExit(_ =>
            {
                Util.InvokeSphereEffect(transform.position + Vector3.up, Vector3.one * 5f, 1.5f, 1f, 0 );
                isSprinting = true;
                _timeSinceMinorLeyline = 0;
                _minorLeylineHalo.Hide();
                transform.rotation = _currentMinorLeyline.GetPlayerRotationAt(_currentMinorLeylineWeight, _currentMinorLeylineDirection, out var yVelocityMod);
                _momentum = Mathf.Lerp(MaxMomentum, 6f, yVelocityMod);
                PlaySpeedLineParticlesForDuration(Mathf.Lerp(0.75f, 0.25f, yVelocityMod));
                YVelocity = Mathf.Max(YVelocity, 20f * yVelocityMod + 20f);
                MakeAllRenderersVisible();
            });
    }
    
    private void MinorLeylineStartupOnUpdate()
    {
        var speedModifier = Mathf.Lerp(0.25f, 1f, Mathf.InverseLerp(0.3f, 0.45f, TimeInCurrentState()));
        var lerpSpeed = Mathf.Lerp(0.05f, 3f, Mathf.InverseLerp(0.3f, 0.5f, TimeInCurrentState()));
        MoveAlongCurrentMinorLeyline(speedModifier, lerpSpeed);
     
        // MoveAlongCurrentMinorLeyline(1f, 3f);
    }
    
    private void MinorLeylineActiveOnUpdate()
    {
     
        MoveAlongCurrentMinorLeyline(1f, 3f);
    }

    private void MoveAlongCurrentMinorLeyline(float speedModifier, float lerpSpeed)
    {
        var newWeight = _currentMinorLeylineWeight +
            Time.deltaTime * MinorLeylineSpeed * speedModifier * (_currentMinorLeylineDirection ? 1f : -1f) / _currentMinorLeyline.Length();

        transform.position = Vector3.Lerp(transform.position, _currentMinorLeyline.EvaluatePosition(newWeight), lerpSpeed);
        _currentMinorLeylineWeight = newWeight;
    }
}