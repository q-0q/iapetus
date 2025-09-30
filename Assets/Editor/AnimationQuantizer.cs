using UnityEditor;
using UnityEngine;

namespace SomeNamespace
{
    /// <summary>
    ///
    /// </summary>
    public class AnimationQuantizer : AssetPostprocessor
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="root"></param>
        /// <param name="clip"></param>
        private void OnPostprocessAnimation(GameObject root, AnimationClip clip)
        {
            if(AnimationQuantizerSettings.Enabled)
            {
                Debug.Log($"Quanitizing animation clip '{clip.name}'");
                var curveBindings = AnimationUtility.GetCurveBindings(clip);
                foreach(var curveBinding in curveBindings)
                {
                    var curve = AnimationUtility.GetEditorCurve(clip, curveBinding);
                    for(int i = 0; i < curve.keys.Length; i++)
                    {
                        //probably not worth doing ALL of these but hey, let's not take any chances at this point
                        curve.keys[i].inWeight = 1;
                        curve.keys[i].outWeight = 1;
                        curve.keys[i].inTangent = 1;
                        curve.keys[i].outTangent = 1;
                        curve.keys[i].weightedMode = WeightedMode.Both;
                        AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                        AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                    }
                    AnimationUtility.SetEditorCurve(clip, curveBinding, curve);
                }
            }
        }
    }


    /// <summary>
    ///
    /// </summary>
    public static class AnimationQuantizerSettings
    {
        public static bool Enabled = true;
    }
}