using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Uni3D;
using static Uni3D.Maker;
public class loadtex : MonoBehaviour
{
    [SerializeField] Texture2D tex;
    // Start is called before the first frame update
    void Start()
    {

        if (tex == null) tex = ftex("#789");
        var me = gameObject;

        var maze = new Maze3(20, 20, 1);
        maze.Run();

        for (var y = 0; y < maze.h; y++)
            for (var x = 0; x < maze.w; x++)
            {

                QuadMaze.make('G', tex, me).Move(0, x, -1 * y);
                {
                    var ch = MazeCore.wall;
                    if (maze.GetGrid(x, y, 'N') == ch)
                        QuadMaze.make('N', tex, me).Move(0, x, -1 * y);
                    if (maze.GetGrid(x, y, 'E') == ch)
                        QuadMaze.make('E', tex, me).Move(0, x, -1 * y);
                    if (maze.GetGrid(x, y, 'W') == ch)
                        QuadMaze.make('W', tex, me).Move(0, x, -1 * y);
                    if (maze.GetGrid(x, y, 'S') == ch)
                        QuadMaze.make('S', tex, me).Move(0, x, -1 * y);
                }
                {
                    var tex=ftex("#000a");
                    var ch = MazeCore.door;
                    if (maze.GetGrid(x, y, 'N') == ch)
                        QuadMaze.make('N', tex, me,false).Move(0, x, -1 * y);
                    if (maze.GetGrid(x, y, 'E') == ch)
                        QuadMaze.make('E', tex, me, false).Move(0, x, -1 * y);
                    if (maze.GetGrid(x, y, 'W') == ch)
                        QuadMaze.make('W', tex, me, false).Move(0, x, -1 * y);
                    if (maze.GetGrid(x, y, 'S') == ch)
                        QuadMaze.make('S', tex, me, false).Move(0, x, -1 * y);
                }

                QuadMaze.make('C', ftex("#000"), me).Move(0, x, -1 * y);

            }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
