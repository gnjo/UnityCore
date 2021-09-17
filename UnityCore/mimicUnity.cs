namespace mimic{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Threading.Tasks;

    public class MimicUnity : MimicRunnerEx
    {
        public async Task DBG(string cmd, string arg)
        {
            Debug.Log(arg.ToData());
            next();//
            await Task.Delay(0);
        }

        public async Task WAI(string cmd, string arg)
        {
            var num = arg.ToData().ToValue<int>(0);
            await Task.Delay(num);
            next();//
                   //await Task.Delay(0);
        }

        public async Task KEY(string cmd, string arg)
        {
            var ret = await KeyGetter.Get();
            gData["$" + cmd] = ret;
            next();//
        }
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


/*quick

await @"#start......".Mimic(nowline => Debug.Log(nowline));

public static class MimicStringRunExtension{
    public static async Task Mimic(this string code,System.Action<string> debug_nowline = null){
        await new Mimic().Run(code,debug_nowline);
    }
}

*/