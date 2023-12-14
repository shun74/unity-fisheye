using System.Collections;
using System.IO;
using UnityEngine;

public class Fisheye : MonoBehaviour
{
    public Shader _shader;
    private Material _material;
    private CameraManager cameraManager;
    public string _saveFileName = "fisheye.png";

    void Start()
    {
        cameraManager = GetComponentInParent<CameraManager>();
        GetComponent<Camera>().fieldOfView = cameraManager.fov;
        _material = new Material(_shader);
        LoadRemapTexture();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            LoadRemapTexture();
            SaveRenderedTexture();
        }
    }

    private void LoadRemapTexture()
    {
        Texture2D remapTexture = Resources.Load<Texture2D>("Textures/RemapTexture");
        if (remapTexture != null)
        {
            _material.SetTexture("_RemapTex", remapTexture);
        }
        else
        {
            Debug.LogError("Remap texture not found.");
        }
    }

    public void SaveRenderedTexture()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = new RenderTexture(cameraManager.width, cameraManager.height, 24, RenderTextureFormat.ARGB32);
        
        GetComponent<Camera>().targetTexture = renderTexture;
        GetComponent<Camera>().Render();
        GetComponent<Camera>().targetTexture = null;

        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string outputPath = Path.Combine(cameraManager.output_dir, _saveFileName);
        File.WriteAllBytes(outputPath, bytes);

        DestroyImmediate(texture);
        RenderTexture.active = currentRT;
        renderTexture.Release();

        Debug.Log("Saved Rendered Image to " + outputPath);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material);
    }
}
