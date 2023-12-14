using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public int width = 1024;
    public int height = 1024;
    public float fx = 256.0f;
    public float fy = 256.0f;
    public float fov = 165.0f;
    public string output_dir = "outputs";
    public float moveSpeed = 0.2f; // 移動速度

    void Update()
    {
        // 移動入力を取得
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;

        // カメラマネージャーの位置を更新
        transform.Translate(moveX, 0, moveZ);
    }
}
