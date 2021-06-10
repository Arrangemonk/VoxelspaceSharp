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
        public Sprite Color { get; set; }
        public Sprite Height { get; set; }
        public Camera Camera { get; set; }
        public float timer { get; set; }

        static void Main(string[] args)
        {
            // Create an instance
            VoxelSpaceSharp rp = new VoxelSpaceSharp();

            rp.Color = Sprite.Load("textures\\C1W.png");
            rp.Height = Sprite.Load("textures\\H.png");

            // Construct the 100x100 game window with 5x5 pixels
            rp.Construct(rp.Color.Width, rp.Color.Height, 1, 1);
            rp.Camera = new Camera(0, 0, 512, 512);

            // Start and show a window
            rp.Start();
        }

        // Called once per frame
        public override void OnUpdate(float elapsed)
        {
            timer += 0.05f;
            DrawSprite(Point.Origin, Color);

            Camera.OriginX = (int)(Sin(timer) * 512f + 512f);
            Camera.OriginY = (int)(Cos(timer) * 512f + 512f);
            Camera.TargetX = (int)(Sin(timer) * 256f + 512f);
            Camera.TargetY = (int)(Cos(timer) * 256f + 512f);

            Camera.Draw(this);
        }


    }
}
