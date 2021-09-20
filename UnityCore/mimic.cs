namespace mimic
{
    using System.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Reflection;


#if !ENABLE_MONO
    public static class Debug
    {
        public static void Log(object s) => System.Console.WriteLine(s);
    }
#endif    

    public static class MimicExteinsion
    {
        public static string[] Split(this string s, string sep, int count)
        {
            ///配列は必ず空白文字で初期化されている。
            var right = Enumerable.Repeat("", count).ToArray();
            var ary = s.Split(new string[] { sep }, count, System.StringSplitOptions.None);
            for (var i = 0; i < ary.Length; i++) right[i] = ary[i];
            return right;
        }
        public static string[] Split(this string s, char sep, int count)
        {
            return s.Split("" + sep, count);
        }
        public static string[] Split(this string s, string sep)
        {
            return s.Split(new string[] { sep }, System.StringSplitOptions.None);
        }


        public static string[] Param(this string s, string sep, int count)
        {
            ///配列は必ず空白文字で初期化されている。
            var right = Enumerable.Repeat("", count).ToArray();
            var ary = s.Split(new string[] { sep }, count, System.StringSplitOptions.None);
            for (var i = 0; i < ary.Length; i++) right[i] = ary[i];
            return right;
        }
        public static string[] Param(this string s, char sep, int count)
        {
            return s.Param("" + sep, count);
        }
        public static string[] Param(this string s, string sep)
        {
            return s.Split(new string[] { sep }, System.StringSplitOptions.None);
        }


        public static string ToLF(this string code)
        {
            return Regex.Replace(code, "\r\n|\r|\n", "\n");
        }
        public static bool ToBool(this string s, bool falsevalue)
        {
            if (Regex.IsMatch(s, @"^1$|^true$", RegexOptions.IgnoreCase)) return true;
            return false;
        }

    }//class

    public static class MimicMathExtension
    {
        
        static bool isMathable(this string me)
        {
            var rule = Regex.Escape("0123456789.+-/*%!<>=");
            var pattern = "^[" + rule + "]+$";
            return Regex.IsMatch(me, pattern);
        }

        static bool isNumable(this string me)
        {
            var pattern = @"^[+\-]?[0-9]+\.[0-9]+$|^[+\-]?[0-9]+$";
            return Regex.IsMatch(me, pattern);
        }
        static bool isCompareble(this string me)
        {
            var rule = Regex.Escape("!<>=");
            var pattern = "[" + rule + "]";
            return Regex.IsMatch(me, pattern);
        }
        
        static bool ToCompare(this string code)
        {
            //a > b
            var map = new Dictionary<string, System.Func<decimal, decimal, bool>>();
            map["=="] = (a, b) => a == b;
            map["!="] = (a, b) => a != b;
            map[">="] = (a, b) => a >= b;
            map["<="] = (a, b) => a <= b;
            map[">"] = (a, b) => a > b;
            map["<"] = (a, b) => a < b;

            bool f(string d) => Regex.IsMatch(code, Regex.Escape(d));
            if (!map.Keys.Any(f)) return false;
            var ch = map.Keys.Where(f).First();
            var ary = code.Param(ch, 2)
             .Select(d => d.Trim())
             .Select(d => d.ToMath())
             .ToArray()
             ;
            return map[ch](ary[0], ary[1]);
        }

        static decimal ToMath(this string code)
        {
            var map = new Dictionary<string, System.Func<decimal, decimal, decimal>>();
            map["+"] = (a, b) => a + b;
            map["-"] = (a, b) => a - b;
            map["*"] = (a, b) => a * b;
            map["/"] = (a, b) => a / b;
            map["%"] = (a, b) => (int)a % (int)b;

            bool f(string d) => Regex.IsMatch(code, Regex.Escape(d));
            if (!map.Keys.Any(f)) return f2(code);
            var ch = map.Keys.Where(f).First();
            var ary = code.Param(ch, 2)
             .Select(d => d.Trim())
             .Select(d => f2(d))
             .ToArray()
             ;
            return map[ch](ary[0], ary[1]);
            //
            decimal f2(string s)
            {
                if (decimal.TryParse(s, out var x)) return x;
                else return s.GetHashCode();
            }

        }

        public static T ToValue<T>(this string me, T falseValue) where T : System.IComparable
        {
            if (typeof(T) == typeof(string)) return (T)(object)me;
            //
            if (typeof(T) == typeof(bool))
            {
                decimal vv;
                if (me.isCompareble()) return (T)(object)me.ToCompare();
                else vv = me.ToValue<decimal>(-1);
                //
                var wk = (int)vv;
                if (wk == 1) return (T)(object)true;
                else if (wk == 0) return (T)(object)false;
                return falseValue;
            }

            ///
            decimal v;//数値は一度decimalへ
            if (me.isMathable())
            {
                v = me.ToMath();
            }
            else if (!decimal.TryParse(me, out v)) return falseValue;

            if (typeof(T) == typeof(decimal)) return (T)(object)v;

            if (typeof(T) == typeof(double)) return (T)(object)(double)v;

            if (typeof(T) == typeof(float)) return (T)(object)(float)v;

            if (typeof(T) == typeof(int)) return (T)(object)(int)v;

            if (typeof(T) == typeof(bool))
            {
                var wk = (int)v;
                if (wk == 1) return (T)(object)true;
                else if (wk == 0) return (T)(object)false;
                return falseValue;
            }

            return falseValue;
        }

    }//class

    public static class MimicDataMappingExtension
    {
        internal static string ToData(this string me)
        {
            var ch = '$';
            var dic = MimicRunner.gData;
            ///辞書を使って値を代入する。シンボルは$
            if (!Regex.IsMatch(me, Regex.Escape("" + ch))) return me;
            var keys = dic.Keys
             .Where(d => d.IndexOf(ch) != -1)
             .OrderByDescending(d => d.Length)
             ;
            foreach (var k in keys) me = me.Replace(k, dic[k]);
            return me;
        }

    }//class

    public class MimicRunner
    {
        protected readonly char LF = '\n';
        protected readonly char SP = ' ';
        protected int tick = 0;
        protected string[] codeary;
        protected bool isEnd() => tick > codeary.Length - 1;
        protected void next(int i = -1) => tick = i == -1 ? tick + 1 : i;
        protected async Task docmd(string cmdline)
        {
            var args = cmdline.Param(SP, 2);
            MethodInfo method = this.GetType().GetMethod(args[0]);
            if (method == null) next();
            else await (Task)method.Invoke(this, args.ToArray());
        }
        public static Dictionary<string, string> gData = new Dictionary<string, string>();

        public async Task Run(string code,System.Action<string> debug_nowline=null)
        {
            codeary = parse(code);
            while (!isEnd()){
                if(debug_nowline !=null) debug_nowline(tick+":"+codeary[tick]);
                await docmd(codeary[tick]);
            }
        }
        public virtual string[] parse(string code)
        {
            return code.Trim().ToLF().Split(LF).Select(d => d.Trim()).ToArray();
        }

        //basic command IFJ SET MRK
        public async Task IFJ(string cmd, string arg)
        {
            //IFJ a #xyz
            var ary = arg.Param(SP, 2).Select(d => d.Trim()).ToArray();
            var flg = ary[0].ToData().ToValue(false);
            var addr = ary[1];
            var i = -1;
            if (flg) i = codeary.ToList().FindIndex((a) => Regex.IsMatch(a, @$"^MRK {addr}$"));
            //
            gData["$" + cmd] = addr;
            next(i);
            await Task.Delay(0);
        }

        public async Task MRK(string cmd, string arg)
        {
            //MRK #xyz
            gData["$" + cmd] = arg;
            next();
            await Task.Delay(0);
        }

        public async Task SET(string cmd, string arg)
        {
            //SET $a 1234
            decimal badcode = -909090;
            var ary = arg.Param(SP, 2).Select(d => d.Trim()).ToArray();
            var wk =ary[1].ToData();
            var num =wk.ToValue<decimal>(badcode);
            gData[ary[0]] = (num == badcode)?wk:num.ToString();
            gData["$" + cmd] = arg;
            next();
            await Task.Delay(0);
        }

        //
        public async Task DBG(string cmd, string arg)
        {
            arg = arg.ToData();
            Debug.Log(arg);
            gData["$" + cmd] = "" + arg;
            next();//need next
            await Task.Delay(0);
        }

        public async Task WAI(string cmd, string arg)
        {
            var num = arg.ToData().ToValue<int>(0);
            await Task.Delay(num);
            gData["$" + cmd] = "" + num;
            next();//
            //await Task.Delay(0);
        }

        public async Task KEY(string cmd, string arg)
        {
            var ret = await KeyGetter.Get();
            gData["$" + cmd] = ret;
            next();//
        }

    }
}//namespace mimic

namespace mimic
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Collections.Generic;

    public class MimicRunnerEx : MimicRunner
    {
        public override string[] parse(string code)
        {

            //return code.Trim().ToLF().Param(LF).Select(d => d.Trim()).ToArray();

            code = code.Trim().ToLF() + LF;
            var re = @"{{{[\s\S]*?}}}|\$.+ =\.[\s\S]*?\/\/|.*\s";
            var re_empty = @"^\s|^\/\/";
            var ms = Regex.Matches(code, re);

            var ary = new List<Match>();//
            foreach (Match match in ms) ary.Add(match);//

            return ary.Select(dd => dd.Value)
             .Where(dd => !Regex.IsMatch(dd, re_empty))
             .Select(dd => dd.Trim())
             .Select(format)
             .ToArray();
        }

        public string format(string str)
        {
            var buf = str;
            if (Regex.IsMatch(buf, @"=\."))
            {
                var ary = buf.Param("=.", 2).Select(dd => dd.Trim()).ToArray();
                var wk = ary[1].Param("//").First().Trim();
                return "SET" + SP + ary[0] + SP + wk; //ary[1];
            }
            if (Regex.IsMatch(buf, @"^\$"))
            {
                var ary = buf.Param("=", 2).Select(dd => dd.Trim()).ToArray();
                return "SET" + SP + ary[0] + SP + ary[1];
            }
            //#start //=> MRK #start
            if (Regex.IsMatch(buf, @"^#"))
            {
                return "MRK" + SP + buf;
            }
            //>>> $a=b >>>#start => IFJ $a=b #start
            if (Regex.IsMatch(buf, @"^>>>(.*)>>>"))
            {
                var ary = buf.Param(">>>", 3).Select(dd => dd.Trim()).ToArray();
                return "IFJ" + SP + ary[1] + SP + ary[2];
            }

            //>>> #start //=> IFJ 1 #start
            if (Regex.IsMatch(buf, @"^>>>"))
            {
                var ary = buf.Param(">>>", 2).Select(dd => dd.Trim()).ToArray();
                return "IFJ 1" + SP + ary[1];
            }
            if (Regex.IsMatch(buf, @"^{{{"))
            {
                buf = buf.Replace("{{{", "").Replace("}}}", "").Trim();
                return "SET" + SP + "$$$" + SP + buf;
            }
            return buf;
        }

    }

}//namespace mimic


/*

public class Mimic : MimicRunnerEx
{

}

*/