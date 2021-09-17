# UnityCore

lastupdate 20210917.1800

```C#
//sample
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Linq;

using mimic;
using Uni3D;

public class mimicTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {

        await @"
#start
ACT Image 0
$a=0
#start.loop
$a=$a+1
WAI 100
>>> $a==10 >>>#end

>>>#start.loop

#end
ACT Image 1
IMG stand
WAI 1000
ACT Image 0
WAI 1000
IMG #028
ACT Image 1

        ".Mimic(nowline => Debug.Log(nowline));
    }

}

public static class MimicStringRunExtension{
    public static async Task Mimic(this string code,System.Action<string> debug_nowline = null){
        await new Mimic().Run(code,debug_nowline);
    }
}

public class Mimic:MimicUnity{
    Image img;
    public async Task IMG(string cmd, string arg)
    {
        //IMG aaa
        if (img == null) img = GameObject.Find("Image").GetComponent<Image>();
        img.sprite = Uni3D.Maker.ftex(arg).ToSprite();
        next();//
        await Task.Delay(0);
    }

}

```
