using UnityEngine;

namespace Code.Misc
{
    public static class Util
    {
        /// <summary>
        /// Calculates the surface normal for CapsuleCast and SphereCast
        /// </summary>
        /// <param name="hit">original hit</param>
        /// <param name="dir">original direction of the raycast</param>
        /// <returns>correct normal</returns>
        /// <remarks> https://discussions.unity.com/t/554303 </remarks>
        public static Vector3 GetCorrectNormalForSphere(this RaycastHit hit, Vector3 dir) {
            RaycastHit result;
            var ray = new Ray(hit.point - dir.normalized * 0.5f, dir);
            hit.collider.Raycast(ray, out result, 1f);
            return result.normal;
        }
        
        public static float SmoothLerp01(float t)
        {
            t = Mathf.Clamp01(t);
            return t * t * (3f - 2f * t);
        }
        
        public static Vector3 LerpWithArc(Vector3 start, Vector3 end, float t, float height)
        {
            // Clamp t for safety
            t = Mathf.Clamp01(t);

            // Base linear interpolation
            Vector3 position = Vector3.Lerp(start, end, t);

            // Quadratic arc: peaks at t = 0.5, zero at t = 0 and t = 1
            float arc = 4f * height * t * (1f - t);

            // Apply arc in the world-up direction
            position += Vector3.up * arc;

            return position;
        }
        
        public static void ReplaceAnimatorTrigger(Animator animator, string trigger)
        {
            if (trigger == "") return;
            foreach (var t in animator.parameters)
            {
                if (t.type != AnimatorControllerParameterType.Trigger) continue;
                if (t.name == trigger)
                {
                    animator.SetTrigger(t.name);
                }
                else animator.ResetTrigger(t.name);
            }
        }

        public static void InvokeSphereEffect(Vector3 position, Vector3 initialScale, float finalScale, float ageMultiplier, float distanceOffset)
        {
            
            var spherePrefab = Resources.Load("Prefab/Fsm/SphereEffect") as GameObject;
            var spherePosition = position + Vector3.up;
            var sphereObject = Object.Instantiate(spherePrefab, spherePosition,
                Quaternion.identity, null);
            sphereObject.GetComponent<SphereEffect>().SetConfig(initialScale, finalScale, ageMultiplier, distanceOffset);
        }

        public static Transform FindGamePositionById(string id)
        {
            if (id == "") return null;
            
            foreach (var playerGamePosition in UnityEngine.Object.FindObjectsByType<PlayerGamePosition>(FindObjectsSortMode.None))
            {
                if (playerGamePosition.Id == id)
                {
                    return playerGamePosition.transform;
                }
            }

            return null;
        }
    }
}