using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
public static class TransformExtension
{
    public static string ToPos(this Transform @this, Vector3 offset, float basesize = 1.0f)
    {
        var pure = @this.localPosition - offset;
        var f = (int)(pure.y / basesize) * -1;
        var x = (int)(pure.x / basesize);
        var y = (int)(pure.z / basesize) * -1;
        var v = @this.ToV();

        return $"F{pad(f)}X{pad(x)}Y{pad(y)}.{v}";
        //
        string pad(int num)
        {
            var s = "00" + num;
            return s.Substring(s.Length - 2);
        }
    }

    public static string ToV(this Transform @this)
    {
        var t = @this;
        var a = (int)t.eulerAngles.y; //0 is 1.0000E-06...
        if (a == 0) return "N";
        else if (a == 90) return "E";
        else if (a == 180) return "S";
        else if (a == 270) return "W";
        return "N?";
    }

    public static Vector4 _movesize = new Vector4(1, 1, 1, 90);
    public static async Task TickMove(this Transform @transform, Vector4 v, Vector4? movesize = null, int tickcount = 3)
    {
        var m = (movesize == null) ? _movesize : movesize.Value;
        //m.x is movesize
        //m.w is moveangle
        var d = tickcount;
        Vector3 target = transform.localPosition + transform.direction(v) * m.x;
        float targetangle = transform.localEulerAngles.y + v.w * m.w;

        for (var i = 0; i < d + 2; i++)
        {
            var lop = transform.localPosition;
            var roy = transform.localEulerAngles.y;
            transform.localPosition = Vector3.MoveTowards(lop, target, m.x / d);
            transform.localRotation = Quaternion.Euler(0, Mathf.MoveTowardsAngle(roy, targetangle, m.w / d), 0);
            await Task.Delay(5);
        }
        //
        return;
    }
    public static Vector3 direction(this Transform transform, float vx, float vy, float vz)
    {
        if (vz != 0)
        {
            var dic_fw = new Dictionary<string, Vector3>(){
                {"N",new Vector3(0,0,1)},
                {"E",new Vector3(1,0,0)},
                {"S",new Vector3(0,0,-1)},
                {"W",new Vector3(-1,0,0)},
                };
            var V3 = dic_fw[transform.ToV()] * vz;
            V3.y = vy;
            return V3;
        }

        if (vx != 0)
        {
            var dic_right = new Dictionary<string, Vector3>(){
                {"N",new Vector3(1,0,0)},
                {"E",new Vector3(0,0,-1)},
                {"S",new Vector3(-1,0,0)},
                {"W",new Vector3(0,0,1)},
                };
            var V3 = dic_right[transform.ToV()] * vx;
            V3.y = vy;
            return V3;
        }

        return new Vector3(vx, vy, vz);
    }
    public static Vector3 direction(this Transform transform, Vector4 v)
    {
        return transform.direction(v.x, v.y, v.z);
    }

}//class



/*
    //using DG.Tweening;

    async Task RotR()
    {
        bool flg = true;
        transform
         .DOLocalRotateQuaternion(Quaternion.Euler(0, transform.localEulerAngles.y + moveangle, 0), movesec)
         .OnComplete(() => flg = false)
        ;
        while (true) await Task.Delay(5);
    }
    async Task Foward()
    {
        bool flg = true;
        transform
         .DOLocalMove(transform.localPosition + movesize * getdirection(true), movesec)
         .OnComplete(() => flg = false)
        ;
        while (true) await Task.Delay(5);
    }

*/


/*sample


using UnityEngine;
using UnityEngine.UI;

public class MoveCube : MonoBehaviour
{
    readonly Vector3 offset = new Vector3(0, 0.5f, 0);
    readonly Vector4 movesize = new Vector4(1, 1, 1, 90);

    async void Start()
    {

#if !UNITY_WEBGL
        Application.targetFrameRate=20;
#endif
        UpdatePosText();

        var ret="";
        while(ret!="P"){
            ret=await KeyGetter.Get();

            if (ret == "_U") await transform.TickMove(new Vector4(0, 0, 1, 0),movesize); //foward
            else if (ret == "_D") await transform.TickMove(new Vector4(0, 0, -1, 0),movesize);//back
            else if (ret == "L") await transform.TickMove(new Vector4(-1, 0, 0, 0),movesize);//left
            else if (ret == "R") await transform.TickMove(new Vector4(1, 0, 0, 0),movesize);//right
            else if (ret == "_L") await transform.TickMove(new Vector4(0, 0, 0, -1),movesize);//role the left
            else if (ret == "_R") await transform.TickMove(new Vector4(0, 0, 0, 1),movesize);//role the right

            UpdatePosText();
        }
        
    }    
    void UpdatePosText()=>Debug.Log( transform.ToPos(offset) );

    
}//class


*/
