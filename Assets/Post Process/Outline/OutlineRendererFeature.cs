using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class OutlineSettings
{
    public LayerMask outlineLayer = 0;
    public Material outlineMaterial = null;
    [Range(0f, 20f)] public float thickness = 1f;
    [Range(0f, 1f)] public float depthMin = 0f;
    [Range(0f, 1f)] public float depthMax = 1f;
    public Color outlineColor = Color.black;
}

public class OutlineRendererFeature : ScriptableRendererFeature
{
    class OutlinePass : ScriptableRenderPass
    {
        public OutlineSettings settings;
        private RTHandle tempTexture;

        public OutlinePass(OutlineSettings settings)
        {
            this.settings = settings;
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (tempTexture == null)
            {
                RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
                desc.depthBufferBits = 0;
                tempTexture = RTHandles.Alloc(desc, name: "_OutlineTempTexture");
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            Camera camera = renderingData.cameraData.camera;
            if (camera.cameraType == CameraType.SceneView)
                return;

            if (settings.outlineMaterial == null)
            {
                Debug.LogError("OutlineRendererFeature: Missing outline material!");
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("Outline Pass");

            // Set material parameters
            settings.outlineMaterial.SetFloat("_Thickness", settings.thickness);
            settings.outlineMaterial.SetFloat("_MinDepth", settings.depthMin);
            settings.outlineMaterial.SetFloat("_MaxDepth", settings.depthMax);
            settings.outlineMaterial.SetColor("_OutlineColor", settings.outlineColor);

            // --- Render outline objects into tempTexture ---
            cmd.SetRenderTarget(tempTexture);
            cmd.ClearRenderTarget(true, true, Color.clear);

            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            FilteringSettings filtering = new FilteringSettings(RenderQueueRange.all, settings.outlineLayer);
            DrawingSettings drawSettings = CreateDrawingSettings(
                new ShaderTagId("UniversalForward"), ref renderingData, SortingCriteria.CommonOpaque
            );
            drawSettings.overrideMaterial = settings.outlineMaterial;
            drawSettings.overrideMaterialPassIndex = 0;

            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filtering);

            // --- Composite tempTexture over camera color target ---
            cmd.Blit(tempTexture, renderingData.cameraData.renderer.cameraColorTargetHandle);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (tempTexture != null)
            {
                RTHandles.Release(tempTexture);
                tempTexture = null;
            }
        }
    }

    public OutlineSettings settings = new OutlineSettings();
    private OutlinePass outlinePass;

    public override void Create()
    {
        if (settings.outlineMaterial == null)
            Debug.LogError("OutlineRendererFeature: Missing outline material!");

        outlinePass = new OutlinePass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (outlinePass != null)
            renderer.EnqueuePass(outlinePass);
    }
}
