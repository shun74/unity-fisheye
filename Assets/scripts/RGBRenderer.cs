using System.Collections;
using UnityEngine;
using System.IO;

public class RGBRenderer : MonoBehaviour
{
    private CameraManager cameraManager;
    public string _saveFileName = "rgb.png";

    void Start()
    {
        cameraManager = GetComponentInParent<CameraManager>();
        GetComponent<Camera>().fieldOfView = cameraManager.fov;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            SaveScreenshot();
        }
    }
    
    public void SaveScreenshot()
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

        Debug.Log("RGB saved to: " + output_path);
    }
}
