using System.Collections;
using UnityEngine;
using System.IO;

public class DepthRenderer : MonoBehaviour
{
    public Shader _shader;
    private Material _material;
    private CameraManager cameraManager;
    public string _saveFileName = "depth.exr";

    void Start()
    {
        cameraManager = GetComponentInParent<CameraManager>();
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        GetComponent<Camera>().fieldOfView = cameraManager.fov;
        _material = new Material(_shader);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SaveDepthTextureToEXR();
        }
    }

    public void SaveDepthTextureToEXR()
    {
        RenderTexture currentRT = RenderTexture.active;

        RenderTexture depthTexture = new RenderTexture(cameraManager.width, cameraManager.height, 0, RenderTextureFormat.RFloat);

        GetComponent<Camera>().targetTexture = depthTexture;
        GetComponent<Camera>().Render();
        GetComponent<Camera>().targetTexture = null;

        RenderTexture.active = depthTexture;
        Texture2D texture = new Texture2D(depthTexture.width, depthTexture.height, TextureFormat.RFloat, false);
        texture.ReadPixels(new Rect(0, 0, depthTexture.width, depthTexture.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToEXR(Texture2D.EXRFlags.OutputAsFloat);
        string output_path = Path.Combine(cameraManager.output_dir, _saveFileName);
        File.WriteAllBytes(output_path, bytes);

        DestroyImmediate(texture);
        RenderTexture.active = currentRT;
        depthTexture.Release();

        Debug.Log("Depth saved to: " + output_path);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(source, dest, _material);
    }
}
