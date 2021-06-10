using PixelEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace voxelspace
{
    static class Helpers
    {
		public static void DrawLine(this Game game,int p1x,int p1y,int p2x,int p2y, Pixel col)
		{
			int x, y, dx, dy, dx1, dy1, px, py, xe, ye, i;
			dx = p2x - p1x; dy = p2y - p1y;
			dx1 = Math.Abs(dx); dy1 = Math.Abs(dy);
			px = 2 * dy1 - dx1; py = 2 * dx1 - dy1;
			if (dy1 <= dx1)
			{
				if (dx >= 0)
				{
					x = p1x; y = p1y; xe = p2x;
				}
				else
				{
					x = p2x; y = p2y; xe = p1x;
				}

				game.Draw(x, y, col);

				for (i = 0; x < xe; i++)
				{
					x = x + 1;
					if (px < 0)
						px = px + 2 * dy1;
					else
					{
						if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) y = y + 1; else y = y - 1;
						px = px + 2 * (dy1 - dx1);
					}
					game.Draw(x, y, col);
				}
			}
			else
			{
				if (dy >= 0)
				{
					x = p1x; y = p1y; ye = p2y;
				}
				else
				{
					x = p2x; y = p2y; ye = p1y;
				}

				game.Draw(x, y, col);

				for (i = 0; y < ye; i++)
				{
					y = y + 1;
					if (py <= 0)
						py = py + 2 * dx1;
					else
					{
						if ((dx < 0 && dy < 0) || (dx > 0 && dy > 0)) x = x + 1; else x = x - 1;
						py = py + 2 * (dx1 - dy1);
					}
					game.Draw(x, y, col);
				}
			}
		}
	}
}
