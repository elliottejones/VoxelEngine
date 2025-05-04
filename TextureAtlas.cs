using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelEngine
{
    internal class TextureAtlas
    {
        private struct TileSet { public int TopX, TopY, SideX, SideY, BotX, BotY; }

        static readonly Dictionary<BlockType, (int topX, int topY, int sideX, int sideY, int botX, int botY)> TileCoords = new Dictionary<BlockType, (int topX, int topY, int sideX, int sideY, int botX, int botY)>
        {
            [BlockType.Grass] = (topX: 0, topY: 0, sideX: 1, sideY: 0, botX: 2, botY: 0),
            [BlockType.Dirt] = (topX: 2, topY: 0, sideX: 2, sideY: 0, botX: 2, botY: 0),
            [BlockType.Stone] = (topX: 3, topY: 0, sideX: 3, sideY: 0, botX: 3, botY: 0),
        };

        private readonly Texture2D _atlasTexture;
        private readonly int _tilePx, _tilesX, _tilesY;
        private readonly Dictionary<BlockType, TileSet> _tileMap;

        public TextureAtlas(string atlasFile, int tilePx, int tilesX, int tilesY)
        {
            _atlasTexture = Raylib.LoadTexture(atlasFile);
            _tilePx = tilePx;
            _tilesX = tilesX;
            _tilesY = tilesY;

            _tileMap = new Dictionary<BlockType, TileSet>
            {
                [BlockType.Grass] = new TileSet { TopX = 0, TopY = 0, SideX = 1, SideY = 0, BotX = 2, BotY = 0 },
                [BlockType.Dirt] = new TileSet { TopX = 2, TopY = 0, SideX = 2, SideY = 0, BotX = 2, BotY = 0 },
                [BlockType.Stone] = new TileSet { TopX = 3, TopY = 0, SideX = 3, SideY = 0, BotX = 3, BotY = 0 },
            };
        }

        public Vector2[] GetUVs(BlockType type, IntXYZ faceDir)
        {
            var t = _tileMap[type];
            // pick coords based on faceDir
            (int tx, int ty) = faceDir switch
            {
                var d when d == IntXYZ.UnitY => (t.TopX, t.TopY),
                var d when d == -IntXYZ.UnitY => (t.BotX, t.BotY),
                _ => (t.SideX, t.SideY)
            };

            float u0 = tx * _tilePx / (float)(_tilesX * _tilePx);
            float v0 = ty * _tilePx / (float)(_tilesY * _tilePx);
            float u1 = (tx + 1) * _tilePx / (float)(_tilesX * _tilePx);
            float v1 = (ty + 1) * _tilePx / (float)(_tilesY * _tilePx);

            // Raylib UV origin is bottom‐left
            return new[]
            {
            new Vector2(u0, v1),
            new Vector2(u0, v0),
            new Vector2(u1, v0),
            new Vector2(u1, v1),
            };
        }

        public Texture2D Texture => _atlasTexture;
    }
}
