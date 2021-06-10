using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelEngine;

namespace voxelspace
{
	public class RandomPixels : Game
	{
		static void Main(string[] args)
		{
			// Create an instance
			RandomPixels rp = new RandomPixels();

			// Construct the 100x100 game window with 5x5 pixels
			rp.Construct(100, 100, 5, 5);
			var sprite = new Sprite(1024, 1024);


			// Start and show a window
			rp.Start();
		}

		// Called once per frame
		public override void OnUpdate(float elapsed)
		{
			// Loop through all the pixels
			for (int i = 0; i < ScreenWidth; i++)
				for (int j = 0; j < ScreenHeight; j++)
					Draw(i, j, Pixel.Random()); // Draw a random pixel
		}
	}
}
