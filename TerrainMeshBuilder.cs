using Raylib_cs;
using System.Numerics;

namespace VoxelEngine
{
    public static class TerrainMeshBuilder
    {

        private static TextureAtlas _atlas = new TextureAtlas("TextureAtlas.png", 32, 8, 8);

        static readonly Dictionary<IntXYZ, Vector3[]> FaceCorners = new()
        {
          { new IntXYZ( 1,  0,  0), new[]{
              new Vector3(1,0,0), new Vector3(1,1,0),
              new Vector3(1,1,1), new Vector3(1,0,1)
            }
          },
          { new IntXYZ(-1,  0,  0), new[]{
              new Vector3(0,0,1), new Vector3(0,1,1),
              new Vector3(0,1,0), new Vector3(0,0,0)
            }
          },
          { new IntXYZ( 0,  1,  0), new[]{
              new Vector3(0,1,1), new Vector3(1,1,1),
              new Vector3(1,1,0), new Vector3(0,1,0)
            }
          },
          { new IntXYZ( 0, -1,  0), new[]{
              new Vector3(0,0,0), new Vector3(1,0,0),
              new Vector3(1,0,1), new Vector3(0,0,1)
            }
          },
          { new IntXYZ( 0,  0,  1), new[]{
              new Vector3(1,0,1), new Vector3(1,1,1),
              new Vector3(0,1,1), new Vector3(0,0,1)
            }
          },
          { new IntXYZ( 0,  0, -1), new[]{
              new Vector3(0,0,0), new Vector3(0,1,0),
              new Vector3(1,1,0), new Vector3(1,0,0)
            }
          },
        };

        public static readonly IntXYZ[] Directions = new IntXYZ[]
        {
            new IntXYZ( 1,  0,  0),  // +X 
            new IntXYZ(-1,  0,  0),  // -X
            new IntXYZ( 0,  1,  0),  // +Y 
            new IntXYZ( 0, -1,  0),  // -Y 
            new IntXYZ( 0,  0,  1),  // +Z
            new IntXYZ( 0,  0, -1),  // -Z 
        };

        public static (List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> indices) Build(Block[,,] blocks, IntXYZ chunkPos)
        {
            var verts = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var idx = new List<int>();
            int nextIndex = 0;

            // for each block in the 16^3 chunk
            for (int x = 0; x < Settings.CHUNK_SIZE; x++)
                for (int y = 0; y < Settings.CHUNK_SIZE; y++)
                    for (int z = 0; z < Settings.CHUNK_SIZE; z++)
                    {
                        var block = blocks[x, y, z];
                        if (block.Type == BlockType.Air) continue;

                        // world space position of this block’s corner
                        var worldBase = new Vector3(
                            chunkPos.X * Settings.CHUNK_SIZE + x,
                            chunkPos.Y * Settings.CHUNK_SIZE + y,
                            chunkPos.Z * Settings.CHUNK_SIZE + z
                        );

                        // for each of the 6 directions
                        foreach (var dir in Directions)
                        {
                            int nx = x + dir.X, ny = y + dir.Y, nz = z + dir.Z;
                            bool neighborSolid =
                                nx >= 0 && nx < Settings.CHUNK_SIZE &&
                                ny >= 0 && ny < Settings.CHUNK_SIZE &&
                                nz >= 0 && nz < Settings.CHUNK_SIZE
                                  ? blocks[nx, ny, nz].Type != BlockType.Air
                                  : false;  // treat out of bounds as air

                            if (neighborSolid) continue;  // face is hidden

                            // emit that face
                            EmitFace(
                              worldBase, dir,
                              block.Type,
                              verts, normals, uvs, idx,
                              ref nextIndex
                            );
                        }
                    }

            return (verts, normals, uvs, idx);
        }

        static void EmitFace(Vector3 worldBase, IntXYZ dir, BlockType type, List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> indices, ref int nextIndex)
        {
            // get the 4 corner offsets for this face
            var corners = FaceCorners[dir];
            var normal = new Vector3(dir.X, dir.Y, dir.Z);

            // lookup UVs for this blocktype the atlas
            var faceUVs = _atlas.GetUVs(type, dir);

            // add 4 verts
            for (int i = 0; i < 4; i++)
            {
                verts.Add(worldBase + corners[i]);
                normals.Add(normal);
                uvs.Add(faceUVs[i]);
            }

            // creates the two triangles for this face
            indices.Add(nextIndex + 0);
            indices.Add(nextIndex + 1);
            indices.Add(nextIndex + 2);
            indices.Add(nextIndex + 0);
            indices.Add(nextIndex + 2);
            indices.Add(nextIndex + 3);

            // increment the index for the next face
            nextIndex += 4;
        }
    }
}
