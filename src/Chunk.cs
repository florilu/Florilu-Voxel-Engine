using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

/*
 * Florilu Voxel Engine
 * This is a voxel engine thought as an easy to use framework.
 * Copyright (C) 2014/2015  Florian Ludewig
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * Contact: florianludewig@gmx.de
 */

namespace _3D_Tests
{
    public class Chunk
    {
        public static readonly int CHUNK_WIDTH = 15;
        public static readonly int CHUNK_HEIGHT = 64;
        public static readonly int CHUNK_LENGTH = 15;

        private ChunkRenderer renderer;

        public Cube[, ,] cubes;

        public List<Vector3> cubesSurrounding;
        private List<Cube> changedCubes;

        public bool isDirty = true;

        public Vector2 position;

        public int seed;

        public Chunk(Vector2 position, int seed)
        {
            this.position = position;
            this.cubes = new Cube[CHUNK_WIDTH, CHUNK_HEIGHT, CHUNK_LENGTH];

            this.changedCubes = new List<Cube>();
            this.cubesSurrounding = new List<Vector3>();

            this.seed = seed;
        }

        public void initRenderer(GraphicsDevice graphicsDevice)
        {
            if (this.renderer == null)
            {
                this.renderer = new ChunkRenderer(graphicsDevice, this);
            }

            this.renderer.createVertices();
        }

        public void initRenderer(GraphicsDevice graphicsDevice, bool testVertices)
        {
            if (this.renderer == null)
            {
                this.renderer = new ChunkRenderer(graphicsDevice, this);
            }

            if (testVertices)
            {
                this.renderer.createTestVertices();
            }
            else
            {
                this.renderer.createVertices();
            }
        }

        public void removeBorderPolys(Chunk[] surroundingChunks)
        {
            this.renderer.removeBorderPolys(surroundingChunks);
        }

        public void setBlock(Vector3 cubePosition, CubeType type)
        {
            Vector3 transformedPosition = new Vector3(cubePosition.X + (this.position.X * CHUNK_WIDTH), cubePosition.Y, cubePosition.Z + (this.position.Y * CHUNK_LENGTH));

            cubes[(int)cubePosition.X, (int)cubePosition.Y, (int)cubePosition.Z] = new Cube(type, transformedPosition, cubePosition);

            changedCubes.Add(new Cube(type, transformedPosition, cubePosition));

            getCubesSurrounding();

            this.isDirty = true;
        }

        public void getCubesSurrounding()
        {
            if (changedCubes.Count > 0)
            {
                Cube nextCube = null;

                for (int i = 0; i < changedCubes.Count; i++)
                {
                    Vector3 cubePosition = changedCubes[i].position;
                    int x = (int)cubePosition.X;
                    int y = (int)cubePosition.Y;
                    int z = (int)cubePosition.Z;

                    if (!((x - 1) < 0) || x == 0)
                    {
                        nextCube = cubes[x - 1, y, z];
                        cubesSurrounding.Add(nextCube.position);
                    }
                    nextCube = null;
                    if (!((x + 1) >= Chunk.CHUNK_WIDTH) || x == Chunk.CHUNK_WIDTH - 1)
                    {
                        nextCube = cubes[x + 1, y, z];
                        cubesSurrounding.Add(nextCube.position);
                    }
                    nextCube = null;
                    if (!((y - 1) < 0) || y == 0)
                    {
                        nextCube = cubes[x, y - 1, z];
                        cubesSurrounding.Add(nextCube.position);
                    }
                    nextCube = null;
                    if (!((y + 1) >= Chunk.CHUNK_HEIGHT) || y == Chunk.CHUNK_HEIGHT - 1)
                    {
                        nextCube = cubes[x, y + 1, z];
                        cubesSurrounding.Add(nextCube.position);
                    }
                    nextCube = null;
                    if (!((z - 1) < 0) || z == 0)
                    {
                        nextCube = cubes[x, y, z - 1];
                        cubesSurrounding.Add(nextCube.position);
                    }
                    nextCube = null;
                    if (!((z + 1) >= Chunk.CHUNK_LENGTH) || z == Chunk.CHUNK_LENGTH - 1)
                    {
                        nextCube = cubes[x, y, z + 1];
                        cubesSurrounding.Add(nextCube.position);
                    }
                    nextCube = null;
                }
                this.changedCubes.Clear();
            }
        }

        public void draw(GameTime gameTime, Camera camera, BasicEffect effect)
        
        {
            if (this.renderer != null)
            {
                this.renderer.draw(gameTime, camera, effect);
            }
        }

        public void buildChunk(GraphicsDevice graphicsDevice)
        {
            GeneratorThread thread = new GeneratorThread(graphicsDevice, this);
            thread.Start();
        }
    }
}
