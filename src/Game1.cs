#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

using System.Diagnostics;

#endregion

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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect basicEffect;

        KeyboardState oldKeyState;

        SpriteFont font;

        RasterizerState rasterizerState;

        bool polygonMode = false;

        Camera camera;

        World world;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            rasterizerState = new RasterizerState();
            rasterizerState.FillMode = FillMode.Solid;
            rasterizerState.CullMode = CullMode.CullCounterClockwiseFace;

            GraphicsDevice.RasterizerState = rasterizerState;

            graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = false;

            basicEffect = new BasicEffect(GraphicsDevice);

            camera = new Camera(this, new Vector3(0.0f, 0.0f, 0.0f), Vector3.Zero, 20.0f);

            Components.Add(camera);

            //buildWorld();

            base.Initialize();
        }

        private void buildWorld()
        {
            Random random = new Random();
            int seed = random.Next();

            world = new World();

            WorldGenThreadExecutor executor = new WorldGenThreadExecutor(this.GraphicsDevice);

            for (int a = 0; a < world.chunks.GetLength(0); a++)
            {
                for (int b = 0; b < world.chunks.GetLength(1); b++)
                {
                    world.chunks[a, b] = new Chunk(new Vector2(a, b), seed);
                }
            }
            
            executor.setChunkList(world.chunks);
            executor.Start();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Content.RootDirectory = "Content";

            //font = Content.Load<SpriteFont>("myFont");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState newKeyState = Keyboard.GetState();
            if (oldKeyState != null)
            {
                if (newKeyState.IsKeyDown(Keys.P) && oldKeyState.IsKeyUp(Keys.P))
                {
                    if (world != null)
                    {
                        world.setBlockAt(new Vector3(17.0f, 16.0f, 5.0f), CubeType.TYPE_DIRT);
                    }
                }

                if (newKeyState.IsKeyDown(Keys.O) && oldKeyState.IsKeyUp(Keys.O))
                {
                    if (polygonMode)
                    {
                        polygonMode = false;

                        rasterizerState = new RasterizerState();
                        rasterizerState.FillMode = FillMode.Solid;
                        
                        GraphicsDevice.RasterizerState = rasterizerState;
                    }
                    else
                    {
                        polygonMode = true;

                        rasterizerState = new RasterizerState();
                        rasterizerState.FillMode = FillMode.WireFrame;

                        GraphicsDevice.RasterizerState = rasterizerState;
                    }
                }

                if (newKeyState.IsKeyDown(Keys.N) && oldKeyState.IsKeyUp(Keys.N))
                {
                    buildWorld();
                }
            }

            oldKeyState = newKeyState;

            base.Update(gameTime);
        }

        private void DrawText()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "X: " + camera.Position.X, new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(font, "Y: " + camera.Position.Y, new Vector2(0, 20), Color.White);
            spriteBatch.DrawString(font, "Z: " + camera.Position.Z, new Vector2(0, 40), Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (world != null)
            {
                for (int a = 0; a < world.chunks.GetLength(0); a++)
                {
                    for (int b = 0; b < world.chunks.GetLength(1); b++)
                    {
                        if (world.chunks[a, b] != null)
                        {
                            world.chunks[a, b].draw(gameTime, camera, basicEffect);
                        }
                    }
                }
            }



            //GraphicsDevice.BlendState = BlendState.Opaque;
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            //DrawText();

            base.Draw(gameTime);
        }
    }
}
