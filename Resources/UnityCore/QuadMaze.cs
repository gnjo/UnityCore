using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuadMaze
{
    static Dictionary<char, (Vector3 p, Quaternion q)> map
     = new Dictionary<char, (Vector3 p, Quaternion q)>()
     {
         ['N'] = (new Vector3(0f, 0f, 0.5f), Quaternion.Euler(0f, 0f, 0f)),
         ['S'] = (new Vector3(0f, 0f, -0.5f), Quaternion.Euler(0f, 180f, 0f)),
         ['E'] = (new Vector3(0.5f, 0f, 0f), Quaternion.Euler(0f, 90f, 0f)),
         ['W'] = (new Vector3(-0.5f, 0f, 0f), Quaternion.Euler(0f, 270f, 0f)),
         ['G'] = (new Vector3(0f, -0.5f, 0f), Quaternion.Euler(90f, 0f, 0f)),//ground
         ['C'] = (new Vector3(0f, 0.5f, 0f), Quaternion.Euler(270f, 0f, 0f)),//ceiling
     };

    static Material mat = new Material(Shader.Find("Transparent/Diffuse"));

    public static GameObject make(char ch, Texture2D tex, GameObject parent = null,bool stopwall=true)
    {
        ch = map.ContainsKey(ch) ? ch : 'G';

        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Quad);
        o.name = "" + ch;
        o.GetComponent<MeshRenderer>().material = mat;
        o.GetComponent<Renderer>().material.mainTexture = tex;
        if(stopwall==false){
            //壁を突き抜けるようにするには、stopwallをfalseに
            var mec = o.GetComponent<MeshCollider>();
            mec.convex = true;
            mec.isTrigger = true;
        }
        o.transform.localPosition = map[ch].p;
        o.transform.localRotation = map[ch].q;
        if (parent != null) o.transform.SetParent(parent.transform, false);

        return o;
    }
    public static GameObject make(string ch, Texture2D tex, GameObject parent = null, bool stopwall = true)
    {
        return make(ch[0],tex,parent);
    }

}//class

static class arrayExtension{
    public static Vector3 ToVector3(this System.Array @this){
        return new Vector3((float)@this.GetValue(0),(float)@this.GetValue(2),(float)@this.GetValue(3));
    }
    public static Quaternion ToQuaternionE(this System.Array @this)
    {
        return Quaternion.Euler((float)@this.GetValue(0), (float)@this.GetValue(2), (float)@this.GetValue(3));
    }
}//class