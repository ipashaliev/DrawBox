using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawBox.Shapes
{
    [Serializable]
    public class Triangle : Shape
    {

        public override void Paint(Graphics g)
        {

            var borderColor = Selected
              ? Color.Red
              : Color;

            if (Fill)
            {
                using (var brush = new SolidBrush(Color))
                    g.FillPolygon(brush, Points);
            }
            using (var pen = new Pen(borderColor, 4))
                g.DrawPolygon(pen, Points);

        }
        

        private static double CheckArea(Point A, Point B, Point C)
        {

            return Math.Abs((A.X * (B.Y - C.Y) +
                             B.X * (C.Y - A.Y) +
                             C.X * (A.Y - B.Y)) / 2.0);
        }
        public override bool Contains(Point p)
        {
            double A = CheckArea(Points[0], Points[1], Points[2]);
            double A1 = CheckArea(p, Points[1], Points[2]);
            double A2 = CheckArea(Points[0], p, Points[2]);
            double A3 = CheckArea(Points[0], Points[1], p);

            return (A == A1 + A2 + A3);
       
        }

        public override bool Intersects(Shape shape)
        {
            foreach (var point in shape.Points)
            {
                if (Contains(point))
                {
                    return true;
                }
            }
            return false;
        }

        public override int Area
        {
            get
            {
                return Width * Height/2;
            }
        }

        public static implicit operator int(Triangle triangle)
        {
            return triangle.Area;
        }
    }
}
