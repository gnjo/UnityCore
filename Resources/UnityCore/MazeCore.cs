

public class RandomXorS
{
    System.UInt32 x, y, z, w;
    public RandomXorS(int seed = 0)
    {
        x = 1234567890u;
        y = (System.UInt32)(seed >> 32) & 0xFFFFFFFF;
        z = (System.UInt32)(seed & 0xFFFFFFFF);
        w = x ^ z;
    }

    System.UInt32 Gen(){
        System.UInt32 t = x ^ (x << 11);
        x = y; y = z; z = w;
        w = (w ^ (w >> 19)) ^ (t ^ (t >> 8));
        return w;
    }
    double Round() => Gen() * (1.0 / (uint.MaxValue + 1.0));
    public int Next(int max = 0)=>(max == 0) ? (int)(Round()*int.MaxValue) : (int)(Round() *max);
}//class


public static class intbetweenExtension
{
    public static bool Between(this int num, int lower, int upper, bool inclusive = true)
    {
        return inclusive
            ? lower <= num && num <= upper
            : lower < num && num < upper;
    }
}//class


public class MazeCore
{
    //public System.Random r;
    public RandomXorS r;
    public int w;
    public int h;
    int calc_w;
    int calc_h;
    public char[,] grid;
    public static char road = '　';
    public static char wall = '壁';
    public static char pole = '柱';
    public static char door='扉';

    public char N = 'N', E = 'E', W = 'W', S = 'S', G = 'G', C = 'C';
    public MazeCore(int w, int h, int seed = 1)
    {
        this.w = w;
        this.h = h;
        calc_w = w * 2 + 1;
        calc_h = h * 2 + 1;
        r = new RandomXorS(seed);
        //r= new System.Random(seed);
        grid = MakeOutWall(this.w, this.h);
    }

    public int rand(int n)=>r.Next(n);

    public char GetGrid(int x, int y, char ch = 'G')
    {
        //NEWS GC
        x = x * 2 + 1;
        y = y * 2 + 1;
        if (ch == G) return grid[x, y];
        if (ch == C) return grid[x, y];
        if (ch == N) return grid[x, y - 1];
        if (ch == S) return grid[x, y + 1];
        if (ch == E) return grid[x + 1, y];
        if (ch == W) return grid[x - 1, y];
        else return grid[x, y];
    }
    public MazeCore SetGrid(int x,int y,char dir,char ch){
        //out of range
        if( !(x.Between(0,w-1) && y.Between(0,h-1)) )return this;
        //
        x = x * 2 + 1;
        y = y * 2 + 1;
        if (dir == G) grid[x, y]=ch;
        else if (dir == C) grid[x, y]=ch;
        else if (dir == N) grid[x, y - 1]=ch;
        else if (dir == S) grid[x, y + 1]=ch;
        else if (dir == E) grid[x + 1, y]=ch;
        else if (dir == W) grid[x - 1, y]=ch;
        return this;
    }
    public char[,] MakeOutWall(int _x, int _y)
    {
        //not calc size;
        var xmax = _x * 2 + 1;
        var ymax = _y * 2 + 1;
        var wk = new char[xmax, ymax];
        for (int y = 0; y < ymax; y++)
            for (int x = 0; x < xmax; x++)
                wk[x, y] = (y == 0 || x == 0 || y == ymax - 1 || x == xmax - 1) ? wall : (x % 2 == 0 && y % 2 == 0) ? pole : road;
        //
        return wk;
    }
    public string ToViewString()
    {
        var wk = "";
        for (int y = 0; y < grid.GetLength(1); y++)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                wk += grid[x, y];
            }
            wk += "\n";
        }
        return wk;
    }

}//class


public class Maze1 : MazeCore
{
    public Maze1(int x, int y, int seed = 1) : base(x, y, seed) {/**/}
    public MazeCore Run()
    {
        for (int y = 1; y < h; y++)
            for (int x = 1; x < w; x++)
                SetGrid(x, y, rand(2) == 0 ? N : W, wall);//SetGrid(1,1,'N','壁');
        return this;
        //
    }

}//class

public class Maze3 : MazeCore
{

    public Maze3(int x, int y, int seed = 1) : base(x, y, seed) {/**/}

    const int HORIZONTAL = 1;
    const int VERTICAL = 2;

    System.Collections.Generic.List<(int x,int y,char ch)> doors
     =new System.Collections.Generic.List<(int x, int y, char ch)>();

    public MazeCore Run()
    {
        int[,] igrid = new int[w, h];

        divide(igrid, 0, 0, w, h, choose_orientation(w, h));
        copy_grid(igrid);

        return this;
    }

    public int choose_orientation(int width, int height)
    {
        return width < height ? HORIZONTAL
         : height < width ? VERTICAL
         : rand(2) == 0 ? HORIZONTAL
         : VERTICAL
         ;
    }
    public void divide(int[,] grid, int x, int y, int width, int height, int orientation)
    {
        const int S = 1;
        const int E = 2;

        var bigroon = rand(10) == 0;
        if (bigroon && (width < 6 || height < 6)) return;
        if (width < 3 || height < 3) return;
        //if (width < 2 || height < 2) return;

        var horizontal = orientation == HORIZONTAL;

        //# where will the wall be drawn from?
        var wx = x + (horizontal ? 0 : rand(width - 2));
        var wy = y + (horizontal ? rand(height - 2) : 0);

        //# where will the passage through the wall exist?
        var px = wx + (horizontal ? rand(width) : 0);
        var py = wy + (horizontal ? 0 : rand(height));
        //grid[px,py]= horizontal ? DS : DE;///
        doors.Add( (px,py,horizontal ?this.S:this.E) );

        //# what direction will the wall be drawn?
        var dx = horizontal ? 1 : 0;
        var dy = horizontal ? 0 : 1;

        //# how long will the wall be?
        var length = horizontal ? width : height;

        //# what direction is perpendicular to the wall?
        var dir = horizontal ? S : E;

        for (var i = 0; i < length; i++)
        {
            if (wx.Between(0, this.w - 1) && wy.Between(0, this.h - 1))
                if (wx != px || wy != py)
                    grid[wx, wy] |= dir;//??????
            wx += dx;
            wy += dy;
        }

        var nx = x;
        var ny = y;
        var w = horizontal ? width : wx - x + 1;
        var h = horizontal ? wy - y + 1 : height;
        divide(grid, nx, ny, w, h, choose_orientation(w, h));
        //

        nx = horizontal ? x : wx + 1;
        ny = horizontal ? wy + 1 : y;
        w = horizontal ? width : x + width - wx - 1;
        h = horizontal ? y + height - wy - 1 : height;
        divide(grid, nx, ny, w, h, choose_orientation(w, h));

    }

    public void copy_grid(int[,] grid/*,char[,] cgrid*/)
    {
        const int S = 1;
        const int E = 2;

        var H = grid.GetLength(1);
        var W = grid.GetLength(0);
        //var ary = cgrid;

        for (var y = 0; y < H; y++)
            for (var x = 0; x < W; x++)
            {
                var cell = grid[x, y];
                //
                var bottom = y + 1 >= H;
                var south = ((cell & S) != 0 || bottom);
                var south2 = (x + 1 < W && (grid[x + 1, y] & S) != 0 || bottom);
                var east = ((cell & E) != 0 || x + 1 >= W);
                //
                /*
                int ax = x * 2 + 1, ay = y * 2 + 1;
                ary[ax, ay + 1] = south ? wall : road;
                if (east) ary[ax + 1, ay] = wall;
                else if (south && south2) ary[ax, ay + 1] = wall;
                */
                SetGrid(x,y,'S',south?wall:road);
                if(east) SetGrid(x,y,'E',wall);
                else if(south&&south2) SetGrid(x,y,'S',wall);
            }

        foreach(var wk in doors)
            SetGrid(wk.x,wk.y,wk.ch,door);

    }

}//class
