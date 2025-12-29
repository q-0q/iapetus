using System;
using UnityEngine;

namespace Code.PlayerComponents
{
    public class PlayerFootTracker : MonoBehaviour
    {
        private Vector3 _previousPosition;
        private const float YPosTriggerThreshold = -0.7f;
        public static event Action OnPlayerFootstep;

        private void Start()
        {
            _previousPosition = transform.position;
        }

        private void Update()
        {
            var playerPosY = PlayerFsm.Singleton.transform.position.y;
            var yDelta = playerPosY - transform.position.y;
            var prevYDelta = playerPosY - _previousPosition.y;
            // print(yDelta +  ", " + prevYDelta);
            if (yDelta < YPosTriggerThreshold &&
                prevYDelta >= YPosTriggerThreshold)
            {
                print("wahoo!");
                OnPlayerFootstep?.Invoke();
            }
            _previousPosition = transform.position;
        }
        
    }
}