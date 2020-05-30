using System.Collections.Generic;
using UnityEngine;
using System;

public class TripodController : MonoBehaviour
{
    public GameObject TripodModel;
    public Action<Texture2D> OnTripodViewUpdate;

    private Camera MainCamera;
    private List<GameObject> Tripods = new List<GameObject>();
    private int CurTripodIdx = 0;

    //For debugging purposes in the inspector window only
    [SerializeField]
    private RenderTexture ColorMatrix;
    private int TripodLayer;

    void Start()
    {
        MainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        TripodLayer = LayerMask.NameToLayer("Tripods");
    }

    void Update()
    {
        if (Input.GetKeyDown("c"))
        {
            CreateTripod();
        }
        else if(Tripods.Count > 1 && Input.GetKeyDown("z")) //Cycle through cameras left
        {
            CycleThroughAvailableCameras(-1);
        }
        else if (Tripods.Count > 1 && Input.GetKeyDown("x")) //Cycle through cameras right
        {
            CycleThroughAvailableCameras(1);
        }
        else if (Tripods.Count > 0 && Input.GetKeyDown(",") && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) //Delete Current Tripod
        {
            DestroyTripod();
        }

        BroadcastCurrentTripodView();
    }

    private void DestroyTripod()
    {
        if (Tripods.Count > 0 && CurTripodIdx >= 0 && CurTripodIdx < Tripods.Count)
        {
            Destroy(Tripods[CurTripodIdx]);
            Tripods.RemoveAt(CurTripodIdx);
            CycleThroughAvailableCameras(-1);
        }
    }

    private void BroadcastCurrentTripodView()
    {
        if (OnTripodViewUpdate != null && ColorMatrix != null && Tripods.Count > 0)
        {
            int width = ColorMatrix.width;
            int height = ColorMatrix.height;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            Camera activeCamera = Tripods[CurTripodIdx].GetComponent<Camera>();

            // Initialize and render
            RenderTexture.active = ColorMatrix;

            // Read pixels from active RenderTexture, this moves data from GPU to system RAM, slow but necessary to read pixel data!
            tex.ReadPixels(new Rect(0, 0, ColorMatrix.width, ColorMatrix.height), 0, 0);
            OnTripodViewUpdate(tex);
        }
    }

    private void CreateTripod()
    {
        //Create a new camera from the current player's view
        var tripod = Instantiate(TripodModel, MainCamera.transform.position, MainCamera.transform.rotation); //Just create a dummy object so we can see where we created our camera
        tripod.layer = TripodLayer;

        var camera = tripod.AddComponent<Camera>();

        InitCamera(camera);
        EnableCamera(camera);

        //We have to handle the first camera we create a little different from the rest since there is no previous camera to disable
        if (Tripods.Count > 0 && CurTripodIdx < Tripods.Count)
        {
            DisableCamera(Tripods[CurTripodIdx].GetComponent<Camera>());
            CurTripodIdx++;
        }

        Tripods.Add(tripod);
    }

    private void CycleThroughAvailableCameras(int relativeIndex)
    {
        if(Tripods.Count == 0)
        {
            CurTripodIdx = 0;
            return;
        }

        //Ensure always working with positive values mod Tripods.Count, even if CurTripodIdx somehow gets to be negative and outside the range of |Tripods.Count|
        //The final '+ Tripods.Count' before modulus is to ensure negative values are properly shifted with respect to modulo Tripods.Count, has no impact on positive numbers
        int oldIdx = ((CurTripodIdx % Tripods.Count) + Tripods.Count) % Tripods.Count;
        int newIdx = ((CurTripodIdx % Tripods.Count) + (relativeIndex % Tripods.Count) + Tripods.Count) % Tripods.Count;

        if(oldIdx < Tripods.Count && oldIdx >= 0)
        {
            DisableCamera(Tripods[oldIdx].GetComponent<Camera>());
        }

        if(newIdx < Tripods.Count && newIdx >= 0)
        {
            EnableCamera(Tripods[newIdx].GetComponent<Camera>());
        }

        CurTripodIdx = newIdx;
    }

    private void DisableCamera(Camera camera)
    {
        camera.enabled = false;
        camera.targetTexture = null;
    }

    private void InitCamera(Camera camera)
    {
        //Ensure tripod cameras don't render other tripods by masking them out
        int mask = 1 << TripodLayer;
        camera.cullingMask = ~mask;

        camera.backgroundColor = Color.black;
        camera.depth = 1;
    }

    private void EnableCamera(Camera camera)
    {
        camera.enabled = true;
        camera.targetTexture = ColorMatrix;
    }
}
