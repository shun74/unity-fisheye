using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public int width = 1024;
    public int height = 1024;
    public string dataset_dir = "D:/Dataset/fisheye_depth/";
    public string scene_name = "scene_00";

    public int fps = 30;
    public int captureLimit = 10000;
    public int count = 0;
    public bool record = false;
    private float captureInterval;
    private float timeSinceLastCapture = 0f;

    private List<ICapture> cameraCaptures;

    void Start()
    {
        cameraCaptures = new List<ICapture>();
        foreach (var captureComponent in GetComponentsInChildren<ICapture>())
        {
            cameraCaptures.Add(captureComponent);
        }
        captureInterval = 1f / fps;
    }

    void Update()
    {
        if (!record) return;
        timeSinceLastCapture += Time.deltaTime;

        if (timeSinceLastCapture >= captureInterval)
        {
            if (count < captureLimit)
            {
                foreach (var capture in cameraCaptures)
                {
                    capture.Capture();
                }
                count++;
                timeSinceLastCapture -= captureInterval;
            }
        }
    }

}
