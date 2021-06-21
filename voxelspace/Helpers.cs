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
        public static float getLength(Game g, float aX, float aY, float bX, float bY)
        {
            var tmpx = Math.Abs(aX - bX);
            var tmpy = Math.Abs(aY - bY);
            return g.Power((tmpx * tmpx) + (tmpy * tmpy), 0.5f);
        }

        public static void DrawLine(this Game game, int p1x, int p1y, int p2x, int p2y, Pixel col)
        {
            int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
            dx = p2x - p1x; dy = p2y - p1y;
            dx1 = Math.Abs(dx); dy1 = Math.Abs(dy);
            px = 2 * dy1 - dx1; py = 2 * dx1 - dy1;
            if (dy1 <= dx1)
            {
                if (dx >= 0)
                {
                    x = p1x; y = p1y; xe = p2x;
                }
                else
                {
                    x = p2x; y = p2y; xe = p1x;
                }

                game.Draw(x, y, col);

                for (i = 0; x < xe; i++)
                {
                    x = x + 1;
                    if (px < 0)
                        px = px + 2 * dy1;
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) y = y + 1; else y = y - 1;
                        px = px + 2 * (dy1 - dx1);
                    }
                    game.Draw(x, y, col);
                }
            }
            else
            {
                if (dy >= 0)
                {
                    x = p1x; y = p1y; ye = p2y;
                }
                else
                {
                    x = p2x; y = p2y; ye = p1y;
                }

                game.Draw(x, y, col);

                for (i = 0; y < ye; i++)
                {
                    y = y + 1;
                    if (py <= 0)
                        py = py + 2 * dx1;
                    else
                    {
                        if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) x = x + 1; else x = x - 1;
                        py = py + 2 * (dx1 - dy1);
                    }
                    game.Draw(x, y, col);
                }
            }
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
                //game.Draw((int)game.Round(p1x), (int)game.Round(lower + i), interpolate(col2, col1, 1.0f * i * amountInverse));
                game.Draw((int)p1x, (int)lower + i, interpolate(col2, col1, 1.0f * i * amountInverse));
            }
        }


        public static IEnumerable<Pixel> GetColorLine(this Sprite sprite, Game g, float p1x, float p1y, float p2x, float p2y)
        {
            if (p1x == p2x && p1y == p2y)
            {
                yield return sprite.getColorAt(p1x, p1y);

                yield break;
            }

            float end = Helpers.getLength(g, p1x, p1y, p2x, p2y);
            float enddivider = 1.0f / end;
            for (int i = 0; i < Math.Round(end, 0); i++)
            {
                float currentTargetX = Helpers.interpolate(p2x, p1x, i * enddivider);
                float currentTargetY = Helpers.interpolate(p2y, p1y, i * enddivider);
                yield return sprite.getColorAt(currentTargetX, currentTargetY);
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
