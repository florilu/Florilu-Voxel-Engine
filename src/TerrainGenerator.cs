using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibNoise.Builder;
using LibNoise.Filter;
using LibNoise.Modifier;
using LibNoise.Primitive;
using LibNoise.Renderer;
using LibNoise;

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
    public class TerrainGenerator
    {
        private int seed = 0;
        private float frequency;
        private float offset;
        private float gain;
        private float exponent;
        private float octaveCount;
        private float lacunarity;

        private GradientColor gradient;
        private NoiseQuality quality;
        private NoisePrimitive primitive;
        private NoiseFilter filter;
        private PrimitiveModule pModule;
        private FilterModule fModule;
        private ScaleBias scale;

        private IModule3D finalModule;

        public TerrainGenerator(int seed)
        {
            this.seed = seed;
            /*this.frequency = 1;
            this.lacunarity = 2;
            this.offset = 1;
            this.gain = 2;
            this.exponent = 0.9f;
            this.octaveCount = 10;

            this.gradient = GradientColors.Grayscale;
            this.quality = PrimitiveModule.DefaultQuality;
            this.primitive = NoisePrimitive.ImprovedPerlin;
            this.filter = NoiseFilter.MultiFractal;

            this.pModule = new ImprovedPerlin();

            this.pModule.Quality = quality;
            this.pModule.Seed = seed;

            this.fModule = new MultiFractal();

            this.scale = new ScaleBias(fModule, 1f, -0.8f);

            this.fModule.Frequency = frequency;
            this.fModule.Lacunarity = lacunarity;
            this.fModule.OctaveCount = octaveCount;
            this.fModule.Offset = offset;
            this.fModule.Gain = gain;
            this.fModule.Primitive3D = (IModule3D)pModule;

            this.finalModule = scale;*/
        }

        public float getValue(Vector3 cubeWorldPosition)
        {
            this.frequency = 0.5f;
            this.lacunarity = 2;
            this.offset = 1;
            this.gain = 2;
            this.exponent = 1f;
            this.octaveCount = 4f;

            this.gradient = GradientColors.Grayscale;
            this.quality = PrimitiveModule.DefaultQuality;
            this.primitive = NoisePrimitive.ImprovedPerlin;
            this.filter = NoiseFilter.MultiFractal;

            this.pModule = new ImprovedPerlin();

            this.pModule.Quality = quality;
            this.pModule.Seed = seed;

            this.fModule = new MultiFractal();

            this.scale = new ScaleBias(fModule, 1f, -0.8f);

            this.fModule.Frequency = frequency;
            this.fModule.Lacunarity = lacunarity;
            this.fModule.OctaveCount = octaveCount;
            this.fModule.Offset = offset;
            this.fModule.Gain = gain;
            this.fModule.Primitive3D = (IModule3D)pModule;

            this.finalModule = scale;

            return this.finalModule.GetValue(cubeWorldPosition.X, cubeWorldPosition.Y, cubeWorldPosition.Z);
        }
    }
}
