using System;
using UnityEngine;

/// <summary>
/// Virtual reality camera. Uses two cameras, for right and left eyes.
/// </summary>
public class VRCamera : MonoBehaviour
{
    // texture on which the two cameras render
    private RenderTexture renderTexture;
    // texture used to capture the screen
    private Texture2D texture;

    // camera resolution
    public int textureWidth = 1920;
    public int textureHeight = 1080;

    public Camera _cameraLeft;
    public Camera _cameraRight;

    // "lenses" (cameras) regulation parameters, adjust these as you would adjust the lenses on a real VR headset
    public float Distance = 0f;
    public float Degree = 0f;

    private float leftCameraX;
    private float leftCameraDegree;
    private float rightCameraX;
    private float rightCameraDegree;

    // scale the resolution of the camera, is needed for performance reasons. Rendering at 1080p is often too heavy, causing fps problems.
    public int imageScaleFactor = 1;

    void Start()
    {
        renderTexture = new RenderTexture(textureWidth/imageScaleFactor, textureHeight/imageScaleFactor, 16);
        texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // both cameras will render on this texture
        _cameraLeft.targetTexture = renderTexture;
        _cameraRight.targetTexture = renderTexture;
    }

    /// <summary>
    /// Captures the image in the currently active render texture and stores it in a Texture2D. Then encodes the Texture2D data as JPG.
    /// This method is super performance intensive and is definitely not a good solution.
    /// </summary>
    internal byte[] GetImage()
    {
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToJPG();

        return bytes;
    }

    internal void Destroy()
    {
        Destroy(gameObject);
        RenderTexture.active = null;
    }

    void Update()
    {
        AdjustCameras();

        /*
        a bad attempt to quality control :)

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        fps = 1 / deltaTime;

        // check fps every 1 second
        if (Time.time - lastFrameRateCheckTime > 2)
        {
            // if fps < 55, reduce image quality
            if (fps < 55)
            {
                imageScaleFactor++;

                renderTexture = new RenderTexture(textureWidth / imageScaleFactor, textureHeight / imageScaleFactor, 16);
                texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

                _cameraLeft.targetTexture = renderTexture;
                _cameraRight.targetTexture = renderTexture;
            }

            lastFrameRateCheckTime = Time.time;
        }
        */
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
}