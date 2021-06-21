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


        static void Main(string[] args)
        {
            // Create an instance
            VoxelSpaceSharp game = new VoxelSpaceSharp();

            game.Color = Sprite.Load("textures\\C1W.png");
            game.Height = Sprite.Load("textures\\D1.png");

            var screenwidth = 320;
            var screenheight = 240;

            game.Construct(screenwidth, screenheight, 2, 2);
            game.Camera = new Camera(game,512, 512);

            // Start and show a window
            game.Start();
        }

        public override void OnKeyDown(Key k)
        {
            switch(k)
            {
                case Key.W:
                case Key.Up:
                    Camera.Speed +=1f;
                    break;
                case Key.S:
                case Key.Down:
                    Camera.Speed -= 1f;
                    break;
                case Key.A:
                case Key.Left:
                    Camera.Angle += 1f;
                    break;
                case Key.D:
                case Key.Right:
                    Camera.Angle -= 1f;
                    break;
                case Key.Q:
                case Key.Control:
                    Camera.Height -= 1f;
                    break;
                case Key.E:
                case Key.Shift:
                    Camera.Height += 1f;
                    break;
            }
        }

        public override void OnUpdate(float elapsed)
        {
            Timer += 0.005f;
            
            Camera.Update(Timer);
            Camera.Render(120, 120, 500, ScreenWidth, ScreenHeight, Color, Height);
        }
    }
}
