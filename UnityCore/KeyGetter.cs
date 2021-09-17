using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class KeyGetterObject : MonoBehaviour
{
    void Update()
    {
        if(Input.anyKeyDown)
            foreach(var m in KeyGetter.InputMap)
                if(Input.GetKeyDown(m.Key)) m.Value();
    }
}

public static class KeyGetter
{
    public static Dictionary<string,System.Action> InputMap=new Dictionary<string, System.Action>(){
        {"w",KeyGetter.PressArrowUp},
        {"s",KeyGetter.PressArrowDown},
        {"a",KeyGetter.PressArrowLeft},
        {"d",KeyGetter.PressArrowRight},

        {"j",KeyGetter.PressA},
        {"k",KeyGetter.PressB},
        {"l",KeyGetter.PressX},
        {"o",KeyGetter.PressY},
        {"q",KeyGetter.PressL},
        {"e",KeyGetter.PressR},

        {"space",KeyGetter.PressS},
        {"escape",KeyGetter.PressP},
    };

    static string key = "";
    static GameObject go;
    public static async Task<string> Get()
    {
        if(go==null){
            go=new GameObject("_KeyGetterGameObject");
            go.AddComponent<KeyGetterObject>();
        }
        key="";
        while(key=="") await Task.Delay(16);
        return key;
    }

    //for touch 
    public static void PressArrowUp() => key = "_U";
    public static void PressArrowDown() => key = "_D";
    public static void PressArrowLeft() => key = "_L";
    public static void PressArrowRight() => key = "_R";

    public static void PressA() => key = "A";
    public static void PressB() => key = "B";
    public static void PressX() => key = "X";
    public static void PressY() => key = "Y";
    public static void PressL() => key = "L";
    public static void PressR() => key = "R";
    public static void PressS() => key = "S"; //like a start
    public static void PressP() => key = "P"; //like a pause

}

/*testcoce

    async void Start()
    {
        var ret="";
        while(ret!="P"){
            ret=await KeyGetter.Get();
            Debug.Log(ret);
        }
        Debug.Log("end");
    }

*/