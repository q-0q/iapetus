using UnityEditor;
using UnityEngine;

namespace SomeNamespace
{
    /// <summary>
    ///
    /// </summary>
    public class AnimationQuantizer : AssetPostprocessor
    {

        private bool ShouldQuantizeClip(AnimationClip clip)
        {
            return true;
            return clip.name == "Armature|DashVault" || clip.name == "Armature|Vault" || clip.name == "Armature|VaultRun";
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="root"></param>
        /// <param name="clip"></param>
        private void OnPostprocessAnimation(GameObject root, AnimationClip clip)
        {
            if(AnimationQuantizerSettings.Enabled && ShouldQuantizeClip(clip))
            {
                Debug.Log($"Quanitizing animation clip '{clip.name}'");
                var curveBindings = AnimationUtility.GetCurveBindings(clip);
                foreach(var curveBinding in curveBindings)
                {
                    var curve = AnimationUtility.GetEditorCurve(clip, curveBinding);
                    for(int i = 0; i < curve.keys.Length; i++)
                    {
                        //probably not worth doing ALL of these but hey, let's not take any chances at this point
                        curve.keys[i].inWeight = 0;
                        curve.keys[i].outWeight = 0;
                        curve.keys[i].inTangent = 0;
                        curve.keys[i].outTangent = 0;
                        curve.keys[i].weightedMode = WeightedMode.None;
                        AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                        AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                    }
                    AnimationUtility.SetEditorCurve(clip, curveBinding, curve);
                }
            }
            else
            {
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