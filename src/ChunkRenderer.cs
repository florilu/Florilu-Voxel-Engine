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

using System.Diagnostics;

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
    public class ChunkRenderer
    {
        private List<VertexPositionColor> vertexList;

        private VertexBuffer vertexBuffer;

        private GraphicsDevice graphicsDevice;

        private Chunk chunk;

        private Random random = new Random();

        private static Vector3 normalFront = new Vector3(0.0f, 0.0f, -1.0f);
        private static Vector3 normalBack = new Vector3(0.0f, 0.0f, 1.0f);
        private static Vector3 normalTop = new Vector3(0.0f, 1.0f, 0.0f);
        private static Vector3 normalBottom = new Vector3(0.0f, -1.0f, 0.0f);
        private static Vector3 normalLeft = new Vector3(-1.0f, 0.0f, 0.0f);
        private static Vector3 normalRight = new Vector3(1.0f, 0.0f, 0.0f);

        private static Vector3[] normals = { normalFront, normalBack, normalTop, normalBottom, normalLeft, normalRight };

        private VertexPositionColor[] vertexArray;

#if DEBUG
        private bool usedForRemoving = false;
#endif
   
        public ChunkRenderer(GraphicsDevice graphicsDevice, Chunk chunk)
        {
            this.chunk = chunk;

            this.vertexList = new List<VertexPositionColor>();
            this.graphicsDevice = graphicsDevice;
        }


        /// <summary>
        /// Can cause memory overflow!
        /// Test Method.
        /// </summary>
        public void createTestVertices()
        {
            vertexList.Clear();

            Cube cube;

            for (int x = 0; x < chunk.cubes.GetLength(0); x++)
            {
                for (int y = 0; y < chunk.cubes.GetLength(1); y++)
                {
                    for (int z = 0; z < chunk.cubes.GetLength(2); z++)
                    {
                        cube = chunk.cubes[x, y, z];

                        foreach (Vector3 normal in normals)
                        {
                            addVertexes(cube.WorldPosition, normal, cube.getType());
                        }
                    }
                }
            }

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), this.vertexList.Count, BufferUsage.WriteOnly);
            VertexPositionColor[] vertexArray = this.vertexList.ToArray();
            vertexBuffer.SetData(vertexArray);
            chunk.isDirty = false;
        }

        public void createVertices()
        {
            vertexList.Clear();

            Cube nextCube = null;

            for (int x = 0; x < chunk.cubes.GetLength(0); x++)
            {
                for (int y = 0; y < chunk.cubes.GetLength(1); y++)
                {
                    for (int z = 0; z < chunk.cubes.GetLength(2); z++)
                    {
                        Cube cube = chunk.cubes[x, y, z];

                        if (cube != null)
                        {
                            if (cube.getType() != CubeType.TYPE_AIR)
                            {
                                createCubeVertices(cube, nextCube, x, y, z);
                            }
                        }
                    }
                }
            }

            if (this.vertexList.Count > 0)
            {
                vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), this.vertexList.Count, BufferUsage.WriteOnly);
                VertexPositionColor[] vertexArray = this.vertexList.ToArray();
                vertexBuffer.SetData(vertexArray);
            }
            
            chunk.isDirty = false;
        }

        public void removeBorderPolys(Chunk[] surroundingChunks)
        {
            this.vertexList.Clear();
            
            Cube nextCube = null;

            bool frontFace = true;
            bool backFace = true;
            bool leftFace = true;
            bool rightFace = true;
            bool topFace = true;
            bool bottomFace = true;

            Cube currentCube;
            Cube cubeInOtherChunk;

            for (int x = 0; x < chunk.cubes.GetLength(0); x++)
            {
                for (int y = 0; y < chunk.cubes.GetLength(1); y++)
                {
                    for (int z = 0; z < chunk.cubes.GetLength(2); z++)
                    {
                        currentCube = chunk.cubes[x, y, z];

                        if (currentCube != null)
                        {
                            if (currentCube.getType() != CubeType.TYPE_AIR)
                            {
                                //Front
                                if (surroundingChunks[0] != null)
                                {
                                    if (z == 0)
                                    {
                                        cubeInOtherChunk = surroundingChunks[0].cubes[x, y, Chunk.CHUNK_LENGTH - 1];
                                        if (cubeInOtherChunk != null)
                                        {
                                            if (cubeInOtherChunk.getType() != CubeType.TYPE_AIR)
                                            {
                                                frontFace = false;
                                            }

                                            /*if (frontFace != false)
                                            {
                                                checkAndBuildFrontFace(currentCube, nextCube, x, y, z);
                                            }*/
                                        #if DEBUG
                                            this.usedForRemoving = true;
                                        #endif
                                        }
                                    }
                                }
                                //Back
                                if (surroundingChunks[1] != null)
                                {
                                    if (z == Chunk.CHUNK_LENGTH - 1)
                                    {
                                        cubeInOtherChunk = surroundingChunks[1].cubes[x, y, 0];
                                        if (cubeInOtherChunk.getType() != CubeType.TYPE_AIR)
                                        {
                                            backFace = false;
                                        }

                                        /*if (backFace != false)
                                        {
                                            checkAndBuildBackFace(currentCube, nextCube, x, y, z);
                                        }*/
                                    #if DEBUG
                                        this.usedForRemoving = true;
                                    #endif
                                    }
                                }
                            }
                            
                            
                            /*
                            //Left
                            if (surroundingChunks[2] != null)
                            {
                                if (x == 0)
                                {
                                    cubeInOtherChunk = surroundingChunks[2].cubes[Chunk.CHUNK_WIDTH - 1, y, z];
                                    if (cubeInOtherChunk.getType() != CubeType.TYPE_AIR)
                                    {
                                        leftFace = false;
                                    }

                                    if (leftFace != false)
                                    {
                                        checkAndBuildLeftFace(currentCube, nextCube, x, y, z);
                                    }
                                }
                            }
                            //Right
                            if (surroundingChunks[3] != null)
                            {
                                if (x == Chunk.CHUNK_WIDTH - 1)
                                {
                                    cubeInOtherChunk = surroundingChunks[3].cubes[0, y, z];
                                    if (cubeInOtherChunk.getType() != CubeType.TYPE_AIR)
                                    {
                                        rightFace = false;
                                    }

                                    if(rightFace != false)
                                    {
                                        checkAndBuildRightFace(currentCube, nextCube, x, y, z);
                                    }
                                }
                            }
                            //Bottom
                            if (!(y - 1 < 0))
                            {
                                if (chunk.cubes[x, y - 1, z].getType() != CubeType.TYPE_AIR)
                                {
                                    bottomFace = false;
                                }

                                if (bottomFace != false)
                                {
                                    checkAndBuildBottomFace(currentCube, nextCube, x, y, z);
                                }
                            }
                            //Top
                            if(!(y + 1 > (Chunk.CHUNK_HEIGHT - 1)))
                            {
                                if (chunk.cubes[x, y + 1, z].getType() != CubeType.TYPE_AIR)
                                {
                                    topFace = false;
                                }

                                if (topFace != false)
                                {
                                    checkAndBuildTopFace(currentCube, nextCube, x, y, z);
                                }
                            }*/

                            if (leftFace && rightFace && backFace && frontFace)
                            {
                                if (currentCube.getType() != CubeType.TYPE_AIR)
                                {
                                    createCubeVertices(currentCube, nextCube, x, y, z);
                                }
                            }
                            else
                            {
                                createCubeFaceVertices(currentCube, frontFace, backFace, leftFace, rightFace, topFace, bottomFace);
                            }
                        }

                        leftFace = true;
                        rightFace = true;
                        frontFace = true;
                        backFace = true;
                        topFace = true;
                        bottomFace = true;
                    }
                }
            }

            if (this.vertexList.Count > 0)
            {
                vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), this.vertexList.Count, BufferUsage.WriteOnly);
                VertexPositionColor[] vertexArray = this.vertexList.ToArray();
                vertexBuffer.SetData(vertexArray);
            }
        }

        private void createCubeFaceVertices(Cube cube, bool front, bool back, bool left, bool right, bool top, bool bottom)
        {
            if (front)
            {
                addVertexes(cube.WorldPosition, normalFront, cube.getType());
            }
            if (back)
            {
                addVertexes(cube.WorldPosition, normalBack, cube.getType());
            }
            if (left)
            {
                addVertexes(cube.WorldPosition, normalLeft, cube.getType());
            }
            if (right)
            {
                addVertexes(cube.WorldPosition, normalRight, cube.getType());
            }
            if (top)
            {
                addVertexes(cube.WorldPosition, normalTop, cube.getType());
            }
            if (bottom)
            {
                addVertexes(cube.WorldPosition, normalBottom, cube.getType());
            }
        }

        private void checkAndBuildFrontFace(Cube cube, Cube nextCube, int x, int y, int z)
        {
            if (!((z - 1) < 0) || z == 0)
            {
                if (z != 0)
                {
                    nextCube = chunk.cubes[x, y, z - 1];
                }
                if (nextCube == null || nextCube.getType() == CubeType.TYPE_AIR)
                {
                    addVertexes(cube.WorldPosition, normalFront, cube.getType());
                }
            }
            nextCube = null;
        }

        private void checkAndBuildBackFace(Cube cube, Cube nextCube, int x, int y, int z)
        {
            if (!((z + 1) >= Chunk.CHUNK_LENGTH) || z == Chunk.CHUNK_LENGTH - 1)
            {
                if (z != Chunk.CHUNK_LENGTH - 1)
                {
                    nextCube = chunk.cubes[x, y, z + 1];
                }
                if (nextCube == null || nextCube.getType() == CubeType.TYPE_AIR)
                {
                    addVertexes(cube.WorldPosition, normalBack, cube.getType());
                }
            }
            nextCube = null;
        }

        private void checkAndBuildLeftFace(Cube cube, Cube nextCube, int x, int y, int z)
        {
            if (!((x - 1) < 0) || x == 0)
            {
                if (x != 0)
                {
                    nextCube = chunk.cubes[x - 1, y, z];
                }
                if (nextCube == null || nextCube.getType() == CubeType.TYPE_AIR)
                {
                    addVertexes(cube.WorldPosition, normalLeft, cube.getType());
                }
            }
        }

        private void checkAndBuildRightFace(Cube cube, Cube nextCube, int x, int y, int z)
        {
            if (!((x + 1) >= Chunk.CHUNK_WIDTH) || x == Chunk.CHUNK_WIDTH - 1)
            {
                if (x != Chunk.CHUNK_WIDTH - 1)
                {
                    nextCube = chunk.cubes[x + 1, y, z];
                }
                if (nextCube == null || nextCube.getType() == CubeType.TYPE_AIR)
                {
                    addVertexes(cube.WorldPosition, normalRight, cube.getType());
                }
            }
        }

        private void checkAndBuildTopFace(Cube cube, Cube nextCube, int x, int y, int z)
        {
            if (!((y - 1) < 0) || y == 0)
            {
                if (y != 0)
                {
                    nextCube = chunk.cubes[x, y - 1, z];
                }
                if (nextCube == null || nextCube.getType() == CubeType.TYPE_AIR)
                {
                    addVertexes(cube.WorldPosition, normalBottom, cube.getType());
                }
            }
            nextCube = null;
        }

        private void checkAndBuildBottomFace(Cube cube, Cube nextCube, int x, int y, int z)
        {
            if (!((y + 1) >= Chunk.CHUNK_HEIGHT) || y == Chunk.CHUNK_HEIGHT - 1)
            {
                if (y != Chunk.CHUNK_HEIGHT - 1)
                {
                    nextCube = chunk.cubes[x, y + 1, z];
                }
                if (nextCube == null || nextCube.getType() == CubeType.TYPE_AIR)
                {
                    addVertexes(cube.WorldPosition, normalTop, cube.getType());
                }
            }
            nextCube = null;
        }

        private void createCubeVertices(Cube cube, Cube nextCube, int x, int y, int z)
        {
            if (cube.getType() != CubeType.TYPE_WATER) 
            {
                //Front
                checkAndBuildFrontFace(cube, nextCube, x, y, z);
                //Back
                checkAndBuildBackFace(cube, nextCube, x, y, z);
                //Left
                checkAndBuildLeftFace(cube, nextCube, x, y, z);
                //Right
                checkAndBuildRightFace(cube, nextCube, x, y, z);
            }
            //Bottom
            checkAndBuildBottomFace(cube, nextCube, x, y, z);
            //Top
            checkAndBuildTopFace(cube, nextCube, x, y, z);
        }


        private void addVertexes(Vector3 position, Vector3 normal, CubeType type)
        {
            Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
            Vector3 side2 = Vector3.Cross(normal, side1);

            Color color = Color.White;
            Color greenColor = Color.ForestGreen;
            Color dirtColor = Color.SandyBrown; //244 164 96

#if DEBUG
            if (!usedForRemoving)
            {
#endif
                switch (type)
                {
                    case CubeType.TYPE_DIRT:
                        color = this.getDirtColor();
                        break;
                    case CubeType.TYPE_GRASS:
                        if (normal == normalTop)
                        {
                            color = this.getGrassColor();
                        }
                        else
                        {
                            color = this.getDirtColor();
                        }
                        break;
                    case CubeType.TYPE_SAND:
                        color = Color.Khaki;
                        break;
                    case CubeType.TYPE_WATER:
                        color = new Color(Color.Blue, 50f);
                        break;
                    case CubeType.TYPE_STONE:
                        color = Color.DarkGray;
                        break;
                }
#if DEBUG
            }
            else
            {
                color = Color.Pink;
            }
#endif
            vertexList.Add(new VertexPositionColor(position + ((normal - side1 - side2) * 1.0f / 2), color));
            vertexList.Add(new VertexPositionColor(position + ((normal - side1 + side2) * 1.0f / 2), color));
            vertexList.Add(new VertexPositionColor(position + ((normal + side1 + side2) * 1.0f / 2), color));
            vertexList.Add(new VertexPositionColor(position + ((normal - side1 - side2) * 1.0f / 2), color));
            vertexList.Add(new VertexPositionColor(position + ((normal + side1 + side2) * 1.0f / 2), color));
            vertexList.Add(new VertexPositionColor(position + ((normal + side1 - side2) * 1.0f / 2), color));
        }

        private Color getDirtColor()
        {
            int randomNumber = random.Next(4);
            Color dirtColor = new Color();
            switch (randomNumber)
            {
                case 0:
                    dirtColor.R = 240;
                    dirtColor.G = 160;
                    dirtColor.B = 93;
                    break;
                case 1:
                    dirtColor.R = 246;
                    dirtColor.G = 167;
                    dirtColor.B = 97;
                    break;
                case 2:
                    dirtColor.R = 242;
                    dirtColor.G = 162;
                    dirtColor.B = 96;
                    break;
                case 3:
                    dirtColor.R = 242;
                    dirtColor.G = 165;
                    dirtColor.B = 93;
                    break;
            }
            return dirtColor;
        }

        private Color getGrassColor()
        {
            int randomNumber = random.Next(6);
            Color greenColor = Color.ForestGreen;
            switch (randomNumber)
            {
                case 0:
                    greenColor.G = 142;
                    break;
                case 1:
                    greenColor.G = 135;
                    break;
                case 2:
                    greenColor.G = 137;
                    break;
                case 3:
                    greenColor.G = 146;
                    break;
                case 4:
                    greenColor.G = 147;
                    break;
                case 5:
                    greenColor.G = 130;
                    break;
            }
            return greenColor;
        }

        public void draw(GameTime gameTime, Camera camera, BasicEffect effect)
        {
            if (vertexBuffer != null)
            {
                VertexBuffer newVertexBuffer = null;
                if (vertexArray != null)
                {
                    newVertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), this.vertexList.Count, BufferUsage.WriteOnly);
                    newVertexBuffer.SetData(vertexArray);

                    if (newVertexBuffer != vertexBuffer && newVertexBuffer != null)
                    {
                        vertexBuffer = newVertexBuffer;
                    }
                }
                
                graphicsDevice.SetVertexBuffer(vertexBuffer);

                effect.View = camera.View;
                effect.Projection = camera.Projection;
                effect.World = Matrix.Identity;
                effect.VertexColorEnabled = true;

                effect.CurrentTechnique.Passes[0].Apply();

                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
            }
        }
    }
}
