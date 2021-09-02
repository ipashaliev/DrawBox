using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawBox.Shapes
{
    [Serializable]
    public class Shape
    {

        public virtual Point Location { get; set; }

        public Color Color { get; set; }

        public bool Fill { get; set; } = true;

        public bool Selected { get; set; }

        public int BorderSize { get; set; }

        public Point[] Points { get; set; }


        private int _height;

        public int Height
        { 
            get => _height;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("Negative height value not allowed!");

                _height = value;
            }
        }

        private int _width;

        public int Width 
        {
            get => _width;
            set
            {
                if (value < 0)
                    throw new InvalidValueException("Negative width value not allowed!");

                _width = value;
            }
        }


        public virtual void Paint(Graphics g)
        {
            return;
        }
        public virtual int Area
        {
            get;
        }
        public virtual bool Contains(Point p)
        {
            return
                 Location.X < p.X && p.X < Location.X + Width &&
                 Location.Y < p.Y && p.Y < Location.Y + Height;
        }
        public virtual bool Intersects(Shape shape)
        {
            return
                this.Location.X < shape.Location.X + shape.Width &&
                shape.Location.X < this.Location.X + this.Width &&
                this.Location.Y < shape.Location.Y + shape.Height &&
                shape.Location.Y < this.Location.Y + this.Height;
        }
    }
}
