using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RemapTextureGenerator : MonoBehaviour
{
    private CameraManager cameraManager;
    public string savePath = "Assets/Resources/Textures/RemapTexture.asset";

    void Start()
    {
        cameraManager = GetComponentInParent<CameraManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Texture2D texture = GenerateFisheyeRemapTexture(cameraManager.width, cameraManager.height);
            SaveTextureAsAsset(texture, savePath);
        }
    }

    Texture2D GenerateFisheyeRemapTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGFloat, false);
        float radian = cameraManager.fov/2.0f * Mathf.PI / 180.0f;
        float a = Mathf.Tan(radian);
        float fx = cameraManager.width / (2.0f * a);
        float fy = cameraManager.height / (2.0f * a);
        float cx = cameraManager.width / 2.0f;
        float cy = cameraManager.height / 2.0f;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float x = (j - width/2.0f) / (width/2.0f);
                float y = (i - height/2.0f) / (height/2.0f);
                float r = Mathf.Sqrt(x * x + y * y);

                if (r <= 1.0f)
                {
                    Vector2 uv = inverseFisheyeTransform(new Vector2(x, y), radian);
                    float px = fx * uv.x + cx;
                    float py = fy * uv.y + cy;
                    texture.SetPixel(j, i, new Color(px / width, py / height, 0.0f, 0.0f));
                }
                else
                {
                    texture.SetPixel(j, i, new Color(0, 0, 0, 0));
                }
            }
        }

        texture.Apply();
        return texture;
    }

    Vector2 inverseFisheyeTransform(Vector2 uv, float rad)
    {
        float r = Mathf.Sqrt(uv.x * uv.x + uv.y * uv.y);
        float phi = Mathf.Atan2(uv.y, uv.x);
        float x = Mathf.Tan(r * rad) * Mathf.Cos(phi);
        float y = Mathf.Tan(r * rad) * Mathf.Sin(phi);
        return new Vector2(x, y);
    }

#if UNITY_EDITOR
    void SaveTextureAsAsset(Texture2D texture, string path)
    {
        AssetDatabase.CreateAsset(texture, path);
        AssetDatabase.SaveAssets();
        Debug.Log("Texture saved to: " + path);
    }
#endif
}

