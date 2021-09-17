using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScaler : MonoBehaviour
{
    void Awake()
    {
        //Application.targetFrameRate=20;
        ScaleChange();
    }
    void ScaleChange(){
        const float fixed_width=640f;
        const float fixed_height=480f;
        var sw=Screen.width/fixed_width;
        var sh=Screen.height/fixed_height;
        var scale=Mathf.Min(sw,sh);
        Debug.Log(scale);
        gameObject.transform.localScale=new Vector3(scale,scale,1);
    }
}
