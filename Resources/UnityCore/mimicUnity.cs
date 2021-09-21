#if ENABLE_MONO
namespace mimic{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Threading.Tasks;

    public class MimicUnity : MimicRunnerEx
    {
        public Dictionary<string, GameObject> go_cash = new Dictionary<string, GameObject>();
        public async Task ACT(string cmd, string arg)
        {
            //あらゆるゲームオブジェクトは最初アクティブであること。その後、キャッシュする。
            //ACT Image 1
            //ACT Image 0 //gameobject.SetActive(false);
            //atteintion not Name With Space
            var ary = arg.Split(SP, 2);
            var name = ary[0].ToData();
            var flg = ary[1].ToValue<int>(1) == 1 ? true : false;
            GameObject go;
            if (go_cash.ContainsKey(name)) go = go_cash[name];
            else go = GameObject.Find(name);

            if (go != null)
            {
                go.SetActive(flg);
                go_cash[name] = go;
            }
            gData["$" + cmd] = arg;
            next();//
            await Task.Delay(0);
        }

    }//class

}//namespace
#else
namespace mimic
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using System;

    public class MimicConsole : MimicRunnerEx
    {
        public async Task SEL(string cmd, string arg)
        {
            //SEL a|b|c|d 3
            var cep = "|";
            var ary = arg.Param(SP, 2).Select(d => d.ToData()).ToArray();
            var cur = ary[1].ToValue(0);
            var a = ary[0].Split("|");
            //
            draw();
            var k = "";
            while (k != "A" && k != "B")
            {
                k = await KeyGetter.Get();
                if (k == "_U")
                {
                    cur--;
                    cur = (cur + 1000) % a.Length;
                    draw();
                }
                else if (k == "_D")
                {
                    cur++;
                    cur = (cur + 1000) % a.Length;
                    draw();
                }
            }
            //
            gData["$KEY"] = k;
            gData["$" + cmd] = "" + cur % a.Length;
            next();
            await Task.Delay(0);
            return;
            //
            void draw()
            {
                Console.Clear();
                for (var i = 0; i < a.Length; i++)
                {
                    var wk = (i == cur) ? "<color=#ff2288>" + a[i] + "</color>" : a[i];
                    Console.WriteLine(wk);
                }
            }
        }

    }//class

}//namespace

#endif

/*quick

await @"#start......".Mimic(nowline => Debug.Log(nowline));

public static class MimicStringRunExtension{
    public static async Task Mimic(this string code,System.Action<string> debug_nowline = null){
        await new Mimic().Run(code,debug_nowline);
    }
}

*/