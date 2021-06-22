using PixelEngine;
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

        public static Pixel getColorAtNN(this Sprite sprite, float x, float y)
        {
            int intX = (int)Math.Round(x);
            int intY = (int)Math.Round(y);
            var maxX = sprite.Width;
            var maxY = sprite.Height;
            return sprite[Wrap(intX, maxX), Wrap(intY, maxY)];

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
