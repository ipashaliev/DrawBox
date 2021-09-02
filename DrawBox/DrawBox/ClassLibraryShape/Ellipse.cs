using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawBox.Shapes
{
    [Serializable]
    public class Ellipse : Shape
    {
       
        public override void Paint(Graphics g)
        {

            var borderColor = Selected
              ? Color.Red
              : Color;

            if (Fill)
            {
                using (var brush = new SolidBrush(Color))
                    g.FillEllipse(brush, Location.X, Location.Y, Width, Height);
            }
            using (var pen = new Pen(borderColor, 4))
                g.DrawEllipse(pen, Location.X, Location.Y, Width, Height);

        }

        public override int Area
        {
            get
            {
                return Convert.ToInt32((Width/2 * Height/2)*Math.PI);
            }
        }

        public static implicit operator double(Ellipse ellipse)
        {
            return ellipse.Area;
        }
    }
}
