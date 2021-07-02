using PixelEngine;
using PixelEngine.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace voxelspace
{
    public static class Helpers
    {

        public static float Noise(Game g, float seed, float x, float y, float z, float w)
        {
            var number = g.Sin(g.Sin(g.Sin(g.Sin(seed + x) * 1553347f + y) * 43393432f + z) * 55303343334f + w);
            return number - ((int)number);
        }
        public static float GetLength(Game g, float aX, float aY, float bX, float bY)
        {
            var tmpx = Math.Abs(aX - bX);
            var tmpy = Math.Abs(aY - bY);
            return g.Power((tmpx * tmpx) + (tmpy * tmpy), 0.5f);
        }

        public static void DrawColumn(this Game game, float p1x, float p1y, float p2y, Pixel col)
        {
            var amount = Math.Abs(p1y - p2y);
            var lower = Math.Min(p1y, p2y);
            for (int i = 0; i < amount; i++)
            {
                game.Draw((int)p1x, (int)lower + i, col);
            }
        }

        public static void ParallaxMapping(Sprite depthMap, float texX, float texY, Vector viewDir,float height_scale,out float xnew, out float ynew)
        {
            float height = getHeightAt(depthMap, texX, texY);
            xnew = viewDir.X / viewDir.Z * (height / 255f * height_scale);
            ynew = viewDir.Y / viewDir.Z * (height / 255f * height_scale);
        }

        public static void DrawColorColumn(this Game game, float p1x, float p1y, float p2y, Pixel col1, Pixel col2)
        {
            var amount = Math.Abs(p1y - p2y);
            var amountInverse = 1.0f / amount;
            var lower = Math.Min(p1y, p2y);
            for (int i = 0; i < amount; i++)
            {
                game.Draw((int)p1x, (int)lower + i, interpolate(col2, col1, 1.0f * i * amountInverse));
            }
        }

        public static void DrawScaledSprite(this Game game, Point origin, Sprite sprite, int scaleX, int scaleY)
        {
            int halfwidth = sprite.Width / scaleX;
            int halfheight = sprite.Height / scaleY;

            for (int x = 0; x < halfwidth; x++)
                for (int y = 0; y < halfheight; y++)
                    game.Draw(x + origin.X, y + origin.Y, sprite[x * scaleX, y * scaleY]);
        }

        public static void DrawGradient(this Game game, Point origin, Sprite sprite, int width, int height, float heightInverse)
        {
            var scalefactor = sprite.Height * heightInverse;
            for (int y = 0; y < height; y++)
            {
                var clr = sprite.getColorAt(0, y * scalefactor);
                for (int x = 0; x < width; x++)
                    game.Draw(x + origin.X, y + origin.Y, clr);
            }
        }

        public static void DrawGradientNN(this Game game, Point origin, Sprite sprite, int width, int height, float heightInverse)
        {
            var scalefactor = sprite.Height * heightInverse;
            for (int y = 0; y < height; y++)
            {
                var clr = sprite[0, (int)(y * scalefactor)];
                for (int x = 0; x < width; x++)
                    game.Draw(x + origin.X, y + origin.Y, clr);
            }
        }

        public static Pixel getColorAt(this Sprite sprite, float x, float y)
        {
            int intX = (int)x;
            int intY = (int)y;
            int intX1 = intX + 1;
            int intY1 = intY + 1;
            float fractx = Clamp(x - intX, 0, 1);
            float fracty = Clamp(y - intY, 0, 1);
            var maxX = sprite.Width;
            var maxY = sprite.Height;

            var color00 = sprite[Wrap(intX, maxX), Wrap(intY, maxY)];
            var color01 = sprite[Wrap(intX, maxX), Wrap(intY1, maxY)];
            var color10 = sprite[Wrap(intX1, maxX), Wrap(intY, maxY)];
            var color11 = sprite[Wrap(intX1, maxX), Wrap(intY1, maxY)];

            var colorhoz0 = interpolate(color10, color00, fractx);
            var colorhoz1 = interpolate(color11, color01, fractx);

            return interpolate(colorhoz1, colorhoz0, fracty);

        }

        public static Vector getNormal(Vector p, Func<float, float, float> f)
        {
            const float eps = 1f;
            return Vector.Normalize(new Vector(f(p.X - eps, p.Z) - f(p.X + eps, p.Z),
                                    2f * eps, f(p.X, p.Z - eps) - f(p.X, p.Z + eps)));
        }

        public static Vector rotatevecY(Game g,Vector vec, float angle)
        {
            Vector m0 = new Vector(-g.Cos(angle), 0, g.Sin(angle));
            Vector m1 = new Vector(0, 1f, 0);
            Vector m2 = new Vector(g.Sin(angle), 0, g.Cos(angle));

            return new Vector(Vector.Dot(m0, vec), Vector.Dot(m1, vec), Vector.Dot(m2, vec));
        }

        public static float SmoothStep(float x, float edge0, float edge1)
        {
            // Scale, bias and saturate x to 0..1 range
            x = Clamp((x - edge0) / (edge1 - edge0), 0, 1);
            // Evaluate polynomial
            return x * x * (3f - 2f * x);
        }
        public static float SmoothStep2(float x, float edge0, float edge1)
        {
            // Scale, bias and saturate x to 0..1 range
            x = Clamp((x - edge0) / (edge1 - edge0), 0, 1);
            // Evaluate polynomial
            return x * x * x * (x * (x * 6f - 15f) + 10f);
        }

        public static float GetNoiseAt(Game g, Func<Game, float, float, float, float, float, float> noisefunc, float seed, float x, float y)
        {
            int intX = (int)x;
            int intY = (int)y;
            int intX1 = intX + 1;
            int intY1 = intY + 1;
            float fractx = SmoothStep2(x - intX, 0, 1);
            float fracty = SmoothStep2(y - intY, 0, 1);

            var color00 = SmoothStep2(noisefunc(g, seed, intX, intY, 0, 0), 0, 1);
            var color01 = SmoothStep2(noisefunc(g, seed, intX, intY1, 0, 0), 0, 1);
            var color10 = SmoothStep2(noisefunc(g, seed, intX1, intY, 0, 0), 0, 1);
            var color11 = SmoothStep2(noisefunc(g, seed, intX1, intY1, 0, 0), 0, 1);

            var colorhoz0 = interpolate(color10, color00, fractx);
            var colorhoz1 = interpolate(color11, color01, fractx);

            return interpolate(colorhoz1, colorhoz0, fracty);

        }

        public static Pixel getColorAtNN(this Sprite sprite, float x, float y)
        {
            int intX = (int)Math.Round(x);
            int intY = (int)Math.Round(y);
            var maxX = sprite.Width;
            var maxY = sprite.Height;
            return sprite[Wrap(intX, maxX), Wrap(intY, maxY)];

        }

        public static float getHeightAt(this Sprite sprite, float x, float y)
        {
            int intX = (int)x;
            int intY = (int)y;
            int intX1 = intX + 1;
            int intY1 = intY + 1;
            float fractx = Clamp(x - intX, 0, 1);
            float fracty = Clamp(y - intY, 0, 1);
            var maxX = sprite.Width;
            var maxY = sprite.Height;

            float height00 = sprite[Wrap(intX, maxX), Wrap(intY, maxY)].R;
            float height01 = sprite[Wrap(intX, maxX), Wrap(intY1, maxY)].R;
            float height10 = sprite[Wrap(intX1, maxX), Wrap(intY, maxY)].R;
            float height11 = sprite[Wrap(intX1, maxX), Wrap(intY1, maxY)].R;

            var heighthoz0 = interpolate(height10, height00, fractx);
            var heighthoz1 = interpolate(height11, height01, fractx);

            return interpolate(heighthoz1, heighthoz0, fracty);
        }
        public static float getHeightAtNN(this Sprite sprite, float x, float y)
        {
            int intX = (int)Math.Round(x);
            int intY = (int)Math.Round(y);
            var maxX = sprite.Width;
            var maxY = sprite.Height;
            return sprite[Wrap(intX, maxX), Wrap(intY, maxY)].R;

        }


        public static float Clamp(float input, int min, int max)
        {
            return Math.Max(Math.Min(max, input), min);
        }
        public static float Clamp(float input, int max)
        {
            return Math.Max(Math.Min(max, input), 0);
        }
        public static int Clamp(int input, int max)
        {
            return Math.Max(Math.Min(max, input), 0);
        }

        public static int Wrap(int input, int max)
        {
            return (input % max + 3 * max) % max;
        }

        public static float Wrap(float input, float max)
        {
            return (input % max + 3 * max) % max;
        }

        public static byte interpolate(byte a, byte b, float scalar)
        {
            return (byte)(a * scalar + b * (1.0d - scalar));
        }

        public static int interpolate(int a, int b, float scalar)
        {
            return (int)(a * scalar + b * (1.0d - scalar));
        }

        public static float interpolate(float a, float b, float scalar)
        {
            return a * scalar + b * (1.0f - scalar);
        }

        public static Pixel interpolate(Pixel a, Pixel b, float scalar)
        {
            return new Pixel(interpolate(a.R, b.R, scalar),
                interpolate(a.G, b.G, scalar),
                interpolate(a.B, b.B, scalar),
                interpolate(a.A, b.A, scalar));
        }

        public static Point interpolate(Point a, Point b, float scalar)
        {
            return new Point(interpolate(a.X, b.X, scalar),
                interpolate(a.Y, b.Y, scalar));
        }
    }
}
