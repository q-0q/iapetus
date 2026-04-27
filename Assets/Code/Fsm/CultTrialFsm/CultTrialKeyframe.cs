
using System;
using System.Collections;
using Code.Misc;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.WSA;

namespace Code.Fsm.TrialCollectibleFSM
{
    public class CultTrialKeyframe : MonoBehaviour
    {
        private CultTrialFsm _ownerFsm;
        private Renderer _renderer;
        private TriggerProxy _triggerProxy;
        public bool isFinalKeyframe;
        private ParticleSystem _particleSystem;

        private void Awake()
        {
            _ownerFsm = transform.parent.parent.GetComponent<CultTrialFsm>();
            _renderer = GetComponentInChildren<Renderer>();
            _triggerProxy = GetComponentInChildren<TriggerProxy>();
            _renderer.enabled = false;
            _triggerProxy.GetComponent<Collider>().enabled = false;
            _particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        private void OnEnable()
        {
            CultTrialManager.OnTrialActive += OnTrialActive;
            _triggerProxy.OnTriggerProxyStay += OnTrigger;
            CultTrialFsm.OnTrialInactive += OnTrialInactive;
        }

        private void OnTrigger(Collider obj)
        {
            PlayerFsm.Singleton.OnCultTrialBoostTrigger();
            CultTrialManager.Singleton.ReplenishCurseDuration();
            if (isFinalKeyframe) _ownerFsm.Machine.Fire(CultTrialFsm.CultTrialFsmTrigger.FinalKeyframeTriggered);
            Deactivate();
        }

        private void OnDisable()
        {
            CultTrialManager.OnTrialActive -= OnTrialActive;
            CultTrialFsm.OnTrialInactive -= OnTrialInactive;
            _triggerProxy.OnTriggerProxyStay -= OnTrigger;
        }

        private void OnTrialActive(CultTrialFsm fsm)
        {
            if (fsm != _ownerFsm) return;
            Activate();
        }
        
        private void OnTrialInactive(CultTrialFsm fsm)
        {
            if (fsm != _ownerFsm) return;
            Deactivate();
        }

        private void Deactivate()
        {
            
            _triggerProxy.GetComponent<Collider>().enabled = false;
            StartCoroutine(RendererClipCoroutine());
            _particleSystem.Stop();
            
            IEnumerator RendererClipCoroutine()
            {
                var t = 0f;
                var d = 0.5f;
                while (t < d)
                {
                    _renderer.material.SetFloat("_Clip", Mathf.Lerp(0.2f, 1f, Util.SmoothLerp01(t / d)));
                    t += Time.deltaTime;
                    yield return null;
                }
                _renderer.enabled = false;
            }
        }
        
        private void Activate()
        {
            _renderer.enabled = true;
            _triggerProxy.GetComponent<Collider>().enabled = true;
            StartCoroutine(RendererClipCoroutine());
            _particleSystem.Play();
            
            _renderer.material.SetFloat("_IsFinal", isFinalKeyframe ? 1f : 0f);
            IEnumerator RendererClipCoroutine()
            {
                var t = 0f;
                var d = 0.5f;
                while (t < d)
                {
                    _renderer.material.SetFloat("_Clip", Mathf.Lerp(1f, 0.2f, Util.SmoothLerp01(t / d)));
                    t += Time.deltaTime;
                    yield return null;
                }
            }
        }


    }
}