using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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
    public class GeneratorThread : BaseThread
    {
        private GraphicsDevice graphicsDevice;

        private Chunk chunk;

        public bool isRunning = true;

        public GeneratorThread(GraphicsDevice graphicsDevice, Chunk chunk)
        {
            this.graphicsDevice = graphicsDevice;
            this.chunk = chunk;
        }

        public void GenerateTerrain()
        {
            this.RunThread();
        }

        public override void RunThread()
        {
            TerrainGenerator generator = new TerrainGenerator(chunk.seed);

            float amplitude = 0.20f; //0.20

            float multiplicator = 0.006f; //0.006

            for (int x = 0; x < chunk.cubes.GetLength(0); x++)
            {
                for (int y = 0; y < chunk.cubes.GetLength(1); y++)
                {
                    for (int z = 0; z < chunk.cubes.GetLength(2); z++)
                    {
                        Vector3 worldPosition = new Vector3(x + (chunk.position.X * Chunk.CHUNK_WIDTH), y, z + (chunk.position.Y * Chunk.CHUNK_LENGTH));
                        Vector3 multiplicatedVector = new Vector3(worldPosition.X * multiplicator, worldPosition.Y * (multiplicator / 2), worldPosition.Z * multiplicator);

                        float noiseValue = generator.getValue(multiplicatedVector) * amplitude;

                        if (noiseValue > worldPosition.Y * multiplicator && noiseValue < 1.0f)
                        {
                            if (y < 3)
                            {
                                chunk.cubes[x, y, z] = new Cube(CubeType.TYPE_SAND, worldPosition, new Vector3(x, y, z));
                            }
                            else
                            {
#if DEBUG
                                chunk.cubes[x, y, z] = new Cube(CubeType.TYPE_STONE, worldPosition, new Vector3(x, y, z));
#else
                                chunk.cubes[x, y, z] = new Cube(CubeType.TYPE_GRASS, worldPosition, new Vector3(x, y, z));
#endif
                            }
                        }
                        else
                        {
                            if (y == 0)
                            {
                                chunk.cubes[x, y, z] = new Cube(CubeType.TYPE_WATER, worldPosition, new Vector3(x, y, z));
                            }
                        }
                    }
                }
            }
#if DEBUG
            for (int x = 0; x < chunk.cubes.GetLength(0); x++)
            {
                for (int y = 0; y < chunk.cubes.GetLength(1); y++)
                {
                    for (int z = 0; z < chunk.cubes.GetLength(2); z++)
                    {
                        if (y < Chunk.CHUNK_HEIGHT - 1 || y == Chunk.CHUNK_HEIGHT - 1)
                        {
                            Cube currentCube = chunk.cubes[x, y, z];
                            if (y == Chunk.CHUNK_HEIGHT - 1)
                            {
                                if (currentCube != null)
                                {
                                    if (currentCube.getType() != CubeType.TYPE_AIR)
                                    {
                                        Vector3 worldPosition = new Vector3(x + (chunk.position.X * Chunk.CHUNK_WIDTH), y, z + (chunk.position.Y * Chunk.CHUNK_LENGTH));
                                        chunk.cubes[x, y, z] = new Cube(CubeType.TYPE_GRASS, worldPosition, new Vector3(x, y, z));
                                    }
                                }
                            }
                            else
                            {
                                if (y != 0)
                                {
                                    Cube above = chunk.cubes[x, y + 1, z];
                                    Cube below = chunk.cubes[x, y - 1, z];
                                    Vector3 worldPosition = new Vector3(x + (chunk.position.X * Chunk.CHUNK_WIDTH), y, z + (chunk.position.Y * Chunk.CHUNK_LENGTH));
                                    if (currentCube != null)
                                    {
                                        if (currentCube.getType() != CubeType.TYPE_AIR)
                                        {
                                            if (above == null || above.getType() == CubeType.TYPE_AIR)
                                            {
                                                if (below != null)
                                                {
                                                    if (below.getType() != CubeType.TYPE_AIR)
                                                    {
                                                        chunk.cubes[x, y, z] = new Cube(CubeType.TYPE_GRASS, worldPosition, new Vector3(x, y, z));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
#endif
                
            chunk.initRenderer(this.graphicsDevice);

            /*Noise2D generator = new Noise2D(chunk.seed);

            float amplitude = 0.0f;
            float multiplicator = 0.0f;

            Cube cube;
            float noiseValue;

            for (int x = 0; x < chunk.cubes.GetLength(0); x++)
            {
                for (int z = 0; z < chunk.cubes.GetLength(2); z++)
                {
                    cube = chunk.cubes[x, 0, z];
                    Vector2 cubeWorldPosition = new Vector2(cube.WorldPosition.X, cube.WorldPosition.Z);

                    noiseValue = generator.getValue(cubeWorldPosition);

                    Debug.WriteLine(noiseValue);
                }
            }*/
        }
    }
}
