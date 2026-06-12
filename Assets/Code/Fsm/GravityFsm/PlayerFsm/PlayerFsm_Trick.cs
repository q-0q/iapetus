using System;
using System.Collections;
using System.Collections.Generic;
using Code.Misc;
using UnityEngine;

public partial class PlayerFsm
{

    public static event Action<string> OnTrickAcquired;

    private void TinsicaOnUpdate()
    {
        var speedMod = Mathf.Lerp(1f, Mathf.Lerp(1.75f, 1f, Mathf.InverseLerp(0.2f, 0.45f, TimeInCurrentState())), 
            Mathf.InverseLerp(0.1f, 0.2f, TimeInCurrentState()));
        HandleCollisionMove(speedMod, false);
        transform.position += ComputeCollisionMove(Vector3.down * (3f * Time.deltaTime));
        HandleTurning(1f, false, 0f, false, 0.25f);
        
        
        var animatorSpeedMod = Mathf.Lerp(GroundMoveMinimumAnimatorSpeedMod, GroundMoveMaximumAnimatorSpeedMod, ComputeMomentumWeight()) *
                       (_isSurging ? 1.5f : 1f); // same as SetAnimatorSpeedMod() but whtout boost
        Animator.SetFloat("SpeedMod", animatorSpeedMod);
        
        if (UpdateLedgePosition(FaceHighLedgeHeight))
        {
            MoveYOntoLedge(0f, -1f);
        }
        else if (UpdateLedgePosition(FaceLedgeHeight))
        {
            MoveYOntoLedge(0f, -1f);
        }    
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
                if (Machine.IsInState(PlayerFsmState.SlowVaultFinish) && TimeInCurrentState() < 0.1f) return false;
                return PlayerManaManager.Singleton.GetCurrentAvailableMana() >= 1 && SaveSystem.GetTrick("Tinsica");
            });
        
        Machine.Configure(PlayerFsmState.Tinsica)
            .SubstateOf(GravityFsmState.Grounded)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.TinsicaJump, _ =>
            {
                return TimeInCurrentState() > 0.4 * ComputeTiniscaDurationMod() && PlayerManaManager.Singleton.GetCurrentAvailableMana() >= 1;;
            }, 2)
            .PermitIf(PlayerFsmTrigger.Jump, PlayerFsmState.Jump, _ =>
            {
                return TimeInCurrentState() > 0.5 * ComputeTiniscaDurationMod()  && PlayerManaManager.Singleton.GetCurrentAvailableMana() < 1;
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
        var comboMultiplier = GetCurrentSurgeSpeedMultiplier();
        var boostMultiplier = GetCurrentBoostSpeedMultiplier();
        var miscMultiplier = GetCurrentMiscSpeedMultiplier();
        
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

    public void AcquireTrick(string trick)
    {
        SaveSystem.WriteTrick(trick);
        OnTrickAcquired?.Invoke(trick);
    }
}


public class TrickRegistration
{
    public string displayName;
    public string description;
    public int cost;
    public string lore;
    
    public string useInput;
    public string useClause;
}

public static class TrickRegistry
{
    public static readonly Dictionary<string, TrickRegistration> TrickRegistrations;
    public const string TrickColor = "D0C4FF";
    
    static TrickRegistry()
    {
        TrickRegistrations = new Dictionary<string, TrickRegistration>();
        
        TrickRegistrations.Add("Tinsica", new TrickRegistration()
        {
            displayName = "Tinsica",
            description = "A fast front cartwheel that crosses gaps and mounts ledges.",
            lore = "By expelling energy into their palms, Lotus Monks balance qi evenly across their bodies.",
            cost = 1,
            useInput = "Trick",
            useClause = "while on the ground"
        });
        
        TrickRegistrations.Add("TinsicaJump", new TrickRegistration()
        {
            displayName = "Tinsica Jump",
            description = "A floating frontflip that travels far.",
            lore = "Motion is the plucking of a string.",
            cost = 1,
            useInput = "Jump",
            useClause = "while in a Tinsica"
        });
        
        
        
    }
}