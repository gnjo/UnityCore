using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;
using UnityEngine.UI;
using mimic;
using Uni3D;

public class TextMaker
{
    GameObject prefab;
    GameObject prefabImage;
    GameObject parent;
    List<GameObject> stack =new List<GameObject>();
    public TextMaker(string parentname="Canvas")
    {
        if(prefab==null) prefab = Resources.Load<GameObject>("UnityCore/PrefabText");
        if(prefabImage==null) prefabImage=Resources.Load<GameObject>("UnityCore/PrefabImage");
        if(parent==null) parent =GameObject.Find(parentname);
    }

    public void clear(){
        foreach(var go in stack)
            GameObject.Destroy(go);
        stack.Clear();
    }
    // Update is called once per frame
    public GameObject make(string rect,string text)
    {
        var go = GameObject.Instantiate(prefab);
        if(parent!=null) go.transform.SetParent(parent.transform);
        go.GetComponent<RectTransform>().localScale=new Vector3(1,1,1);
        //pos x,y,w,h
        //0,0,5,7 aiuewokakikukeko
        var ary = rect.Split(',').Select(d=>d.ToValue(0f)).ToArray();
        if(ary.Length==2){
            var v =new Vector3(ary[0],-1*ary[1],0);
            go.GetComponent<RectTransform>().anchoredPosition3D = v;
        }
        if(ary.Length==4){
            var v = new Vector3(ary[0], -1*ary[1], 0);
            go.GetComponent<RectTransform>().anchoredPosition3D = v;
            var s = new Vector2(ary[2], ary[3]);
            //
            go.GetComponent<RectTransform>()
             .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, s.x);
            go.GetComponent<RectTransform>()
             .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,s.y);             
        }
        //
        go.GetComponent<Text>().text=text;
        stack.Add(go);
        return go;
    }
    public GameObject makeWithBox(string rect, string text)
    {
        var go = GameObject.Instantiate(prefabImage);
        if (parent != null) go.transform.SetParent(parent.transform);
        go.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        //pos x,y,w,h
        //0,0,5,7 aiuewokakikukeko
        var ary = rect.Split(',').Select(d => d.ToValue(0f)).ToArray();
        var w =(int)ary[2];
        var h=(int)ary[3];
        var t2d = new DrawTexture2D();
        var board = new Texture2D(w,h);
        board.filterMode =FilterMode.Point;///
        t2d.Begin(board);
        t2d.Clear("#000a".ToColor());
        t2d.DrawRectangle(0,0,w-1,h-1,Color.white);
        t2d.End();
        go.GetComponent<Image>().sprite = board.ToSprite();

        if (ary.Length == 4)
        {
            var v = new Vector3(ary[0], -1*ary[1],0); //y is uppder
            go.GetComponent<RectTransform>().anchoredPosition3D = v;
            var s = new Vector2(ary[2], ary[3]);
            go.GetComponent<RectTransform>().sizeDelta = s;
        }

        //
        //go.GetComponent<Text>().text = text;
        stack.Add(go);
        
        return make(rect,text);
    }

}
