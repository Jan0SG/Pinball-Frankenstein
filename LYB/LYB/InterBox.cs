using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LYB
{
    public class InterBox : VBox
    {
        public event EventHandler Collided;

        public InterBox(int x, int y, int width, int height, int id) : base(x, y, width, height, id) { }

        public override void React(Graphics g, List<VPoint> pts, int width, int height)
        {
            base.React(g, pts, width, height);

            foreach (var p in pts)
            {
                if (React(g, p))
                {
                    Collided?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}