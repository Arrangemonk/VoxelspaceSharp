using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelEngine;
using PixelEngine.Utilities;

namespace voxelspace
{
    public class Camera
    {
        public Camera(Game g,float originx, float originy,float fov = 90.0f)
        {
            Game = g;
            OriginX = originx;
            OriginY = originy;
            Fov = fov;
            Speed = 0;
            Height = 120;
        }
        public Game Game { get; private set; }
        public float OriginX { get; set; }
        public float OriginY { get; set; }
        public float Angle { get; set; }
        public float Fov { get; set; }
        public float Speed { get; set; }
        public float Height { get; set; }

        public void Update(float dt)
        {
            float sinphi = Game.Sin(GetRadian(Angle));
            float cosphi = Game.Cos(GetRadian(Angle));

            OriginX -= sinphi * Speed;
            OriginY -= cosphi * Speed;
        }

        public void Render(int horizon, int scale_height, int distance, int screen_width, int screen_height, Sprite colorMap, Sprite heightmap)
        {
            Game.Clear(clrColor);

            var yBuffer = new float[screen_width];
            var aBuffer = new float[screen_width];
            var distanceInvers = 1.0f / distance;
            var screen_width_inverse = 1.0f / screen_width;
            float sinphi = Game.Sin(GetRadian(Angle));
            float cosphi = Game.Cos(GetRadian(Angle));
            for (int i = 0; i < screen_width; i++)
            {
                yBuffer[i] = screen_height;
                aBuffer[i] = 2 - Game.Sin(i * screen_width_inverse * Pi);
            }

            for (float z = 1.0f; z < distance; z += 1.0f)
            {
                float zinverse = 1.0f * scale_height / z;
                float fogamount = z * distanceInvers;
                fogamount *= fogamount;
                float pLeftX = ((-cosphi - sinphi) * z) + OriginX;
                float pLeftY = ((sinphi - cosphi) * z) + OriginY;
                float pRightX = ((cosphi - sinphi) * z) + OriginX;
                float pRightY = ((-sinphi - cosphi) * z) + OriginY;

                float dx = (pRightX - pLeftX) * screen_width_inverse;
                float dy = (pRightY - pLeftY) * screen_width_inverse;
                for (int x = 0; x < screen_width; x++)
                {

                    float tmp_pLeftX = pLeftX + (dx * x);
                    float tmp_pLeftY = pLeftY + (dy * x);

                    float heightOfHeightMap = heightmap.getColorAtNN(tmp_pLeftX, tmp_pLeftY).R;
                    float height_on_screen = (Height - heightOfHeightMap) * zinverse + horizon;

                    if (height_on_screen < yBuffer[x])
                    {
                        Pixel color = colorMap.getColorAtNN(tmp_pLeftX, tmp_pLeftY);

                        color = Helpers.interpolate(clrColor, color, Math.Min(fogamount * aBuffer[x],1.0f));
                        Game.DrawColumn(x, height_on_screen, yBuffer[x], color);

                        yBuffer[x] = height_on_screen;
                    }
                }
            }
        }

        public void RenderHQ(int horizon, int scale_height, int distance, int screen_width, int screen_height, Sprite colorMap, Sprite heightmap)
        {
            Game.Clear(clrColor);

            var yBuffer = new float[screen_width];
            var cBuffer = new Pixel[screen_width];
            var vBuffer = new bool[screen_width];
            var aBuffer = new float[screen_width];
            var distanceInvers = 1.0f / distance;
            var screen_width_inverse = 1.0f / screen_width;
            var groundColor = colorMap.getColorAt(OriginX, OriginY);
            for (int i = 0; i < screen_width; i++)
            {
                yBuffer[i] = screen_height;
                cBuffer[i] = groundColor;
                vBuffer[i] = true;
                aBuffer[i] = 2 - Game.Sin(i * screen_width_inverse * Pi);
            }

            float sinphi = Game.Sin(GetRadian(Angle));
            float cosphi = Game.Cos(GetRadian(Angle));

            for (float z = 1.0f; z < distance; z += 1.0f)
            {
                float zinverse = 1.0f * scale_height / z;
                float fogamount = z * distanceInvers;
                fogamount *= fogamount;
                float pLeftX = ((-cosphi - sinphi) * z) + OriginX;
                float pLeftY = ((sinphi - cosphi) * z) + OriginY;
                float pRightX = ((cosphi - sinphi) * z) + OriginX;
                float pRightY = ((-sinphi - cosphi) * z) + OriginY;
                float dx = (pRightX - pLeftX) * screen_width_inverse;
                float dy = (pRightY - pLeftY) * screen_width_inverse;
                for (int x = 0; x < screen_width; x++)
                {
                    float tmp_pLeftX = pLeftX + (dx * x);
                    float tmp_pLeftY = pLeftY + (dy * x);
                    float heightOfHeightMap = heightmap.getColorAt(tmp_pLeftX, tmp_pLeftY).R;
                    float height_on_screen = (Height - heightOfHeightMap) * zinverse + horizon;

                    if (height_on_screen < yBuffer[x])
                    {
                        Pixel color = colorMap.getColorAt(tmp_pLeftX, tmp_pLeftY);
                        color = Helpers.interpolate(clrColor, color, Math.Min(fogamount * aBuffer[x], 1.0f));
                        Game.DrawColorColumn(x, height_on_screen, yBuffer[x], color, vBuffer[x] ? cBuffer[x] : color);
                        yBuffer[x] = height_on_screen;
                        cBuffer[x] = color;
                        vBuffer[x] = true;
                    }
                    else
                    {
                        vBuffer[x] = false;
                    }
                }
            }
        }

        private static readonly float Pi = (float)Math.PI;
        private static readonly Pixel clrColor = new Pixel(102, 163, 225);

        private float GetRadian(float degrees)
        {
            return Pi * degrees / 180.0f;
        }
    }
}
