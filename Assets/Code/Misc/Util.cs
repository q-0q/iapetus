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
            if(hit.collider is MeshCollider) {
                var collider = hit.collider as MeshCollider;
                var mesh = collider.sharedMesh;
                var tris = mesh.triangles;
                var verts = mesh.vertices;

                var v0 = verts[tris[hit.triangleIndex * 3]];
                var v1 = verts[tris[hit.triangleIndex * 3 + 1]];
                var v2 = verts[tris[hit.triangleIndex * 3 + 2]];

                var n = Vector3.Cross(v1 - v0, v2 - v1).normalized;

                return hit.transform.TransformDirection(n);
            }
            else {
                RaycastHit result;
                var ray = new Ray(hit.point - dir.normalized * 0.5f, dir);
                hit.collider.Raycast(ray, out result, 1f);
                return result.normal;
            }
        }
    }
}