using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public partial class PlayerFsm
{
    public static event Action OnPlayerRespawn;

    private void DyingOnUpdate()
    {
        Shader.SetGlobalFloat("_PlayerTintWeight", TimeInCurrentState() / 0.3f);
    }
    
    private void RespawnOnUpdate()
    {
        Shader.SetGlobalFloat("_PlayerTintWeight", Mathf.InverseLerp(0.5f, 0, TimeInCurrentState()));
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(0.275f);
        // Reset();
        
        
    }
    
    private void DeathConfigure()
    {
        Machine.Configure(PlayerFsmState.Dying1)
            .Permit(PlayerFsmTrigger.Timeout, PlayerFsmState.Dead)
            .SubstateOf(PlayerFsmState.DisplaceFoliage)
            .OnEntry(_ =>
            {
                FMODUnity.RuntimeManager.PlayOneShot(deathFmodEvent);
                Time.timeScale = 1f;
                YVelocity = 0;
                
                Shader.SetGlobalColor("_PlayerTintColor", Color.white);
                Animator.StartPlayback();
                Animator.enabled = false;
                isSprinting = false;
                _deathParticles.PlayDeath();
                EndSurge();
            });
        
        Machine.Configure(PlayerFsmState.Dying2)
            .Permit(PlayerFsmTrigger.Timeout, PlayerFsmState.Dead)
            .OnEntry(_ =>
            {

            });

        Machine.Configure(PlayerFsmState.Dead)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Respawn)
            .OnEntry(_ =>
            {
                YVelocity = 0;
                // _skinnedMeshRenderer.transform.DOShakePosition(0.35f, 0.25f, 20);
                _deathParticles.transform.position = transform.position;
                MakeAllRenderersInvisible();
            });
        
        
        Machine.Configure(PlayerFsmState.Respawn)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.IdleLong)
            .SubstateOf(GravityFsmState.Grounded)
            .OnEntry(_ =>
            {
                _momentum = 0;
                LastUpwardsY = transform.position.y;
                // _skinnedMeshRenderer.transform.DOComplete();
                Invoke(nameof(MakeAllRenderersVisible), 0.45f);
                Animator.StopPlayback();
                Animator.enabled = true;
                Animator.SetLayerWeight(1, 0);
                Shader.SetGlobalFloat("_PlayerTintWeight", 0);
                var initialPosition = transform.position;
                transform.position = _safeGroundPosition;
                OnPlayerTeleported?.Invoke(transform.position - initialPosition);
                // OnPlayerRespawn?.Invoke();
            })
            .OnExit(_ =>
            {
                Shader.SetGlobalFloat("_PlayerTintWeight", 0);
            });
    }
}