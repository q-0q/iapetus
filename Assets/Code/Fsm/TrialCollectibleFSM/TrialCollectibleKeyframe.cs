using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Fsm.TrialCollectibleFSM
{
    public class TrialCollectibleKeyframe : MonoBehaviour
    {
        public float duration = 5f;
        public CameraBehaviorZone attachedCameraBehaviorZone;
        
        // public CameraZoneDisableMode cameraZoneDisableMode;
        // private int _attachedCameraBehaciorZoneBasePriority;
        // public enum CameraZoneDisableMode
        // {
        //     Disable,
        //     RevertPriority
        // }


        private void Start()
        {
            // _attachedCameraBehaciorZoneBasePriority = attachedCameraBehaviorZone.priority;
            DisableCameraZone();
        }

        public void EnableCameraZone()
        {
            if (attachedCameraBehaviorZone == null) return;
            attachedCameraBehaviorZone.enabled = true;
            attachedCameraBehaviorZone.priority = 100;
        }
        
        public void DisableCameraZone()
        {
            if (attachedCameraBehaviorZone == null) return;
            attachedCameraBehaviorZone.enabled = false;}
        }
}