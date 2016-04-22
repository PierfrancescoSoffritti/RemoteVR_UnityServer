using UnityEngine;
using System.Collections;

/**
* attach this script to the camera
*/

[RequireComponent(typeof(Camera))]
public class OutputManager : MonoBehaviour
{
    //base effects
    [HideInInspector]
    public Texture2D texture;
    public int textureHeight;
    public int textureWidth;
    public bool capture;
    [Range(0f, 0.04f)]
    public float latency;

    public SocketTest socket;

    public Camera myCamera;

    public Rect prevRect;
    public float asp;
    public GameObject dom;

    public void OnPreRender()
    {        
        asp = myCamera.pixelWidth / myCamera.pixelHeight;
        if (!dom)
        {
            dom = new GameObject("capture", typeof(Camera));
            dom.GetComponent<Camera>().CopyFrom(myCamera);
            dom.GetComponent<Camera>().depth = myCamera.depth - 3;
        }
        if (capture)
        {
            dom.transform.position = myCamera.transform.position;
            dom.transform.rotation = myCamera.transform.rotation;
            //dom.GetComponent<Camera>().aspect = asp;
            dom.GetComponent<Camera>().pixelRect = new Rect(0, 0, textureWidth, textureHeight);
            dom.GetComponent<Camera>().Render();
            texture.ReadPixels(new Rect(myCamera.pixelRect.x, myCamera.pixelRect.y, textureWidth, textureHeight), 0, 0);
            texture.Apply();

            if (socket != null)
            {
                byte[] bytes = texture.EncodeToJPG();

                int lenghtBytes = bytes.Length;

                socket.sendData(lenghtBytes);
                socket.sendData(bytes);

                // For testing purposes, also write to a file in the project folder		
                //File.WriteAllBytes("C:/Users/Pierfrancesco/Desktop/tesi/trash/unity/SavedScreen" + i + ".jpg", bytes);
                //i++;
            }
        }
    }

    public IEnumerator Start()
    {
        myCamera = GetComponent<Camera>();
        textureWidth = 1920/4;
        textureHeight = 1080/4;
        capture = true;
        texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        while (Application.isPlaying)
        {
            capture = true;
            yield return new WaitForSeconds(latency);
        }
    }
}