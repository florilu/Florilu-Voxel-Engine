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
    public class World
    {
        public static readonly int WORLD_WIDTH = 20;
        public static readonly int WORLD_LENGTH = 20;

        public Chunk[,] chunks;

        public World()
        {
            chunks = new Chunk[WORLD_WIDTH, WORLD_LENGTH];
        }

        public void setBlockAt(Vector3 worldCubePosition, CubeType type)
        {
            Vector3 chunkPosition = new Vector3((float)((int)worldCubePosition.X / (int)Chunk.CHUNK_WIDTH), 0, (float)((int)worldCubePosition.Z / (int)Chunk.CHUNK_LENGTH)); //1, 0, 0
            Vector3 chunkCubePosition = new Vector3((int)worldCubePosition.X % Chunk.CHUNK_WIDTH, worldCubePosition.Y, (int)worldCubePosition.Z % Chunk.CHUNK_LENGTH); //1, 16, 5

            Debug.WriteLine(chunkCubePosition);
            Debug.WriteLine(chunkPosition);

            if (chunkPosition.X < WORLD_WIDTH && chunkPosition.Z < WORLD_LENGTH)
            {
                chunks[(int)chunkPosition.X, (int)chunkPosition.Z].setBlock(chunkCubePosition, type);
            }
        }
    }
}
