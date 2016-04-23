using System;
using System.Collections;
using UnityEngine;

public class VRCamera : MonoBehaviour
{
    public float Distance = 0.1f;
    public float Degree = 0f;

    private RenderTexture renderTexture;
    private Texture2D texture;
    public SocketTest socket;

    public Camera _cameraLeft;
    public Camera _cameraRight;
    private float leftCameraX;
    private float leftCameraDegree;
    private float rightCameraX;
    private float rightCameraDegree;
    private bool dirty = true;

    void Start()
    {
        renderTexture = new RenderTexture(1920 / 4, 1080 / 4, 16);
        texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        _cameraLeft.targetTexture = renderTexture;
        _cameraRight.targetTexture = renderTexture;
    }

    void Update()
    {
        AdjustCameras();
        StartCoroutine(SendScreen());
    }

    private void AdjustCameras()
    {
        leftCameraX = -Distance / 2;
        rightCameraX = Distance / 2;
        leftCameraDegree = Degree / 2;
        rightCameraDegree = -Degree / 2;
        const double tolerance = 0.000000000000000001;
        // Adjust rotations
        if (Math.Abs(leftCameraDegree - _cameraLeft.transform.localRotation.y) > tolerance)
            _cameraLeft.transform.localRotation = new Quaternion(_cameraLeft.transform.localRotation.x, leftCameraDegree, _cameraLeft.transform.localRotation.z, _cameraLeft.transform.localRotation.w);
        if (Math.Abs(rightCameraDegree - _cameraRight.transform.localRotation.y) > tolerance)
            _cameraRight.transform.localRotation = new Quaternion(_cameraRight.transform.localRotation.x, rightCameraDegree, _cameraRight.transform.localRotation.z, _cameraRight.transform.localRotation.w);
        // Adjust x positions of cameras
        if (Math.Abs(leftCameraX - _cameraLeft.transform.localPosition.x) > tolerance)
            _cameraLeft.transform.localPosition = new Vector3(leftCameraX, _cameraLeft.transform.localPosition.y, _cameraLeft.transform.localPosition.z);
        if (Math.Abs(rightCameraX - _cameraRight.transform.localPosition.x) > tolerance)
            _cameraRight.transform.localPosition = new Vector3(rightCameraX, _cameraRight.transform.localPosition.y, _cameraRight.transform.localPosition.z);
    }

    IEnumerator SendScreen()
    {
        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        if (socket != null)
        {

            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();

            // Encode texture into PNG
            byte[] bytes = texture.EncodeToJPG();

            // For testing purposes, also write to a file in the project folder		
            //File.WriteAllBytes("C:/Users/Pierfrancesco/Desktop/tesi/trash/unity/SavedScreen" + i + ".jpg", bytes);
            //i++;

            int lenghtBytes = bytes.Length;

            socket.sendData(lenghtBytes);
            socket.sendData(bytes);
        }
    }
}