using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public int width = 512;
    public int height = 512;
    public float fx = 256.0f;
    public float fy = 256.0f;
    public float fov = 60.0f;
    public float k1 = 0.0f;
    public float k2 = 0.0f;
    public float k3 = 0.0f;
    public float k4 = 0.0f;
    public string output_dir = "outputs";
}
