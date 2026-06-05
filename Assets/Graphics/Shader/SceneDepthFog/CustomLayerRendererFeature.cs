using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

public class CustomLayerRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class FeatureSettings
    {
        public LayerMask layerMask;
        public Material overrideMaterial;
        public string targetTextureName = "_CustomLayerTexture";
        public GraphicsFormat textureFormat = GraphicsFormat.R32_SFloat; 
        public float emptyAreaValue = 10000.0f; 
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public FeatureSettings settings = new FeatureSettings();
    private CustomLayerPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomLayerPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.overrideMaterial == null) return;
        
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent;
        renderer.EnqueuePass(m_ScriptablePass);
    }

    class CustomLayerPass : ScriptableRenderPass
    {
        private FeatureSettings settings;
        private List<ShaderTagId> m_ShaderTagIds = new List<ShaderTagId>();
        private int m_TextureNameID;

        private class PassData
        {
            public RendererListHandle rendererListHandle;
        }

        public CustomLayerPass(FeatureSettings settings)
        {
            this.settings = settings;
            m_TextureNameID = Shader.PropertyToID(settings.targetTextureName);

            m_ShaderTagIds.Add(new ShaderTagId("SRPDefaultUnlit"));
            m_ShaderTagIds.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIds.Add(new ShaderTagId("UniversalForwardOnly"));
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();

            RenderTextureDescriptor cameraDesc = cameraData.cameraTargetDescriptor;

            // Define the high value clear color for all channels
            Color customClearColor = new Color(settings.emptyAreaValue, settings.emptyAreaValue, settings.emptyAreaValue, settings.emptyAreaValue);

            // 1. FIXED: Set the clearing behavior directly on the Texture Descriptor
            TextureDesc customTextureDesc = new TextureDesc(cameraDesc.width, cameraDesc.height)
            {
                format = settings.textureFormat,
                msaaSamples = MSAASamples.None,
                name = settings.targetTextureName,
                clearBuffer = true,            // Tells Render Graph to clear this buffer
                clearColor = customClearColor  // Applies your custom high value
            };
            TextureHandle customTextureHandle = renderGraph.CreateTexture(customTextureDesc);

            // 2. Configure the companion hardware depth texture
            TextureDesc depthTextureDesc = new TextureDesc(cameraDesc.width, cameraDesc.height)
            {
                format = GraphicsFormat.D32_SFloat,
                msaaSamples = MSAASamples.None,
                name = settings.targetTextureName + "_Depth",
                clearBuffer = true             // Automatically clears the hardware depth to 1.0f
            };
            TextureHandle depthTextureHandle = renderGraph.CreateTexture(depthTextureDesc);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Custom Layer Buffer Pass", out var passData))
            {
                SortingCriteria sortingCriteria = cameraData.defaultOpaqueSortFlags;
                DrawingSettings drawingSettings = RenderingUtils.CreateDrawingSettings(
                    m_ShaderTagIds, renderingData, cameraData, lightData, sortingCriteria
                );
                drawingSettings.overrideMaterial = settings.overrideMaterial;
                drawingSettings.overrideMaterialPassIndex = 0;

                FilteringSettings filteringSettings = new FilteringSettings(RenderQueueRange.all, settings.layerMask);

                var rendererListParams = new RendererListParams(renderingData.cullResults, drawingSettings, filteringSettings);
                RendererListHandle listHandle = renderGraph.CreateRendererList(rendererListParams);
                
                passData.rendererListHandle = listHandle;
                builder.UseRendererList(listHandle);

                // Set targets. Render Graph honors the clear flags set up in steps 1 and 2 automatically.
                builder.SetRenderAttachment(customTextureHandle, 0);
                builder.SetRenderAttachmentDepth(depthTextureHandle, AccessFlags.Write);

                builder.SetGlobalTextureAfterPass(customTextureHandle, m_TextureNameID);

                builder.SetRenderFunc(static (PassData data, RasterGraphContext context) =>
                {
                    if (data.rendererListHandle.IsValid())
                    {
                        context.cmd.DrawRendererList(data.rendererListHandle);
                    }
                });
            }
        }
    }
}