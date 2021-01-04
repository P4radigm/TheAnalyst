using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SnapShotCamera : MonoBehaviour
{
    [SerializeField] private Sprite pngBook;

    private int resWidth = 702;
    private int resHeight = 904;
    private Camera snapCam;

    public void CallTakeSnapshot()
    {
        snapCam.gameObject.SetActive(true);
    }

    private void Awake()
    {
        snapCam = GetComponent<Camera>();
        if(snapCam.targetTexture == null) { snapCam.targetTexture = new RenderTexture(resWidth, resHeight, 24); }
        else 
        {
            resWidth = snapCam.targetTexture.width;
            resHeight = snapCam.targetTexture.height;
        }
        snapCam.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (snapCam.gameObject.activeInHierarchy) 
        { 
            Texture2D snapchot = new Texture2D(resWidth, resHeight, TextureFormat.RGBA32, false);
            snapCam.Render();
            RenderTexture.active = snapCam.targetTexture;
            snapchot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            byte[] bytes = snapchot.EncodeToPNG();
            string fileName = SnapshotName();
            System.IO.File.WriteAllBytes(fileName, bytes);
            snapCam.gameObject.SetActive(false);
        }
    }

    private string SnapshotName()
    {
        return string.Format("{0}/Snapshots/snap_{1}x{2}_{3}.png",
            Application.dataPath,
            resWidth,
            resHeight,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
}
