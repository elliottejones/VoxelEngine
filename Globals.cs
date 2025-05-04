global using VoxelEngine.Globals;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VoxelEngine.Globals
{

    public static class Settings
    {
        public const int CHUNK_SIZE = 16; // size of cubic chunks
        public const int TERRAIN_MAX_HEIGHT = 128; // max height of terrain above y=0
    }

    public static class Statics
    {
        public static Texture2D atlas = Raylib.LoadTexture("TextureAtlas.png");
    }

    

    public enum BlockType
    {
        Air = 0,
        Grass = 1,
        Dirt = 2,
        Stone = 3,
        Water = 4,
        Lava = 5,
        Sand = 6,
        Wood = 7,
        Leaves = 8
    }
    public struct Block
    {
        public BlockType Type;

        public Block(BlockType type)
        {
            Type = type;
        }
    }

    // integer version of Vector2
    public struct IntXZ : IEquatable<IntXZ> //XZ plane is the flat one that will be used for terrain height
    {
        public int X;
        public int Z;

        public IntXZ(int x, int z)
        {
            this.X = x;
            this.Z = z;
        }

        public bool Equals(IntXZ other) => X == other.X && Z == other.Z;

        public override bool Equals(object? obj) =>
            obj is IntXZ other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(X, Z);

        public static bool operator ==(IntXZ left, IntXZ right) => left.Equals(right);
        public static bool operator !=(IntXZ left, IntXZ right) => !left.Equals(right);

        public static IntXZ operator -(IntXZ a, IntXZ b) => new IntXZ(a.X - b.X, a.Z - b.Z);

        public static IntXZ operator -(IntXZ a) => new IntXZ(-a.X, -a.Z); // unary negation

        public static readonly IntXZ UnitX = new IntXZ(1, 0);
        public static readonly IntXZ UnitZ = new IntXZ(0, 1);
        public static readonly IntXZ NegX = new IntXZ(-1, 0);
        public static readonly IntXZ NegZ = new IntXZ(0, -1);
    }

    public struct IntXYZ : IEquatable<IntXYZ> // integer vector3, for global chunk coordinates and local block coordinates

    {
        public int X;
        public int Y;
        public int Z;

        public IntXYZ(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public bool Equals(IntXYZ other) => X == other.X && Y == other.Y && Z == other.Z;

        public override bool Equals(object? obj) =>
            obj is IntXYZ other && Equals(other);
        public override int GetHashCode() =>
            HashCode.Combine(X, Z);

        public static bool operator ==(IntXYZ left, IntXYZ right) => left.Equals(right);
        public static bool operator !=(IntXYZ left, IntXYZ right) => !left.Equals(right);
        public static IntXYZ operator -(IntXYZ a, IntXYZ b)
        => new IntXYZ(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static IntXYZ operator -(IntXYZ a) // unary negation
            => new IntXYZ(-a.X, -a.Y, -a.Z);

        public static readonly IntXYZ UnitX = new IntXYZ(1, 0, 0);
        public static readonly IntXYZ UnitY = new IntXYZ(0, 1, 0);
        public static readonly IntXYZ UnitZ = new IntXYZ(0, 0, 1);
        public static readonly IntXYZ NegX = new IntXYZ(-1, 0, 0);
        public static readonly IntXYZ NegY = new IntXYZ(0, -1, 0);
        public static readonly IntXYZ NegZ = new IntXYZ(0, 0, -1);
    }
}
