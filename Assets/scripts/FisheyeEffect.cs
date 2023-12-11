using System.Collections;
using UnityEngine;
using System.IO;

public class FisheyeEffect : MonoBehaviour
{
    public Shader _fisheyeShader;
    private Material _fisheyeMaterial;
    private CameraManager cameraManager;
    public string _saveFileName = "fisheye_rgb.png";

    void Start()
    {
        cameraManager = GetComponentInParent<CameraManager>();
        GetComponent<Camera>().fieldOfView = cameraManager.fov;
        _fisheyeMaterial = new Material(_fisheyeShader);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveRenderedTexture();
        }
        Vector2 focalLength = new Vector2(cameraManager.fx, cameraManager.fy);
        _fisheyeMaterial.SetVector("_focalLength", focalLength);
        Vector4 distortion = new Vector4(cameraManager.k1, cameraManager.k2, cameraManager.k3, cameraManager.k4);
        _fisheyeMaterial.SetVector("_distortion", distortion);
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
        string output_path = Path.Combine(cameraManager.output_dir, _saveFileName);
        File.WriteAllBytes(output_path, bytes);

        DestroyImmediate(texture);
        RenderTexture.active = currentRT;
        renderTexture.Release();

        Debug.Log("Saved Rendered Image to " + output_path);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _fisheyeMaterial);
    }

}
