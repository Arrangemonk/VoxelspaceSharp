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
        private static readonly float Pi = (float)Math.PI;
        public Game Game { get; private set; }
        public Sprite ColorMap { get; private set; }
        public Sprite HeightMap { get; private set; }
        public Sprite SkyGradient { get; private set; }
        public float OriginX { get; set; }
        public float OriginY { get; set; }
        public float Angle { get; set; }
        public float Fov { get; set; }
        public float Speed { get; set; }
        public float Height { get; set; }
        private Pixel ClrColor { get; set; }

        public Camera(Game g, Sprite colormap, Sprite heightmap, Sprite skygradient, Pixel clrColor, float originx, float originy, float fov = 90.0f)
        {
            Game = g;
            ColorMap = colormap;
            HeightMap = heightmap;
            SkyGradient = skygradient;
            ClrColor = clrColor;
            OriginX = originx;
            OriginY = originy;
            Fov = fov;
            Speed = 0;
            Height = 120;
        }

        public void Update()
        {
            float sinphi = Game.Sin(GetRadian(Angle));
            float cosphi = Game.Cos(GetRadian(Angle));

            OriginX -= sinphi * Speed;
            OriginY -= cosphi * Speed;
        }

        private float GetRadian(float degrees)
        {
            return Pi * degrees / 180.0f;
        }

        public void Render(int horizon, int scale_height, int distance, int screen_width, int screen_height)
        {
            float[] yBuffer = new float[screen_width];
            float[] aBuffer = new float[screen_width];
            float distanceInvers = 1.0f / distance;
            float screen_width_inverse = 1.0f / screen_width;
            float screen_height_inverse = 1.0f / screen_height;
            float sinphi = Game.Sin(GetRadian(Angle));
            float cosphi = Game.Cos(GetRadian(Angle));

            //Game.Clear(ClrColor);
            Helpers.DrawGradientNN(Game, Point.Origin, SkyGradient, screen_width, screen_height, screen_height_inverse);
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

                    float tmp_pLeftX = pLeftX + (x * dx);
                    float tmp_pLeftY = pLeftY + (x * dy);

                    float heightOfHeightMap = HeightMap.getHeightAtNN(tmp_pLeftX, tmp_pLeftY);
                    float height_on_screen = (Height - heightOfHeightMap) * zinverse + horizon;

                    if (height_on_screen < yBuffer[x])
                    {
                        Pixel color = ColorMap.getColorAtNN(tmp_pLeftX, tmp_pLeftY);
                        Pixel skycolor = SkyGradient.getColorAtNN(0, (height_on_screen * SkyGradient.Height) * screen_height_inverse);

                        color = Helpers.interpolate(skycolor, color, Math.Min(fogamount * aBuffer[x], 1.0f));
                        Game.DrawColumn(x, height_on_screen, yBuffer[x], color);

                        yBuffer[x] = height_on_screen;
                    }
                }
            }
        }

        public void RenderHQ(float horizon, float scale_height, float distance, int screen_width, int screen_height)
        {
            float[] yBuffer = new float[screen_width];
            Pixel[] cBuffer = new Pixel[screen_width];
            bool[] vBuffer = new bool[screen_width];
            float[] aBuffer = new float[screen_width];
            float distanceInvers = 1.0f / distance;
            float screen_width_inverse = 1.0f / screen_width;
            float screen_height_inverse = 1.0f / screen_height;
            Pixel groundColor = ColorMap.getColorAt(OriginX, OriginY);

            //Game.Clear(ClrColor);
            Helpers.DrawGradient(Game, Point.Origin, SkyGradient, screen_width, screen_height, screen_height_inverse);
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
                for (float fx = 0; fx < screen_width; fx+= 1.0f)
                {
                    float tmp_pLeftX = pLeftX + (fx * dx);
                    float tmp_pLeftY = pLeftY + (fx * dy);
                    float heightOfHeightMap = HeightMap.getHeightAt(tmp_pLeftX, tmp_pLeftY);
                    float height_on_screen = (Height - heightOfHeightMap) * zinverse + horizon;

                    int x = (int)fx;
                    if (height_on_screen < yBuffer[x])
                    {
                        Pixel color = ColorMap.getColorAt(tmp_pLeftX, tmp_pLeftY);
                        color = Helpers.interpolate(SkyGradient.getColorAt(0, height_on_screen), color, Math.Min(fogamount * aBuffer[x], 1.0f));
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

    }
}
