using System.Collections;
using Code.Misc;
using UnityEngine;

public partial class PlayerFsm
{

    private void TinsicaOnUpdate()
    {
        var speedMod = Mathf.Lerp(1f, Mathf.Lerp(1.75f, 1f, Mathf.InverseLerp(0.2f, 0.45f, TimeInCurrentState())), 
            Mathf.InverseLerp(0.1f, 0.2f, TimeInCurrentState()));
        HandleCollisionMove(speedMod, false);
        HandleTurning(1f, false, 0f, false, 0.25f);
        SetAnimatorSpeedMod();
    }

    private void TinsicaJumpOnUpdate()
    {
        Animator.SetLayerWeight(1, 0);
        _momentum = Mathf.Max(_momentum, TinsicaEntryMomentum);
        transform.position += ComputeCollisionMove(transform.forward * (Time.deltaTime * 5f));
    }
    
    private void TrickConfigure()
    {
        
        Machine.Configure(PlayerFsmState.TinsicaUsable)
            .PermitIf(PlayerFsmTrigger.Trick, PlayerFsmState.Tinsica, _ =>
            {
                return PlayerManaManager.Singleton.GetCurrentAvailableMana() >= 1;
            });
        
        Machine.Configure(PlayerFsmState.Tinsica)
            .SubstateOf(GravityFsmState.Grounded)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.TinsicaJump, _ =>
            {
                return TimeInCurrentState() > 0.4 * ComputeTiniscaDurationMod() && PlayerManaManager.Singleton.GetCurrentAvailableMana() >= 1;;
            }, 2)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jump, _ =>
            {
                return TimeInCurrentState() > 0.3 * ComputeTiniscaDurationMod()  && PlayerManaManager.Singleton.GetCurrentAvailableMana() < 1;
            })
            .PermitIf(FsmTrigger.Timeout, PlayerFsmState.GroundMove, _ =>
            {
                var durationMod = ComputeTiniscaDurationMod();
                var duration = TinsicaDuration * durationMod;
                return TimeInCurrentState() >= duration;
            })
            .OnEntry(_ =>
            {
                StartCoroutine(TrickTintCoroutine(0.155f));
                _playerTrickParticles.InvokeTinsica();
                _momentum = Mathf.Max(_momentum, TinsicaEntryMomentum);
                PlayerManaManager.Singleton.Consume();
            });
        
        Machine.Configure(PlayerFsmState.TinsicaJumpsquat)
            .SubstateOf(GravityFsmState.Grounded)
            .SubstateOf(PlayerFsmState.LockMomentum)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.TinsicaJump)
            .OnEntry(_ =>
            {
                Animator.SetLayerWeight(1, 0);
                _inputBuffer.ConsumeBuffer("Jump");
                FMODUnity.RuntimeManager.PlayOneShotAttached(jumpFmodEvent, gameObject);
                OnPlayerFootstep();
            })
            .OnExitFrom(FsmTrigger.Timeout, _ =>
            {
                OnPlayerFootstep();
            });
        
        Machine.Configure(PlayerFsmState.TinsicaJump)
            .SubstateOf(GravityFsmState.Aerial)
            .SubstateOf(PlayerFsmState.Landable)
            .SubstateOf(PlayerFsmState.AirControl)
            .SubstateOf(PlayerFsmState.PitonInteractable)
            .Permit(FsmTrigger.Timeout, PlayerFsmState.Fall)
            .PermitIf(PlayerFsmTrigger.Dash, PlayerFsmState.Dashsquat, CanDash)
            .SubstateOf(PlayerFsmState.WallInteractable)
            .PermitIf(GravityFsmTrigger.StartFrameGrounded, PlayerFsmState.LandsquatAfterDash, @params => !IsSlideTrigger(@params) && YVelocity < 0.5f && _momentum > 5f, 1)
            // .PermitIf(PlayerFsmTrigger.IsAboveWater, PlayerFsmState.DiveFall, _ =>
            // {
            //     return TimeInCurrentState() > 0.6f;
            // })
            .OnExitFrom(GravityFsmTrigger.StartFrameGrounded, _ =>
            {
                EndSurge();
            })
            .OnEntry(_ =>
            {
                // _momentum = 5f;
                StartCoroutine(TrickTintCoroutine(0.1f));
                _inputBuffer.ConsumeBuffer("Jump");
                _playerTrickParticles.InvokeTinsicaJump();
                PlayerManaManager.Singleton.Consume();
                YVelocity = 21f;
            });
    }

    private float ComputeTiniscaDurationMod()
    {
        return Mathf.Lerp(1.3f, 1f, Mathf.InverseLerp(TinsicaEntryMomentum, MaxMomentum, _momentum));
    }

    private IEnumerator TrickTintCoroutine(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        var t = 0f;
        var d = 0.15f;
        Shader.SetGlobalColor("_PlayerTintColor", Color.white);
        while (t < d)
        {
            var w = Util.SmoothLerp01(t / d);

            Shader.SetGlobalFloat("_PlayerTintWeight", 1f - w);
            t += Time.deltaTime;
            yield return null;
        }

        Shader.SetGlobalFloat("_PlayerTintWeight", 0);
    }
}