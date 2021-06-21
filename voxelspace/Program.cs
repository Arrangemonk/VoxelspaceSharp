using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using PixelEngine;

namespace voxelspace
{
    public class VoxelSpaceSharp : Game
    {
        public Sprite Color { get; set; }
        public Sprite Height { get; set; }
        public Camera Camera { get; set; }
        public float Timer { get; set; }
        public bool Debug { get; set; }
        public bool HQ { get; set; }


        static void Main(string[] args)
        {
            // Create an instance
            VoxelSpaceSharp game = new VoxelSpaceSharp();

            game.Color = Sprite.Load("textures\\C1W.png");
            game.Height = Sprite.Load("textures\\D1.png");

            var screenwidth = 320;
            var screenheight = 240;

            game.Construct(screenwidth, screenheight, 2, 2);
            game.Camera = new Camera(game, 512, 512);

            // Start and show a window
            game.Start();
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
                    Camera.Speed = Math.Max(0, Camera.Speed - 1);
                    break;
                case Key.A:
                case Key.Left:
                    Camera.Angle = ((Camera.Angle + 4f) + 360) % 360;
                    break;
                case Key.D:
                case Key.Right:
                    Camera.Angle = ((Camera.Angle - 4f) + 360) % 360;
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
                    Debug = !Debug;
                    break;
                case Key.Space:
                    HQ = !HQ;
                    break;
            }
        }

        public override void OnUpdate(float elapsed)
        {
            Timer += 0.005f;

            Camera.Update(Timer);
            if (HQ)
                Camera.RenderHQ(120, 120, 500, ScreenWidth, ScreenHeight, Color, Height);
            else
                Camera.Render(120, 120, 500, ScreenWidth, ScreenHeight, Color, Height);
            if (Debug)
            {
                DrawText(Point.Origin, Camera.Speed.ToString(), Pixel.Presets.Red);
                DrawText(new Point(0, 10), Camera.Angle.ToString(), Pixel.Presets.Red);
            }
        }
    }
}
