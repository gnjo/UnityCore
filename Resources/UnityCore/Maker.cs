
namespace Uni3D
{
    using System.Collections;
    using UnityEngine;
    using System.Linq;
    using System.Text.RegularExpressions;

    //using static Uni3D.Maker;

    public static class Maker
    {
        static GameObject go_runner;
        static EndlessRunner runner;
        public static GameObject makebox(Texture2D tex, string name = "box", bool inbox = false)
        {
            var texs = Enumerable.Repeat(tex, 6).ToArray();
            return makebox(texs, name, inbox);
        }

        public static GameObject makebox(Texture2D[] texs, string name = "box", bool inbox = false)
        {
            float oh = 0.5f;
            float[,] POS = new float[,] {
            { 0f, 0f+oh, -0.5f },//front
            { 0f, 0f+oh, 0.5f },//back
            { -0.5f, 0f+oh, 0f },//left
            { 0.5f, 0f+oh, 0f },//right
            { 0f, 0.5f+oh, 0f },//up
            { 0f, -0.5f+oh, 0f }//down
            };
            float[,] ROT = new float[,] {
            { 0f, 0f, 0f },//front
            { 0f, 180f, 0f },//back
            { 0f, 90f, 0f },//left
            { 0f, 270f, 0f },//right
            { 90f, 0f, 0f },//up
            { 270f, 0f, 0f }//down
            };
            //inbox
            float ex = -1;
            float[,] _POS = new float[,] {
            { 0f, 0f+oh, -0.5f*ex },//front
            { 0f, 0f+oh, 0.5f*ex },//back
            { -0.5f*ex, 0f+oh, 0f },//left
            { 0.5f*ex, 0f+oh, 0f },//right
            { 0f, 0.5f*ex+oh, 0f },//up
            { 0f, -0.5f*ex+oh, 0f }//down
        };

            GameObject customCube = new GameObject(name);
            GameObject tmpQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            //tmpQuad.transform.localScale = new Vector3(1, 1, 1);

            for (int i = 0; i < 6; i++)
            {
                var p = (!inbox)
                 ? new Vector3(POS[i, 0], POS[i, 1], POS[i, 2])
                 : new Vector3(_POS[i, 0], _POS[i, 1], _POS[i, 2])
                 ;
                var r = Quaternion.Euler(ROT[i, 0], ROT[i, 1], ROT[i, 2]);
                var o = GameObject.Instantiate(tmpQuad, p, r);
                o.transform.parent = customCube.transform;
                o.GetComponent<Renderer>().material = texs[i].ToMaterial();
            }
            GameObject.Destroy(tmpQuad);
            return customCube;
        }

        public static GameObject Move(this GameObject @this, int f, int x, int y, float movesize = 1.0f)
        {
            var add = new Vector3(x, f, y) * movesize;
            @this.transform.localPosition += add;
            return @this;
        }

        public static Texture2D ftex(object o)
        {
            if (go_runner == null)
            {
                //for web
                go_runner = new GameObject("endless_runner");
                go_runner.AddComponent<EndlessRunner>();
                runner = go_runner.GetComponent<EndlessRunner>();
            }

            if (o is string)
            {
                string x = (string)o;
                if (x == "") return "#fff".ToColor().ToTexture2D();
                if (x[0] == '#') return x.ToColor().ToTexture2D();
                if (isUri(x))
                {
                    var tex = new Texture2D(1, 1);
                    runner.byWebData(x, (bytes) => { tex.LoadImage(bytes); tex.Apply(); });
                    return tex;
                }
                var a = Resources.Load<Texture2D>(x);
                Resources.UnloadAsset(a);
                return a;
            }
            if (o is Color)
            {
                Color x = (Color)o;
                return x.ToTexture2D();
            }
            if (o is Texture2D)
            {
                return (Texture2D)o;
            }
            if (o is byte[])
            {
                var wk = new Texture2D(1, 1);
                var x = (byte[])o;
                wk.LoadImage(x);
                wk.Apply();
                return wk;
            }

            Debug.LogWarning("ftex format exception. ftex needs string,Color,Texture2d");
            return new Texture2D(1, 1);
        }
        static bool isUri(string s) => Regex.IsMatch(s, @"^file:|^http:|^https:");
    }

    public class EndlessRunner : MonoBehaviour
    {
        public void byWebData(string uri, System.Action<byte[]> caller)
        {
            StartCoroutine(WebData(uri, caller));
        }

        IEnumerator WebData(string uri, System.Action<byte[]> caller)
        {
            //using UnityEngine.Networking;
            var www = UnityEngine.Networking.UnityWebRequest.Get(uri);
            yield return www.SendWebRequest();
            //if (www.isHttpError || www.isNetworkError) Debug.Log(www.error);
            if(www.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError) Debug.Log(www.result);    
            //
            caller(www.downloadHandler.data);
        }
    }

    public static class Texture2DExtension
    {

        public static Color ToColor(this string @this)
        {
            if (ColorUtility.TryParseHtmlString(@this, out var c)) return c;
            return Color.white;
        }

        public static Texture2D ToTexture2D(this string @this)
        {
            var tex = new Texture2D(1, 1);
            Color c = @this.ToColor();
            tex.SetPixel(1, 1, c);
            tex.Apply();

            return tex;
        }

        public static Texture2D ToTexture2D(this Color @this)
        {
            var tex = new Texture2D(1, 1);
            Color c = @this;
            tex.SetPixel(1, 1, c);
            tex.Apply();

            return tex;
        }
        /*
        public static MakeByFile(this Texture2DExtension @this, string name = "#fff",string basepath="")
        {
            future....
        }
        */

        public static Material ToMaterial(this Texture2D tex, string shadername = "Transparent/Diffuse")
        {
            Material mat = new Material(Shader.Find(shadername));
            mat.SetTexture("_MainTex", tex);
            return mat;
        }
        public static Sprite ToSprite(this Texture2D tex)
        {
            return Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.zero);
        }
    }

}