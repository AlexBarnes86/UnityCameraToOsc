using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCreator : MonoBehaviour
{
    private Camera mainCamera;
    private List<GameObject> tripods = new List<GameObject>();
    int tripodIdx = 0;
    public GameObject indicator;
    public RenderTexture colorMatrix;

    void Start()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            // Create new cameras to use, copying from the main camera  
            var tripod = Instantiate(indicator, mainCamera.transform.position, mainCamera.transform.rotation); //Just create a dummy object so we can see where we created our camera
            var camera = tripod.AddComponent<Camera>();

            // Set the backgrounds 
            camera.backgroundColor = Color.black;

            // Set render order
            camera.depth = 1;

            // Camera 2 is a minimap, so disable the audio listener and make it small
            //var fullScreen = mainCamera.pixelRect;
            //camera.pixelRect = new Rect(fullScreen.xMin, fullScreen.yMin, fullScreen.width * 0.2f, fullScreen.height * 0.2f);

            //Disable current camera
            if(tripods.Count > 0)
            {
                Camera prevCamera = tripods[tripodIdx].GetComponent<Camera>();
                prevCamera.enabled = false;
                prevCamera.targetTexture = null;
                tripodIdx++;
            }
            camera.enabled = true;
            camera.targetTexture = colorMatrix;
            tripods.Add(tripod);
        }
        else if(tripods.Count > 1 && Input.GetKeyDown("z")) //Cycle through cameras left
        {
            Camera prevCamera = tripods[tripodIdx].GetComponent<Camera>();
            tripodIdx += tripods.Count - 1;
            tripodIdx %= tripods.Count;
            Camera nextCamera = tripods[tripodIdx].GetComponent<Camera>();
            SwitchCameras(prevCamera, nextCamera);
        }
        else if (tripods.Count > 1 && Input.GetKeyDown("x")) //Cycle through cameras right
        {
            Camera prevCamera = tripods[tripodIdx].GetComponent<Camera>();
            tripodIdx += tripods.Count + 1;
            tripodIdx %= tripods.Count;
            Camera nextCamera = tripods[tripodIdx].GetComponent<Camera>();
            SwitchCameras(prevCamera, nextCamera);
        }
        else if (tripods.Count > 0 && Input.GetKeyDown(",") && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {       
            Destroy(tripods[tripodIdx]);
            tripods.RemoveAt(tripodIdx);
            if (tripods.Count > 0)
            {
                tripodIdx += tripods.Count - 1;
                tripodIdx %= tripods.Count;
            }
            else
            {
                tripodIdx = 0;
            }
        }

        if (false) // colorMatrix != null && tripods.Count > 0)
        {
            int width = colorMatrix.width;
            int height = colorMatrix.height;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            Camera activeCamera = tripods[tripodIdx].GetComponent<Camera>();

            // Initialize and render
            RenderTexture.active = colorMatrix;

            // Read pixels from active RenderTexture, this moves data from GPU to system RAM, slow!
            tex.ReadPixels(new Rect(0, 0, colorMatrix.width, colorMatrix.height), 0, 0);

            string str = "";
            for(int r = 0; r < colorMatrix.height; ++r)
            {
                for (int c = 0; c < colorMatrix.width; ++c)
                {
                    str += tex.GetPixel(r, c) + ", ";
                }
                Debug.Log(str);
            }
        }
    }

    private void SwitchCameras(Camera oldCamera, Camera newCamera)
    {
        oldCamera.enabled = false;
        oldCamera.targetTexture = null;
        newCamera.enabled = true;
        newCamera.targetTexture = colorMatrix;
    }
}
