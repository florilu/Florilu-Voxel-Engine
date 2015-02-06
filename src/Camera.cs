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
    public class Camera : GameComponent
    {
        private Vector3 cameraPosition;
        private Vector3 cameraRotaion;
        private Vector3 cameraLookAt;
        private Vector3 mouseRotationBuffer;

        private MouseState prevMouseState;
        private MouseState currentMouseState;

        private float cameraSpeed;

        private float plusRadiant;
        private float minusRadiant;

        public Vector3 Position
        {
            get { return cameraPosition; }

            set
            {
                cameraPosition = value;

                UpdateLookAt();
            }
        }

        public Vector3 Rotation
        {
            get { return cameraRotaion; }

            set
            {
                cameraRotaion = value;

                UpdateLookAt();
            }
        }

        public Matrix Projection
        {
            get;
            protected set;
        }

        public Matrix View
        {
            get
            {
                return Matrix.CreateLookAt(Position, cameraLookAt, Vector3.Up);
            }
        }

        public Camera(Game game, Vector3 position, Vector3 rotation, float speed)
            : base(game)
        {
            Position = position;
            Rotation = rotation;
            this.cameraSpeed = speed;

            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                         game.GraphicsDevice.Viewport.AspectRatio,
                         0.05f,
                         10000.0f);

            MoveTo(position, rotation);

            prevMouseState = Mouse.GetState();

            this.plusRadiant = MathHelper.ToRadians(89.0f);
            this.minusRadiant = MathHelper.ToRadians(-89.0f);
        }

        private void UpdateLookAt()
        {
            Matrix rotationMatrix = Matrix.CreateRotationX(cameraRotaion.X) *
                                    Matrix.CreateRotationY(cameraRotaion.Y);

            Vector3 lookAtOffset = Vector3.Transform(Vector3.UnitZ, rotationMatrix);

            cameraLookAt = cameraPosition + lookAtOffset;
        }


        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentMouseState = Mouse.GetState();

            Vector3 moveVector = Vector3.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                moveVector.Z = 1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                moveVector.Z = -1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                moveVector.X = 1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                moveVector.X = -1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                moveVector.Y = 1;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
            {
                moveVector.Y = -1;
            }
                

            if (moveVector != Vector3.Zero)
            {
                moveVector.Normalize();

                moveVector *= dt * cameraSpeed;

                Move(moveVector);
            }

            float deltaX;
            float deltaY;

            if (currentMouseState != prevMouseState)
            {
                deltaX = Mouse.GetState().X - (Game.GraphicsDevice.Viewport.Width / 2);
                deltaY = Mouse.GetState().Y - (Game.GraphicsDevice.Viewport.Height / 2);

                mouseRotationBuffer.X -= 0.10f * deltaX * dt;
                mouseRotationBuffer.Y -= 0.10f * deltaY * dt;

                if (mouseRotationBuffer.Y < this.minusRadiant)
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - this.minusRadiant);
                if (mouseRotationBuffer.Y > this.plusRadiant)
                    mouseRotationBuffer.Y = mouseRotationBuffer.Y - (mouseRotationBuffer.Y - this.plusRadiant);

                Rotation = new Vector3(-MathHelper.Clamp(mouseRotationBuffer.Y,
                                       this.minusRadiant, this.plusRadiant),
                                       MathHelper.WrapAngle(mouseRotationBuffer.X), 0);

                deltaX = 0;
                deltaY = 0;
            }

            Mouse.SetPosition(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height / 2);

            prevMouseState = currentMouseState;

            base.Update(gameTime);
        }

        public void MoveTo(Vector3 position, Vector3 rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Vector3 PreviewMove(Vector3 amount)
        {
            Matrix rotate = Matrix.CreateRotationY(cameraRotaion.Y);
            Vector3 movement = new Vector3(amount.X, amount.Y, amount.Z);
            movement = Vector3.Transform(movement, rotate);

            return cameraPosition + movement;
        }

        public void Move(Vector3 scale)
        {
            MoveTo(PreviewMove(scale), Rotation);
        }
    }
}
