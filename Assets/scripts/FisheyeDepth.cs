using System.Collections;
using UnityEngine;
using System.IO;

public class FisheyeDepth : MonoBehaviour
{
    public Shader _fisheyeDepthShader;
    private Material _fisheyeMaterial;
    private CameraManager cameraManager;
    public string _saveFileName = "fisheye_depth.exr";

    void Start()
    {
        cameraManager = GetComponentInParent<CameraManager>();
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
        GetComponent<Camera>().fieldOfView = cameraManager.fov;
        _fisheyeMaterial = new Material(_fisheyeDepthShader);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveDepthTextureToEXR();
        }
        Vector2 focalLength = new Vector2(cameraManager.fx, cameraManager.fy);
        _fisheyeMaterial.SetVector("_focalLength", focalLength);
        Vector4 distortion = new Vector4(cameraManager.k1, cameraManager.k2, cameraManager.k3, cameraManager.k4);
        _fisheyeMaterial.SetVector("_distortion", distortion);
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

        Debug.Log("Fisheye depth saved to: " + output_path);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _fisheyeMaterial);
    }
}

