using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

//unity
#if ENABLE_MONO
using UnityEngine;
public class KeyGetterObject : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
            foreach (var m in KeyGetter.InputMap)
                if (Input.GetKeyDown(m.Key)) m.Value();
    }
}

#endif
//

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

#if ENABLE_MONO
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
#else //consle application
    public static async Task<string> Get()
    {
        key = "";
        while (key == ""){
            var info = System.Console.ReadKey(true);
            var wk =info.KeyChar.ToString();
            if(info.Key == System.ConsoleKey.Spacebar) wk="space";
            else if(info.Key == System.ConsoleKey.Escape) wk="escape";
            foreach (var m in KeyGetter.InputMap)
                if (wk==m.Key) m.Value();

            await Task.Delay(16);
        }
        return key;
    }

#endif    


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

    public static (int dx,int dy,char v) KeyToNEWS(string key,char v='N'/*NEWS*/){
        //A 前進
        //_U 前進
        //_D 後退
        //_L 左を向く。その場で。
        //_R 右を向く。その場で。
        //L 左に一歩移動
        //R 右に一歩移動
        // N
        //W E
        // S
        if(!"NEWS".Find(v)){
            Debug($"KeyToNEWS error char v is NEWS:{v}");
            v='N';
        }
        if(!isIncludeKey(key)){
            Debug($"KeyToNEWS error char v is NEWS:{v}");
            key="B";
        }
        var def =(0,0,v);
        if("BXSP".Find(key))return def;
        if ("A_U".Find(key)){
            var d=diff(v);
            return (d.dx,d.dy,v);
        }
        if (key=="_D"){
            var n="NESW".IndexOf(v);
            var wk="SWNE";
            var d = diff(wk[n]);
            return (d.dx, d.dy, v);
        }
        if(key=="_L"){
            var n="NESW".IndexOf(v);
            var wk="WNES";
            return (0,0,wk[n]);
        }
        if(key=="_R"){
            var n = "NESW".IndexOf(v);
            var wk = "ESWN";
            return (0,0,wk[n]);
        }

        if(key=="L"){
            var n="NSEW".IndexOf(v);
            var wk="WENS";
            var d = diff(wk[n]);
            return (d.dx, d.dy, v);
        }
        if(key=="R"){
            var n = "NSEW".IndexOf(v);
            var wk = "EWSN";
            var d=diff(wk[n]);
            return (d.dx,d.dy,v);
        }
        Debug("dont reach KeyToNEWS");
        return def;
        //
        ;(int dx,int dy) diff(char v){
            if(v=='N')return (0,-1);
            else if(v=='S')return (0,1);
            else if(v=='E')return (1,0);
            else if(v=='W')return (-1,0);
            return (0,0);
        }
    }
    public static (int dx, int dy, char v) KeyToNEWS(string key,string v="N"/*NEWS*/){
        return KeyToNEWS(key,v[0]);
    }
    static bool isIncludeKey(string key){
        return "ABXYLRSP_U_D_L_R".Find(key);
    }
    static bool Find(this string @this,string ch)=>@this.IndexOf(ch)!=-1;
    static bool Find(this string @this,char ch)=>@this.Find(""+ch);
    static void Debug(string s){
        #if ENABLE_MONO
            Debug.Log(s);
        #else
            System.Console.WriteLine(s);
        #endif
    }

}//class

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

/*
        static async Task Main(string[] args)
        {
            var key="";
            while(key!="P"){
                key = await KeyGetter.Get();

                Console.WriteLine(key);
            }
            Console.ReadLine();
        }

*/