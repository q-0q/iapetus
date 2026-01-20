using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Fsm.TrialCollectibleFSM
{
    public class TrialCollectibleKeyframe : MonoBehaviour
    {
        public float duration = 5f;
        public CameraBehaviorZone attachedCameraBehaviorZone;
        public static event Action OnTrialCollectibleCameraZoneUpdated;
        public bool preservePreviousKeyframeCameraZone = false;
        
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
            attachedCameraBehaviorZone.gameObject.SetActive(true);
            attachedCameraBehaviorZone.priority = 100;
            print("enabled " + attachedCameraBehaviorZone.name);
            OnTrialCollectibleCameraZoneUpdated?.Invoke();
        }
        
        public void DisableCameraZone()
        {
            if (attachedCameraBehaviorZone == null) return;
            print("DISABLED");
            attachedCameraBehaviorZone.gameObject.SetActive(false);
        }
    }
}