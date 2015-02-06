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
    public class WorldGenThreadExecutor : BaseThread
    {
        private List<GeneratorThread> genThreads = new List<GeneratorThread>();
        private GraphicsDevice graphicsDevice;
        private Chunk[,] chunks;

        public WorldGenThreadExecutor(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            this.chunks = new Chunk[World.WORLD_WIDTH, World.WORLD_LENGTH];
        }

        public void setChunkList(Chunk[,] chunkList)
        {
            this.chunks = chunkList;
        }

        public override void RunThread()
        {
            foreach (Chunk chunk in chunks)
            {
                genThreads.Add(new GeneratorThread(this.graphicsDevice, chunk));
            }

            GeneratorThread currentThread;
            foreach (GeneratorThread generatorThread in genThreads)
            {
                currentThread = generatorThread;
                currentThread.Start();
                currentThread.Join();
            }

            Chunk[] surroundingChunks = new Chunk[4];
            for (int x = 0; x < chunks.GetLength(0); x++)
            {
                for (int z = 0; z < chunks.GetLength(1); z++)
                {
                    //Front Pass TEST
                    if (!(z - 1 < 0))
                    {
                        surroundingChunks[0] = chunks[x, z - 1];
                    }
                    
                    //Back Pass TEST
                    if (!((z + 1) > World.WORLD_LENGTH - 1))
                    {
                        surroundingChunks[1] = chunks[x, z + 1];
                    }

               
                    //Left
                    /*if (!(x - 1 < 0))
                    {
                        surroundingChunks[2] = chunks[x - 1, z];
                    }

                    //Right
                    if (!((x + 1) > World.WORLD_WIDTH - 1))
                    {
                        surroundingChunks[3] = chunks[x + 1, z];
                    }*/

                    chunks[x, z].removeBorderPolys(surroundingChunks);
                    surroundingChunks = new Chunk[4];
                }
            }
        }
    }
}
