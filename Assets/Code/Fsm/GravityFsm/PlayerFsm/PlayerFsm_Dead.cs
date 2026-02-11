using System.Collections;
using DG.Tweening;
using UnityEngine;

public partial class PlayerFsm
{

    private void DyingOnUpdate()
    {
        Shader.SetGlobalFloat("_PlayerTintWeight", TimeInCurrentState() / 0.3f);
    }

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(0.275f);
        Reset();
    }
    
    private void DeathConfigure()
    {
        Machine.Configure(PlayerFsmState.Dying1)
            .Permit(PlayerFsmTrigger.Timeout, PlayerFsmState.Dead)
            .OnEntry(_ =>
            {
                Time.timeScale = 1f;
                transform.DOShakePosition(0.5f, 0.4f, 30);
                Shader.SetGlobalColor("_PlayerTintColor", Color.white);
                Animator.StartPlayback();
                Animator.enabled = false;
                isSprinting = false;
                ResetCombo();
            });
        
        Machine.Configure(PlayerFsmState.Dying2)
            .Permit(PlayerFsmTrigger.Timeout, PlayerFsmState.Dead)
            .OnEntry(_ =>
            {

            });

        Machine.Configure(PlayerFsmState.Dead)
            .OnEntry(_ =>
            {
                transform.DOShakePosition(0.35f, 0.25f, 20);
                _deathParticles.transform.position = transform.position;
                _deathParticles.Play();
                MakeAllRenderersInvisible();
                StartCoroutine(ResetAfterDelay());
            });
    }
}