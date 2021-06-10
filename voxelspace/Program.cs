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
		static void Main(string[] args)
		{
			// Create an instance
			VoxelSpaceSharp rp = new VoxelSpaceSharp();

			rp.Color = Sprite.Load("textures\\C1W.png");
		    rp.Height = Sprite.Load("textures\\H.png");

			// Construct the 100x100 game window with 5x5 pixels
			rp.Construct(rp.Color.Width, rp.Color.Height, 1, 1);


			// Start and show a window
			rp.Start();
		}

		// Called once per frame
		public override void OnUpdate(float elapsed)
		{
			// Loop through all the pixels
			for (int i = 0; i < ScreenWidth; i++)
				for (int j = 0; j < ScreenHeight; j++)
					Draw(i, j, Color[i,j]); // draw a pixel from the color sprite
		}
	}
}
