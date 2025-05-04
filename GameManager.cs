using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Raylib_cs.Raylib;
using Raylib_cs;

namespace VoxelEngine
{
    internal class GameManager
    {
        public GameManager()
        {
            Initialize();
        }

        public void Initialize()
        {
            InitWindow(1920, 1080, "Konercraft");

            while (!WindowShouldClose())
            {
                
            }

            CloseWindow();
        }
    }
}
