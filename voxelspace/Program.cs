using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelEngine;

namespace voxelspace
{
    public class VoxelSpaceSharp : Game
    {
        private readonly TimeSpan Threshold = new TimeSpan(0, 0, 0, 0, 300);

        public Sprite Color { get; set; }
        public Sprite Height { get; set; }
        public Sprite SkyGradient { get; set; }
        public Camera Camera { get; set; }
        public bool Debug { get; set; }
        public bool HQ { get; set; }
        public bool R { get; set; }
        public Key? LastKey { get; set; }
        public DateTime Stamp { get; set; }


        static void Main(string[] args)
        {
            VoxelSpaceSharp game = new VoxelSpaceSharp();
            game.Start();
        }

        public VoxelSpaceSharp() : base()
        {

            //new Pixel(102, 163, 225)
            Color = Sprite.Load("textures\\carebean.png");
            Height = Sprite.Load("textures\\carebeanheight.png");
            SkyGradient = Sprite.Load("textures\\skygradient.png");
            Construct(320, 240, 2, 2);
            Camera = new Camera(this, Color, Height, SkyGradient, ScreenWidth, ScreenHeight, 1000, 120, 512, 512);
        }

        public override void OnKeyDown(Key k)
        {
            switch (k)
            {
                case Key.W:
                case Key.Up:
                    Camera.Speed = Math.Min(50f, Camera.Speed + 1);
                    break;
                case Key.S:
                case Key.Down:
                    Camera.Speed = Math.Max(-50f, Camera.Speed - 1);
                    break;
                case Key.A:
                case Key.Left:
                    Camera.UpdateAngle(true);
                    break;
                case Key.D:
                case Key.Right:
                    Camera.UpdateAngle(false);
                    break;
                case Key.Q:
                case Key.Control:
                    Camera.Height = Math.Max(0f, Camera.Height - 2.0f);
                    break;
                case Key.E:
                case Key.Shift:
                    Camera.Height = Math.Min(255f, Camera.Height + 2.0f);
                    break;
                case Key.Escape:
                    {
                        if (k == LastKey && DateTime.Now - Stamp < Threshold)
                            break;
                        Debug = !Debug;
                        LastKey = k;
                        Stamp = DateTime.Now;
                        break;
                    }
                case Key.F:
                    {
                        if (k == LastKey && DateTime.Now - Stamp < Threshold)
                            break;
                        HQ = !HQ;
                        LastKey = k;
                        Stamp = DateTime.Now;
                        break;
                    }
                case Key.R:
                    {
                        if (k == LastKey && DateTime.Now - Stamp < Threshold)
                            break;
                        R = !R;
                        LastKey = k;
                        Stamp = DateTime.Now;
                        break;
                    }
                default:
                    break;
            }
        }

        public override void OnUpdate(float elapsed)
        {
            Camera.Update();
            if (R)
                Camera.RenderRayTraced(HQ);
            else if (HQ)
                Camera.RenderHQ();
            else
                Camera.Render();
            if (Debug)
            {
                DrawText(Point.Origin, Camera.Speed.ToString(), Pixel.Presets.Red);
                DrawText(new Point(0, 10), Camera.Angle.ToString(), Pixel.Presets.Red);
                DrawText(new Point(0, 20), $"x: {Camera.OriginX} y: {Camera.OriginY}", Pixel.Presets.Red);
            }
        }
    }
}
