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
        public Camera(int originx,int originy, int targetx, int targety, float angle = 90f)
        {
            OriginX = originx;
            OriginY = originy;
            TargetX = targetx;
            TargetY = targety;
            Angle = angle;
        }

        public int OriginX { get; set; }
        public int OriginY { get; set; }
        public int TargetX { get; set; }
        public int TargetY { get; set; }
        public float Angle { get; set; }

        public void Draw(Game game)
        {
            //draw a trangle 
            // first we need to determinate the 2 end points

            //this only works under the assumption that angle is 90°
            var tmpx = OriginX - TargetX;
            var tmpy = OriginY - TargetY;
            int resultAX = TargetX + tmpy;
            int resultAY = TargetY - tmpx;
            int resultBX = TargetX - tmpy;
            int resultBY =  TargetY + tmpx;

            game.DrawLine(resultAX, resultAY, resultBX, resultBY, Pixel.Presets.Black);
            game.DrawLine(OriginX, OriginY, resultAX, resultAY, Pixel.Presets.Black);
            game.DrawLine(OriginX, OriginY, resultBX, resultBY, Pixel.Presets.Black);
        }
    }
}
