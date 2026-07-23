using UnityEngine;

public class SceneAmbientParticles : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        if (PlayerFsm.Singleton == null) return;

        transform.SetParent(PlayerFsm.Singleton.transform);
        transform.localPosition = Vector3.zero;

        ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }
    }
}