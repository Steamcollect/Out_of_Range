using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace GlitchEffect
{
    public class GlitchEffectFeature : ScriptableRendererFeature
    {
        class GlitchEffectPass : ScriptableRenderPass
        {
            private Material material;
            private LayerMask layerMask;

            private List<ShaderTagId> shaderTag = new List<ShaderTagId>();

            static readonly int ScanLineJitterID = Shader.PropertyToID("_ScanLineJitter");
            static readonly int HorizontalShakeID = Shader.PropertyToID("_HorizontalShake");
            static readonly int ColorDriftID = Shader.PropertyToID("_ColorDrift");
            public GlitchEffectPass(Shader shader, LayerMask layerMask)
            {
                if (shader != null) material = CoreUtils.CreateEngineMaterial(shader);
                this.layerMask = layerMask;
            }
            private class PassData
            {
                public RendererListHandle rendererListHandle;
                public TextureHandle texture;
                public Material material;
            }

            private void InitRendererLists(ContextContainer frameData, ref PassData passData, RenderGraph renderGraph)
            {
                UniversalRenderingData universalRenderingData = frameData.Get<UniversalRenderingData>();
                UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                UniversalLightData lightData = frameData.Get<UniversalLightData>();

                var sortFlags = cameraData.defaultOpaqueSortFlags;
                RenderQueueRange renderQueueRange = RenderQueueRange.all;
                FilteringSettings filterSettings = new FilteringSettings(renderQueueRange, layerMask);

                ShaderTagId[] forwardOnlyShaderTagIds = new ShaderTagId[]
                {
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("LightweightForward")
                };

                shaderTag.Clear();

                foreach (ShaderTagId sid in forwardOnlyShaderTagIds)
                    shaderTag.Add(sid);

                DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(shaderTag, universalRenderingData, cameraData, lightData, sortFlags);

                var param = new RendererListParams(universalRenderingData.cullResults, drawSettings, filterSettings);
                passData.rendererListHandle = renderGraph.CreateRendererList(param);
            }

            private void UpdateSettings()
            {
                if (material == null) { Debug.LogError("update settings material null"); return; }
                var _volume = VolumeManager.instance.stack.GetComponent<GlitchEffectVolume>();

                var scanLineJitter = _volume.scanLineJitter.value;
                var horizontalShake = _volume.horizontalShake.value;
                var colorDrift = _volume.colorDrift.value;

                var slThresh = Mathf.Clamp01(1.0f - scanLineJitter * 1.2f);
                var slDisp = 0.002f + Mathf.Pow(scanLineJitter, 3) * 0.05f;
                material.SetVector(ScanLineJitterID, new Vector2(slDisp, slThresh));

                material.SetFloat(HorizontalShakeID, horizontalShake * 0.2f);

                var cd = new Vector2(colorDrift * 0.04f, Time.time * 606.11f);
                material.SetVector(ColorDriftID, cd);
            }

            static void ExecutePass(PassData data, RasterGraphContext context)
            {
                context.cmd.ClearRenderTarget(false, true, Color.clear);
                context.cmd.DrawRendererList(data.rendererListHandle);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var result = frameData.Get<UniversalResourceData>();
                var camData = frameData.Get<UniversalCameraData>();

                if (!camData.postProcessEnabled || camData.isSceneViewCamera) return;
                if (result.isActiveTargetBackBuffer) return;

                var desc = renderGraph.GetTextureDesc(result.activeColorTexture);
                desc.name = "MySpecialRT";
                desc.clearBuffer = true;
                desc.colorFormat = UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_UNorm;
                var offscreen = renderGraph.CreateTexture(desc);

                UpdateSettings();

                using (var pass = renderGraph.AddRasterRenderPass<PassData>("Special Layer Render", out var passData))
                {
                    passData.texture = offscreen;
                    InitRendererLists(frameData, ref passData, renderGraph);

                    pass.UseRendererList(passData.rendererListHandle);
                    pass.SetRenderAttachment(offscreen, 0, AccessFlags.Write);
                    pass.SetRenderAttachmentDepth(result.activeDepthTexture, AccessFlags.Read);

                    pass.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
                }

                RenderGraphUtils.BlitMaterialParameters param = new(offscreen, result.activeColorTexture, material, 0);

                renderGraph.AddBlitPass(param, "Compose Special Layer");
            }
        }
        [SerializeField] private Shader glitchEffectShader;
        [SerializeField] private LayerMask layerMask;

        GlitchEffectPass scriptablePass;

        public override void Create()
        {
            scriptablePass = new GlitchEffectPass(glitchEffectShader, layerMask);
            scriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(scriptablePass);
        }
    }
}