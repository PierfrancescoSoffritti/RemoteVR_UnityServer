using UnityEngine;
using System.Collections;

public class ServerIO : MonoBehaviour {

    private SocketTest socket;
    private Texture2D texture;
    private InputManager inputManager;

    void Awake() {
    }

    void Start () {

        // Create a texture the size of the screen, RGB24 format
        int width = Screen.width;
        int height = Screen.height;
        texture = new Texture2D(width, height, TextureFormat.RGB24, false);

        socket = new SocketTest();
        socket.sendData(0);

        inputManager = InputManager.getInstance().init(socket);
    }

	void Update () {
        //camera.transform.LookAt(q.eulerAngles);
        //camera.transform.rotation *= q;

        //cameraQuat = camera.transform.rotation;
        //cameraEuler = camera.transform.rotation.eulerAngles;

        //phoneQuat = inputManager.getQuaternion();
        //phoneEuler = phoneQuat.eulerAngles;

        //target.transform.rotation *= phoneQuat;
    }

    void LateUpdate()
    {
        StartCoroutine(UploadPNG());
    }

    void OnApplicationQuit()
    {
        socket.close();
    }

    IEnumerator UploadPNG()
    {
        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();

        // Read screen contents into the texture
        texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
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