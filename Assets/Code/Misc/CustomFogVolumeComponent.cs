using UnityEngine;
using UnityEngine.Rendering;

namespace Code.Misc
{
    [VolumeComponentMenu("CustomFog")]
    public sealed class CustomFogVolumeComponent : VolumeComponent, IUpdatableVolumeComponent
    {

        public ColorParameter Color = new(UnityEngine.Color.white);
        
        public FloatParameter YMin = new(value: -50f);
        public FloatParameter YMax = new(value: 50f);
        public FloatParameter YPower = new(value: 2f);
        
        public FloatParameter DepthMin = new(value: 20);
        public FloatParameter DepthMax = new(value: 100);
        public FloatParameter DepthPower = new(value: 2f);
        
        public FloatParameter NoiseSubtractionAmount = new(value: 50f);
        public FloatParameter NoiseAScale = new(value: 0.01f);
        public Vector3Parameter NoiseAVelocity = new(value: new Vector3(-1, -1, -1));
        public FloatParameter NoiseBScale = new(value: 0.02f);
        public Vector3Parameter NoiseBVelocity = new(value: new Vector3(2, 2, 2));

        public void Update()
        {
            Shader.SetGlobalColor("_CustomFogColor", Color.value);
            
            Shader.SetGlobalFloat("_CustomFogYMin", YMin.value);
            Shader.SetGlobalFloat("_CustomFogYMax", YMax.value);
            Shader.SetGlobalFloat("_CustomFogYPower", YPower.value);
            
            Shader.SetGlobalFloat("_CustomFogDepthMin", DepthMin.value);
            Shader.SetGlobalFloat("_CustomFogDepthMax", DepthMax.value);
            Shader.SetGlobalFloat("_CustomFogDepthPower", DepthPower.value);
            
            Shader.SetGlobalFloat("_CustomFogNoiseSubtractionAmount", NoiseSubtractionAmount.value);
            Shader.SetGlobalFloat("_CustomFogNoiseAScale", NoiseAScale.value);
            Shader.SetGlobalFloat("_CustomFogNoiseBScale", NoiseBScale.value);
            
            Shader.SetGlobalVector("_CustomFogNoiseAVelocity", NoiseAVelocity.value);
            Shader.SetGlobalVector("_CustomFogNoiseBVelocity", NoiseBVelocity.value);
        }
    }
}