using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawBox.Shapes
{
    [Serializable]
    public class Rectangle : Shape
    {
     
        public override void Paint(Graphics g)
        {

            var borderColor = Selected
              ? Color.Red
              : Color;

            if (Fill)
            {
                using (var brush = new SolidBrush(Color))
                    g.FillRectangle(brush, Location.X, Location.Y, Width, Height);
            }
            using (var pen = new Pen(borderColor, BorderSize))
                g.DrawRectangle(pen, Location.X, Location.Y, Width, Height);

        }

        public override int Area
        {
            get
            {
                return Width * Height;
            }
        }

        public static implicit operator int(Rectangle rectangle)
        {
            return rectangle.Area;
        }
    }
}
