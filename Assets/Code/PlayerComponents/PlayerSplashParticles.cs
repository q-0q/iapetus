using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSplashParticles : MonoBehaviour
{

    public static event Action<Vector3, float, float> OnPlayerSplashParticleTriggerEnter;
    
    ParticleSystem ps;

    // these lists are used to contain the particles which match
    // the trigger conditions each frame.
    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
    
    private void Awake()
    {
        TryGetComponent(out ps);
    }

    void OnParticleTrigger()
    {
        // get the particles which matched the trigger conditions this frame
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        // iterate through the particles which entered the trigger and make them red
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enter[i];
            var vector3 = p.position + transform.position;
            Debug.DrawRay(vector3, Vector3.up, Color.cyan);

            var strength = Random.Range(0.5f, 1f);
            OnPlayerSplashParticleTriggerEnter?.Invoke(vector3, 0.5f * strength, 0.0005f * strength);
        }
    }
}
