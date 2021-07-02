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
        //static
        private static readonly float Pi = (float)Math.PI;
        private static readonly float Pi2 = (float)(Math.PI * 2.0d);
        private static readonly float pixelto01 = 1.0f / 255f;
        private Vector rayOrigin;
        public Game Game { get; private set; }
        public Sprite ColorMap { get; private set; }
        public Sprite HeightMap { get; private set; }
        public Sprite SkyGradient { get; private set; }
        public float OriginX { get; private set; }
        public float OriginY { get; private set; }
        public float Fov { get; private set; }
        public float ScaleHeight { get; private set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public float Distance { get; private set; }
        public int Horizon { get; private set; }
        public float DistanceInvers { get; private set; }
        public float ScreenWidthInverse { get; private set; }
        public float ScreenHeightInverse { get; private set; }
        public float SinPhi { get; set; }
        public float CosPhi { get; set; }


        //dynamic
        public float Angle { get; set; }
        public float AngleV { get; set; }
        public float Speed { get; set; }
        public float Height { get; set; }

        private ParallelOptions pos;

        public Camera(Game g, Sprite colormap, Sprite heightmap, Sprite skygradient,
            int screenWidth, int screenHeight, float distance, int horizon,
            float originx, float originy, float fov = 90.0f)
        {
            Game = g;
            ColorMap = colormap;
            HeightMap = heightmap;
            SkyGradient = skygradient;
            OriginX = originx;
            OriginY = originy;
            Fov = fov / 180f * Pi;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            Distance = distance;
            Horizon = horizon;
            Speed = 0;
            Height = 60;
            ScaleHeight = 60;
            Angle = 0;
            SinPhi = Game.Sin(Angle);
            CosPhi = Game.Cos(Angle);

            DistanceInvers = 1.0f / Distance;
            ScreenWidthInverse = 1.0f / ScreenWidth;
            ScreenHeightInverse = 1.0f / ScreenHeight;

            pos = new ParallelOptions();
            pos.MaxDegreeOfParallelism = 4;
        }

        public void Update()
        {
            OriginX -= SinPhi * Speed;
            OriginX = Helpers.Wrap(OriginX, ColorMap.Width);
            OriginY -= CosPhi * Speed;
            OriginY = Helpers.Wrap(OriginY, ColorMap.Height);
            rayOrigin = new Vector(OriginX, Height, OriginY);
        }

        public void UpdateAngle(bool increase)
        {
            Angle = (Angle + (increase ? 0.06981317f : -0.06981317f) + Pi2) % Pi2;
            SinPhi = Game.Sin(Angle);
            CosPhi = Game.Cos(Angle);
        }

        public void Render()
        {
            float[] yBuffer = new float[ScreenWidth];
            float[] aBuffer = new float[ScreenWidth];
            var dz = 1.0f;

            Helpers.DrawGradientNN(Game, Point.Origin, SkyGradient, ScreenWidth, ScreenHeight, ScreenHeightInverse);
            for (int i = 0; i < ScreenWidth; i++)
            {
                yBuffer[i] = ScreenHeight;
                aBuffer[i] = 2 - Game.Sin(i * ScreenWidthInverse * Pi);
            }

            for (float z = 1.0f; z < Distance; z += dz)
            {
                float zinverse = ScaleHeight / z;
                float fogamount = z * DistanceInvers;
                fogamount *= fogamount;
                float pLeftX = ((-CosPhi - SinPhi) * z) + OriginX;
                float pLeftY = ((SinPhi - CosPhi) * z) + OriginY;
                float pRightX = ((CosPhi - SinPhi) * z) + OriginX;
                float pRightY = ((-SinPhi - CosPhi) * z) + OriginY;
                float dx = (pRightX - pLeftX) * ScreenWidthInverse;
                float dy = (pRightY - pLeftY) * ScreenWidthInverse;
                for (int x = 0; x < ScreenWidth; x++)
                {

                    float tmp_pLeftX = pLeftX + (x * dx);
                    float tmp_pLeftY = pLeftY + (x * dy);

                    float heightOfHeightMap = getHeightMapAtNN(tmp_pLeftX, tmp_pLeftY);
                    float height_on_screen = (Height - heightOfHeightMap) * zinverse + Horizon;

                    if (height_on_screen < yBuffer[x])
                    {
                        Pixel color = ColorMap.getColorAtNN(tmp_pLeftX, tmp_pLeftY);
                        Pixel skycolor = SkyGradient.getColorAtNN(0, (height_on_screen * SkyGradient.Height) * ScreenHeightInverse);
                        color = Helpers.interpolate(skycolor, color, Math.Min(fogamount * aBuffer[x], 1.0f));
                        Game.DrawColumn(x, height_on_screen, yBuffer[x], color);
                        yBuffer[x] = height_on_screen;
                    }
                }
                dz += 0.005f;
            }
        }

        public void RenderHQ()
        {

            float[] yBuffer = new float[ScreenWidth];
            Pixel[] cBuffer = new Pixel[ScreenWidth];
            float[] aBuffer = new float[ScreenWidth];
            Pixel groundColor = ColorMap.getColorAt(OriginX, OriginY);
            var dz = 1.0f;

            Helpers.DrawGradient(Game, Point.Origin, SkyGradient, ScreenWidth, ScreenHeight, ScreenHeightInverse);
            for (int i = 0; i < ScreenWidth; i++)
            {
                yBuffer[i] = ScreenHeight;
                cBuffer[i] = groundColor;
                aBuffer[i] = 2 - Game.Sin(i * ScreenWidthInverse * Pi);
            }

            for (float z = 1.0f; z < Distance; z += dz)
            {
                float zinverse = ScaleHeight / z;
                float fogamount = z * DistanceInvers;
                fogamount *= fogamount;
                float pLeftX = ((-CosPhi - SinPhi) * z) + OriginX;
                float pLeftY = ((SinPhi - CosPhi) * z) + OriginY;
                float pRightX = ((CosPhi - SinPhi) * z) + OriginX;
                float pRightY = ((-SinPhi - CosPhi) * z) + OriginY;
                float dx = (pRightX - pLeftX) * ScreenWidthInverse;
                float dy = (pRightY - pLeftY) * ScreenWidthInverse;

                Parallel.For(0, ScreenWidth, pos, x =>
                //for (float fx = 0; fx < ScreenWidth; fx += 1.0f)
                {
                    var fx = 1.0f * x;
                    float tmp_pLeftX = pLeftX + (fx * dx);
                    float tmp_pLeftY = pLeftY + (fx * dy);
                    float heightOfHeightMap = getHeightMapAt(tmp_pLeftX, tmp_pLeftY);
                    float height_on_screen = (Height - heightOfHeightMap) * zinverse + Horizon;

                    Pixel color = ColorMap.getColorAt(tmp_pLeftX, tmp_pLeftY);
                    Pixel skycolor = SkyGradient.getColorAt(0, height_on_screen);
                    color = Helpers.interpolate(skycolor, color, Math.Min(fogamount * aBuffer[x], 1.0f));
                    if (height_on_screen < yBuffer[x])
                    {
                        Game.DrawColorColumn(x, height_on_screen, yBuffer[x], color, cBuffer[x]);
                        yBuffer[x] = height_on_screen;
                    }
                    cBuffer[x] = color;
                });
                dz += 0.005f;
            }
        }

        public void RenderRayTraced(bool March)
        {
            Parallel.For(0, ScreenHeight, y =>
            {
                var skyColor = SkyColor(y);
                for (int x = 0; x < ScreenWidth; x++)
                {
                    Vector direction = GenerateRayFromPixel(x, y);
                    bool result;
                    var distance = March 
                    ? RayMarch(direction, out result) 
                    : Trace(direction, out result);
                    if (result)
                        Game.Draw(x, y, terrainColor(direction, skyColor, distance));
                    else
                        Game.Draw(x, y, skyColor);
                }
            }
            );
        }

        float Trace(Vector direction, out bool result)
        {
            float dd = 0.01f;
            float olddist = 4;
            result = false;
            for (float dist = olddist; dist < Distance; dist += dd)
            {
                Vector rayPoint = Target(direction, dist);
                float mapResult = getHeightMapAt(rayPoint.X, rayPoint.Z);
                result = rayPoint.Y < mapResult;
                dd += 0.005f;
                olddist = dist;
                if(result)
                    break;
            }
            return olddist;
        }

        float RayMarch(Vector direction, out bool result)
        {
            float marchDistance;
            const int maxITers = 1000;
            int iters = 0;
            bool caught = false;
            float finaldistance = Distance;
            for (float dist = 4; dist < Distance; dist += marchDistance)
            {
                Vector rayPoint = Target(direction, dist);
                float mapResult = getHeightMapAt(rayPoint.X, rayPoint.Z);
                marchDistance = (rayPoint.Y - mapResult);
                caught = Math.Abs(marchDistance) < (0.01f * dist );
                iters++;
                finaldistance = dist;
                if (caught || iters > maxITers)
                    break;
            }
                result = caught;
                return finaldistance;
            }

            private Pixel terrainColor(Vector direction, Pixel sky, float distance)
            {
                Vector p = Target(direction, distance);

                var terrain = getColorMapAt(p.X, p.Z);
                return Helpers.interpolate(sky, terrain, Math.Min(distance * DistanceInvers, 1.0f));
            }

            private Vector Target(Vector direction, float distance)
            => rayOrigin + (direction * distance);

            private Pixel getColorMapAt(float x, float y) => Helpers.getColorAt(ColorMap, x, y);
            private float getHeightMapAt(float x, float y) => Helpers.getHeightAt(HeightMap, x, y) * pixelto01 * ScaleHeight;
        private float getHeightMapAtNN(float x, float y) => Helpers.getHeightAtNN(HeightMap, x, y) * pixelto01 * ScaleHeight;

        public Vector GenerateRayFromPixel(int x, int y)
            {
                //float wtf = Game.Tan(Fov * 0.5f);
                float aspect = ScreenWidth * ScreenHeightInverse;
                float xs = (1f - 2f * ((0.5f + x) * ScreenWidthInverse)) * aspect;
                float ys = (1f - 2f * ((0.5f + y) * ScreenHeightInverse));
                var pixeldirection = Vector.Normalize(new Vector(xs, ys, -1f));
                return Vector.Normalize(Helpers.rotatevecY(Game, pixeldirection, Angle));
            }

            public Pixel SkyColor(int y)
            {
                return SkyGradient.getColorAtNN(0, y * SkyGradient.Height * ScreenHeightInverse);
            }
        }
    }
