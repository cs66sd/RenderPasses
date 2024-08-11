using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;



public class RedTintRenderPass : ScriptableRenderPass
{
    private Material material;
    private RenderTextureDescriptor textureDescriptor;
    private RTHandle textureHandle;
    
    public RedTintRenderPass(Material material)
    {
        this.material = material;
        textureDescriptor = new RenderTextureDescriptor(Screen.width, Screen.height,
            RenderTextureFormat.Default, 0);
    }
    


    public override  void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        textureDescriptor.width = cameraTextureDescriptor.width;
        textureDescriptor.height = cameraTextureDescriptor.height;

        RenderingUtils.ReAllocateIfNeeded(ref textureHandle, textureDescriptor);
    }
    
    public override void Execute(ScriptableRenderContext context,
        ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get();
        RTHandle cameraTargetHandle = 
            renderingData.cameraData.renderer.cameraColorTargetHandle;
        
        Blit(cmd, cameraTargetHandle, textureHandle, material, 0);
        Blit(cmd, textureHandle, cameraTargetHandle, material, 1);
     
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Dispose()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlaying)
        {
            Object.Destroy(material);
        }
        else
        {
            Object.DestroyImmediate(material);
        }
#else
            Object.Destroy(material);
#endif

        if (textureHandle != null) textureHandle.Release();
    }
}
