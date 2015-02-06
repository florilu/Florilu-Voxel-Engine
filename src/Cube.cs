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
    public enum CubeType
    {
        TYPE_AIR = 1,
        TYPE_DIRT = 2,
        TYPE_GRASS = 3,
        TYPE_SAND = 4,
        TYPE_WATER = 5,
        TYPE_STONE = 6
    }

    public class Cube
    {
        private CubeType type;
        public Vector3 position { private set; get; }
        public Vector3 WorldPosition;
        public Vector3 ChunkPosition;

        public Cube(CubeType cubeType, Vector3 position, Vector3 chunkPosition)
        {
            this.type = cubeType;
            this.WorldPosition = position;
            this.ChunkPosition = chunkPosition;
        }

        public CubeType getType()
        {
            return this.type;
        }
    }
}
