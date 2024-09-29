using System.Collections;
using UnityEngine;
using System.IO;

public class FisheyeDepth : MonoBehaviour, ICapture
{
    public float fov = 150;
    public string camera_dir = "fisheye_150";
    public string texture_name = "fisheye_remap_150";
    public Shader _shader;
    private CameraManager _cameraManager;
    private string _outputDir;
    private Material _material;

    void Start()
    {
        _cameraManager = GetComponentInParent<CameraManager>();
        GetComponent<Camera>().fieldOfView = fov;
        _outputDir = Path.Combine(Path.Combine(Path.Combine(_cameraManager.dataset_dir, camera_dir), _cameraManager.scene_name), "depth");
        System.IO.Directory.CreateDirectory(_outputDir);
        _material = new Material(_shader);
        LoadRemapTexture();
    }

    void Update()
    {
    }

    private void LoadRemapTexture()
    {
        Texture2D remapTexture = Resources.Load<Texture2D>(texture_name);
        if (remapTexture != null)
        {
            _material.SetTexture("_RemapTex", remapTexture);
        }
        else
        {
            Debug.LogError("Remap texture not found.");
        }
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

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material);
    }
}
