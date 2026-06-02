using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class VolumeDepthPrePassFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public LayerMask volumeLayer;
        public Material depthOverrideMaterial;
    }

    public PassSettings settings = new PassSettings();
    private VolumeDepthPass m_DepthPass;

    public override void Create()
    {
        m_DepthPass = new VolumeDepthPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.depthOverrideMaterial == null) return;
        renderer.EnqueuePass(m_DepthPass);
    }

    class VolumeDepthPass : ScriptableRenderPass
    {
        private PassSettings settings;
        private readonly List<ShaderTagId> m_ShaderTagIds = new List<ShaderTagId>();
        private readonly int m_VolumeDepthTexID = Shader.PropertyToID("_VolumeDepthTexture");

        public VolumeDepthPass(PassSettings settings)
        {
            this.settings = settings;
            this.renderPassEvent = settings.renderPassEvent;

            // Target classic URP geometry passes
            m_ShaderTagIds.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIds.Add(new ShaderTagId("UniversalForwardOnly"));
            m_ShaderTagIds.Add(new ShaderTagId("LightweightForward"));
        }

        // Render Graph uses a dedicated pass data container to handle GPU resources safely
        private class PassData
        {
            public RendererListHandle rendererListHandle;
        }

        
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
{
    if (settings.depthOverrideMaterial == null) return;

    // Retrieve modern URP frame structures
    UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
    UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
    UniversalResourceData resourceData = frameData.Get<UniversalResourceData>(); // Added to access scene textures
    
    // Build a descriptor for our custom render texture
    RenderTextureDescriptor desc = cameraData.cameraTargetDescriptor;
    desc.colorFormat = RenderTextureFormat.RHalf; 
    desc.depthBufferBits = 0; // FIX 1: Set to 0. This is a color texture storing depth data.
    desc.msaaSamples = 1;

    // Use URP's built-in helper to register a descriptor-based texture into the Render Graph
    TextureHandle depthTex = UniversalRenderer.CreateRenderGraphTexture(renderGraph, desc, "_VolumeDepthTexture", false);

    using (var builder = renderGraph.AddRasterRenderPass<PassData>("Volume Depth Pass", out var passData))
    {
        // Set our newly created texture as the active color render target
        builder.SetRenderAttachment(depthTex, 0);
        
        // FIX 2: Bind the camera's active depth buffer for Z-testing, but as Read-Only so we don't overwrite it
        if (resourceData.activeDepthTexture.IsValid())
        {
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Read);
        }
        
        // Configure draw rules and replacement material
        SortingSettings sortingSettings = new SortingSettings(cameraData.camera);
        DrawingSettings drawingSettings = new DrawingSettings(m_ShaderTagIds[0], sortingSettings)
        {
            overrideMaterial = settings.depthOverrideMaterial
        };
        for (int i = 1; i < m_ShaderTagIds.Count; i++)
        {
            drawingSettings.SetShaderPassName(i, m_ShaderTagIds[i]);
        }

        FilteringSettings filterSettings = new FilteringSettings(RenderQueueRange.all, settings.volumeLayer);

        // Initialize the list of specific renderers on our targeted layer
        var rendererListParameters = new RendererListParams(renderingData.cullResults, drawingSettings, filterSettings);
        passData.rendererListHandle = renderGraph.CreateRendererList(rendererListParameters);
        
        builder.UseRendererList(passData.rendererListHandle);
        
        // Expose this texture globally so subsequent shaders/materials can read it
        builder.SetGlobalTextureAfterPass(depthTex, m_VolumeDepthTexID);

        // Define the precise rendering execution block
        builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
        {
            // FIX 3: Only clear the Color flag. Do NOT clear the Depth flag because it belongs to the main scene buffer!
            context.cmd.ClearRenderTarget(RTClearFlags.Color, new Color(99999f, 99999f, 99999f, 99999f), 1.0f, 0);
            
            // Issue the draw call for the renderers
            context.cmd.DrawRendererList(data.rendererListHandle);
        });
    }
}
    }
}