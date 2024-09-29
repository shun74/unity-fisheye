using System.Collections;
using UnityEngine;
using System.IO;

public class RectDepth : MonoBehaviour, ICapture
{
    public float fov = 120;
    public string camera_dir = "rect_120";
    public Shader _shader;
    private CameraManager _cameraManager;
    private string _outputDir;
    private Material _material;

    void Start()
    {
        _cameraManager = GetComponentInParent<CameraManager>();
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        GetComponent<Camera>().fieldOfView = fov;
        _outputDir = Path.Combine(Path.Combine(Path.Combine(_cameraManager.dataset_dir, camera_dir), _cameraManager.scene_name), "depth");
        System.IO.Directory.CreateDirectory(_outputDir);
        _material = new Material(_shader);
    }

    void Update()
    {
    }

    public void Capture()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = new RenderTexture(_cameraManager.width * 4, _cameraManager.height * 4, 0, RenderTextureFormat.RFloat);

        GetComponent<Camera>().targetTexture = renderTexture;
        GetComponent<Camera>().Render();
        GetComponent<Camera>().targetTexture = null;

        Texture2D resizedTexture = new Texture2D(_cameraManager.width, _cameraManager.height, TextureFormat.RHalf, false);

        RenderTexture resizedRenderTexture = RenderTexture.GetTemporary(resizedTexture.width, resizedTexture.height, 0, RenderTextureFormat.RHalf);
        Graphics.Blit(renderTexture, resizedRenderTexture);
        RenderTexture.active = resizedRenderTexture;

        resizedTexture.ReadPixels(new Rect(0, 0, resizedRenderTexture.width, resizedRenderTexture.height), 0, 0);
        resizedTexture.Apply();

        string save_name = _cameraManager.count.ToString("D4") + ".exr";
        string output_path = Path.Combine(_outputDir, save_name);
        byte[] bytes = resizedTexture.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
        File.WriteAllBytes(output_path, bytes);

        DestroyImmediate(resizedTexture);
        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(resizedRenderTexture);
        renderTexture.Release();
    }

    private void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        Graphics.Blit(source, dest, _material);
    }
}
