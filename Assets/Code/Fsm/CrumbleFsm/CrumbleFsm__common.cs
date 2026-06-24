using System.Collections.Generic;
using Code.TriggerParams;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public partial class CrumbleFsm
{
    private Collider _collider;
    private Renderer _renderer;
    private Vector3 _initLocalPosition;

    private ParticleSystem _reformParticleSystem;
    private ParticleSystem _crumbleParticleSystem;
    private ParticleSystem _breakParticleSystem;

    private const string eventPath1 = "event:/Crumble1";
    private const string eventPath2 = "event:/Crumble2";
    private const string eventPath3 = "event:/Crumble3";
    private const string eventPath4 = "event:/Crumble4";

    private void WorldspaceShake(float duration, float strength)
    {
        transform.DOComplete();
        
        // 1. Store the starting world position
        Vector3 origin = transform.position;

        // 2. Shake a virtual vector from zero to the desired strength
        DOTween.Shake(() => Vector3.zero, v => 
        {
            // 3. Apply the offset to the original world origin
            transform.position = origin + v;
        }, duration, strength, 20);
    }

    private void DoIdleJump()
    {
        if (PlayerFsm.Singleton.parentTransform != transform) Machine.Jump(CrumbleFsmState.Idle);
    }
}