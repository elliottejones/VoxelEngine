using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelEngine
{
    internal class World
    {
        Dictionary<Vector3, Chunk> activeWorld = new Dictionary<Vector3, Chunk>();

        public void AddNewChunk(Vector3 location, Chunk chunk)
        {
            activeWorld.Add(location, chunk);
        }

        public void UpdateChunk(Vector3 location, Chunk chunk)
        {
            if (activeWorld.ContainsKey(location))
            {
                activeWorld[location] = chunk;
            }
            else
            {
                activeWorld.Add(location, chunk);
            }
        }
    }
}
