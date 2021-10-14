
namespace StringUtil{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Linq;

    internal static class StringExtensions
    {
        public static string[] SplitNum(this string self, int count)
        {
            var result = new List<string>();
            var length = (int)Math.Ceiling((double)self.Length / count);

            for (int i = 0; i < length; i++)
            {
                int start = count * i;
                if (self.Length <= start)
                {
                    break;
                }
                if (self.Length < start + count)
                {
                    result.Add(self.Substring(start));
                }
                else
                {
                    result.Add(self.Substring(start, count));
                }
            }

            return result.ToArray();
        }
        public static bool between(this int @this,int include,int exclude){
            return include<=@this && @this<exclude;
        }
        public static string ToLF(this string code) => System.Text.RegularExpressions.Regex.Replace(code, "\r\n|\r|\n", "\n");

    }


    public class TextMixer
    {
        public int size;
        public int width;
        public int height;
        public int boxflg;//0 or 1 or 2
        public char sp = '　';
        public string LF="\n";
        public StringBuilder cleanbuffer;
        public StringBuilder buffer;
        public TextMixer(int w, int h, int flg = 0)
        {

            width = w; height = h; boxflg = flg; size = w * h;
            var ret = "";
            if (flg == 0)
            {
                for (var i = 0; i < width * height; i++) ret += sp;
            }
            else
            {
                ret = box(width, height, sp).Replace(LF, "");
            }

            this.cleanbuffer = new StringBuilder(ret);
            this.buffer = new StringBuilder(cleanbuffer.ToString());
            //
        }
        public TextMixer Text(int ox, int oy, string mes)
        {
            var w = width;
            var h = height;
            if (this.boxflg != 0)
            {
                w = w - 2;
                h = h - 2;
                ox = ox + 1;
                oy = oy + 1;
            }
            var ary = mes.ToLF().Split(LF);
            bool out_of_range(int now, int max)=>now>max;

            for (var j = 0; j < ary.Length; j++)
                for (var i = 0; i < ary[j].Length; i++)
                {
                    //
                    if (out_of_range(oy + j, h) || out_of_range(ox + i, w)) continue;
                    //if(!(oy+j).between(0,h) || !(ox+i).between(0,w)) continue;
                    //
                    var str = ary[j];
                    var cel = str[i];
                    var num = (oy + j) * width + ox + i;
                    if(!num.between(0,width*height))continue;
                    //if(out_of_range(num,width*height)) continue;
                    buffer[num] = cel;
                }
            return this;

        }
        public TextMixer Clear()
        {
            buffer = new StringBuilder(cleanbuffer.ToString());
            return this;
        }
        public override string ToString()
        {
            return string.Join(LF, buffer.ToString().SplitNum(width));
        }
        public string box(int w, int h, char sp = '　')
        {
            string buf = "";
            string CR = LF;//

            var thin = boxflg;
            //━　┃　┏　┓　┛　┗　┣　┳　┫　┻　╋
            string tl = "┏";
            string tr = "┓";
            string bl = "┗";
            string br = "┛";
            string bar = "━";
            string stand = "┃";

            if (thin == 2)
            {
                tl = "┌";
                tr = "┐";
                bl = "└";
                br = "┘";
                bar = "─";
                stand = "│";
            }

            //━┃┏┓┗┛
            //─│┌┐└┘

            //string sp = sp;
            //;
            string barbuf = "";
            string spbuf = "";
            string middlebuf = "";
            for (var i = 0; i < w - 2; i++) { barbuf += bar; spbuf += sp; }
            middlebuf = stand + spbuf + stand;
            string top = tl + barbuf + tr;

            string middle = "";
            for (var j = 0; j < h - 2; j++) middle += middlebuf + CR;
            string bottom = bl + barbuf + br;
            //

            buf += top + CR + middle + bottom;

            return buf;

        }

    }

}



/*usage

    class Program
    {
        static void Main(string[] args)
        {

            var viewer =new TextMixer(32,24,1);
            var maze = new TextMixer(20,20,1);
            var party = new TextMixer(20+2,6+2,1)
            .Text(0,0,@"
１２３４５６７８９０１２３４５６７８９０
１２３４５６７８９０１２３４５６７８９０
１２３４５６７８９０１２３４５６７８９０
１２３４５６７８９０１２３４５６７８９０
１２３４５６７８９０１２３４５６７８９０
            ".Trim());

            var x = new TextMixer(14,6+2,1)
             .Text(1,0,"あいうえを\r\nかきくけこ")
             .Text(0,0,"＊")
             ;


             viewer.Text(5,0,maze.ToString());
             viewer.Text(0,0,x.ToString());
             viewer.Text(4,14,party.ToString());

             Console.WriteLine(viewer.ToString());

            Console.ReadLine();

            Console.Clear();
            var qx = new Quest(32, 24, 1);
            Console.WriteLine(qx.ToString());
            Console.ReadLine();


        }
    }

*/