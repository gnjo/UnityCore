using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas2D : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] int width=240;
    [SerializeField] int height=135;
    // Start is called before the first frame update
    void Start()
    {
        if(cam ==null) cam = Camera.main;
        cam.rect =new Rect(0,0,width,height);
        //attach to Canvas
        var canvas=gameObject.GetComponent<Canvas>();
        canvas.pixelPerfect=true;
        canvas.renderMode =RenderMode.ScreenSpaceCamera;
        canvas.worldCamera=cam;
        canvas.planeDistance=0.3f;//check camera far;!!!!!!!!!!!!!!!!!

        var cs=gameObject.GetComponent<CanvasScaler>();
        cs.uiScaleMode =CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referencePixelsPerUnit=8;
        cs.referenceResolution =new Vector2(width,height);
        cs.matchWidthOrHeight=0.5f;
        
    }

}
