using System.Collections.Generic;
using Code.TriggerParams;
using UnityEngine;
using UnityEngine.Serialization;

public partial class CrumbleFsm
{
    private Collider _collider;
    private Renderer _renderer;
    private Vector3 _initLocalPosition;

    private ParticleSystem _reformParticleSystem;
}