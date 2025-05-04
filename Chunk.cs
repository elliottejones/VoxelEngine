using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelEngine
{
    internal class Chunk
    {
        public readonly IntXYZ ChunkCoordinates; // chunk coords are always integer values
        public Block[,,] ChunkData = new Block[16, 16, 16]; // smart 16x16x16 3D chunks :)

        private bool _dirty = true; // do we need to rebuild?
        private Mesh _mesh;
        private Material _material;

        public Chunk(IntXYZ chunkCoordinates, Block[,,] chunkData, Texture2D atlas)
        {
            ChunkData = chunkData;
            ChunkCoordinates = chunkCoordinates;
            _dirty = true;

            _material = Raylib.LoadMaterialDefault();
            // Map your atlas into the material’s albedo (diffuse) slot:
            Raylib.SetMaterialTexture(
                ref _material,
                MaterialMapIndex.Albedo, // or .MATERIAL_MAP_DIFFUSE in older bindings
                atlas
            );
        }

        unsafe public void BuildMesh()
        {
            if (!_dirty) return;

            // generate the raw vertex data
            var (verts, normals, uvs, indices) = TerrainMeshBuilder.Build(ChunkData, ChunkCoordinates);

            var vertsArray = verts.SelectMany(v => new[] { v.X, v.Y, v.Z }).ToArray();
            var normsArray = normals.SelectMany(n => new[] { n.X, n.Y, n.Z }).ToArray();
            var uvsArray = uvs.SelectMany(uv => new[] { uv.X, uv.Y }).ToArray();
            var idxArrayUInt = indices.Select(i => (ushort)i).ToArray();
                                                                        
            fixed (float* vertsPtr = vertsArray)
            fixed (float* normsPtr = normsArray)
            fixed (float* uvsPtr = uvsArray)
            fixed (ushort* idxPtrUInt = idxArrayUInt)
            {
                _mesh.VertexCount = vertsArray.Length / 3;
                _mesh.Vertices = vertsPtr;
                _mesh.Normals = normsPtr;
                _mesh.TexCoords = uvsPtr;
                _mesh.Indices = idxPtrUInt;
            }

            // 3) Upload to GPU and create a VAO
            Raylib.UploadMesh(ref _mesh, false);

            _dirty = false;
        }

        public void Draw(Camera3D cam, Texture2D atlas)
        {
            // Ensure mesh is built
            BuildMesh();
            Raylib.BeginMode3D(cam);
            Raylib.DrawMesh(_mesh, _material, Matrix4x4.Identity);
            Raylib.EndMode3D();
        }


    }
}
