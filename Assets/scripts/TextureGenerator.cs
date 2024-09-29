using System.Collections;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TextureGenerator : MonoBehaviour
{
    private CameraManager _cameraManager;
    public string saveDir = "Assets/Resources";

    void Start()
    {
        _cameraManager = GetComponentInParent<CameraManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Texture2D fisheye_150_texture = GenerateFisheyeRemapTexture(_cameraManager.width * 4, _cameraManager.height * 4, 150);
            Texture2D fisheye_165_texture = GenerateFisheyeRemapTexture(_cameraManager.width * 4, _cameraManager.height * 4, 165);
            SaveTextureAsAsset(fisheye_150_texture, Path.Combine(saveDir, "fisheye_remap_150.asset"));
            SaveTextureAsAsset(fisheye_165_texture, Path.Combine(saveDir, "fisheye_remap_165.asset"));
        }
    }

    Texture2D GenerateFisheyeRemapTexture(int width, int height, float fov)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGFloat, false);
        float radian = fov / 2.0f * Mathf.PI / 180.0f;
        float a = Mathf.Tan(radian);
        float fx = width / (2.0f * a);
        float fy = height / (2.0f * a);
        float cx = width / 2.0f;
        float cy = height / 2.0f;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float x = (j - width / 2.0f) / (width / 2.0f);
                float y = (i - height / 2.0f) / (height / 2.0f);
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

