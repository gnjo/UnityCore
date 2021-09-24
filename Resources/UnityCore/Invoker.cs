namespace Invoker
{
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Linq;

    public static class MethodExtension
    {
        public static async Task<dynamic> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            //あらゆる関数をタスク化する。
            var awaitable = @this.Invoke(obj, parameters);
            if (isNonTask(@this) && awaitable != null) return awaitable;
            if (isNonTask(@this)) return true;
            if (isVoidTask(@this)){
                await (Task)awaitable;
                return true;
            }else{
                return ((Task<dynamic>)awaitable).GetAwaiter().GetResult();
            }
        }

        static bool isNonTask(MethodInfo m)
        {
            var ch = typeof(Task).ToString();
            if (m.ReturnType.ToString().IndexOf(ch) == -1) return true;
            return false;
        }
        static bool isVoidTask(MethodInfo m)
        {
            var ch = typeof(Task).ToString();
            if (m.ReturnType.ToString() == ch) return true;
            return false;
        }

    }


    public static class cmdlineExtension
    {
        public static async Task<dynamic> cmdline(this string str, object instance)
        {

            System.Type objType = (instance is System.Type) ? (System.Type)instance : instance.GetType();
            char sep = ' ';
            var obj = (instance is System.Type) ? null : instance;

            var ary = str.Split(sep);
            var cmd = ary.First().Replace("@", "");
            var fn = objType.GetMethod(cmd);
            if (fn == null) return false;
            var max = fn.GetParameters().Length;
            var args = Enumerable.Repeat("", max).ToArray();//引数分用意
            var _args = ary.Skip(1).Take(max).ToArray();
            for (var i = 0; i < _args.Length; i++) args[i] = _args[i];
            return await fn.InvokeAsync(obj, args);
        }
    }//class CmdLine

    /*usage

    using System.Threading.Tasks;
    using Invoker;

        public class wrapper
        {
            public async Task wait(string time)
            {
                //wait 100
                var ret = int.Parse(time);
                await Task.Delay(ret);
            }
            public void dummy()
            {
                //dummy
                Console.WriteLine("dummy");
            }

        }//class

        class Program
        {
            static async Task Main(string[] args)
            {
                var x = new wrapper();
                await "wait 1000".cmdline(x);
                await "dummy".cmdline(x);
                Console.WriteLine("Hello World!");
            }
        }


    */

}//
