using System.Numerics;

namespace VoxelEngine
{
    internal class WorldGenerator
    {
        private readonly FastNoiseLite.FastNoiseLite _noise = new FastNoiseLite.FastNoiseLite();
        private readonly Dictionary<IntXZ, int[,]> _blockHeights = new Dictionary<IntXZ, int[,]>(); // KEY IS CHUNK XZ - stores the heights of the surface blocks for every chunk in the world

        private readonly int _chunkSize = Settings.CHUNK_SIZE;
        private readonly int _terrainMaxHeight = Settings.TERRAIN_MAX_HEIGHT;

        public WorldGenerator()
        {
            _noise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            _noise.SetFractalLacunarity(2);
            _noise.SetFractalOctaves(4);
            _noise.SetFrequency(0.005f);
        }

        public Chunk GenerateChunk(IntXYZ chunkCoords) // ensure chunkCoords are always integer values
        {
            int[,] globalHeights = new int[_chunkSize, _chunkSize];

            IntXZ flatReference = new IntXZ(chunkCoords.X, chunkCoords.Z); // 2D reference for the chunk coords

            if (!_blockHeights.TryGetValue(flatReference, out var _)) // checks if the heights for that column of chunks have already been generated
            {
                _blockHeights.Add(flatReference, GenerateSurfaceHeights(flatReference));
            }

            globalHeights = _blockHeights[flatReference];

            int[,] localHeights = CalculatelocalHeights(chunkCoords, globalHeights);

            return new Chunk(chunkCoords, GeneratePerBlock(localHeights), Globals.Statics.atlas); // generates the chunk and adds it to the chunk map
        }

        int[,] GenerateSurfaceHeights(IntXZ chunkCoords) // generates the surface heights for a vertical chunk strip
        {
            int[,] globalHeights = new int[_chunkSize, _chunkSize];

            for (int x = 0; x < _chunkSize; x++) // steps over each block in the chunk
            {
                for (int z = 0; z < _chunkSize; z++)
                {
                    // Adding 1 makes the value positive always, multiplying by 64 scales it from 0-128, floor division ensures that it is an integer height
                    globalHeights[x, z] = (int)Math.Floor((_noise.GetNoise(chunkCoords.X + x, chunkCoords.Z + z) + 1) * _terrainMaxHeight / 2);
                }
            }

            return globalHeights;
        }

        int[,] CalculatelocalHeights(IntXYZ chunkCoords, int[,] globalHeights)
        {
            int[,] localHeights = new int[_chunkSize, _chunkSize]; // local block data for this specific chunk

            for (int x = 0; x < _chunkSize; x++)
            {
                for (int z = 0; z < _chunkSize; z++)
                {
                    // finds local height within this specific chunk in 3D space
                    localHeights[x, z] = globalHeights[x, z] - (int)chunkCoords.Y * _chunkSize;
                }
            }

            return localHeights;
        }

        Block[,,] GeneratePerBlock(int[,] globalHeights) // generates the block types for each block in the chunk
        {
            Block[,,] chunkBlockData = new Block[_chunkSize, _chunkSize, _chunkSize]; 

            for (int x = 0; x < _chunkSize; x++)
            {
                for (int z = 0; z < _chunkSize; z++)
                {
                    int clampedHeight = Math.Clamp(globalHeights[x, z], 0, _chunkSize); // clamps the height to be between 0 and chunk height, to ensure no blocks are generated outside the chunk
                    for (int y = 0; y < globalHeights[x, z]; y++)
                    {
                        chunkBlockData[x, y, z] = new Block(BlockType.Stone); // sets the block type to stone - other blocks will be added later
                    }
                }
            }

            return chunkBlockData;
        }
    }
}
