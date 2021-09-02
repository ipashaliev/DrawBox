using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DrawBox.Shapes;
using Rectangle = DrawBox.Shapes.Rectangle;
using Ellipse = DrawBox.Shapes.Ellipse;
using Triangle = DrawBox.Shapes.Triangle;

namespace DrawBox
{
    public partial class FormBoard : Form
    {
        private List<Shape> _shapes = new List<Shape>();

        private Point _currentLocation;

        private Pen _pen = new Pen(Color.Black, 5);

        private bool _captureMouse;
        private Shape _outline;

        private Shape selectedShape;
        private bool _dragging;
        private Point _dragPoint;

        public Graphics g;

        private void toolStripButtonPointer_Click(object sender, EventArgs e)
        {
            UncheckTools();
            toolStripButtonPointer.Checked = true;
            _outline = new Rectangle();
            _outline.BorderSize = 1;
        }

        private void toolStripButtonRectangle_Click(object sender, EventArgs e)
        {
            UncheckTools();
            toolStripButtonRectangle.Checked = true;
            _outline = new Rectangle();
            _outline.BorderSize = 4;
        }

        private void toolStripButtonTriangle_Click(object sender, EventArgs e)
        {
            UncheckTools();
            toolStripButtonTriangle.Checked = true;
            _outline = new Triangle();

        }

        private void toolStripButtonEllipse_Click(object sender, EventArgs e)
        {
            UncheckTools();
            toolStripButtonEllipse.Checked = true;
            _outline = new Ellipse();
        }

    
        private void UncheckTools()
        {
            toolStripButtonPointer.Checked = false;
            toolStripButtonRectangle.Checked = false;
            toolStripButtonEllipse.Checked = false;
            toolStripButtonTriangle.Checked = false;
            _outline = null;
        }


        public FormBoard()
        {
            InitializeComponent();
            g = CreateGraphics();

            SetStyle(
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.OptimizedDoubleBuffer,
              true);
        }

        private Shape CreateFrameShape(Point location)
        {
            _outline.Location = new Point(
             Math.Min(_currentLocation.X, location.X),
             Math.Min(_currentLocation.Y, location.Y));

            _outline.Width = Math.Abs(_currentLocation.X - location.X);
            _outline.Height = Math.Abs(_currentLocation.Y - location.Y);


            _outline.Points = new Point[]
            {
                new Point { X = _currentLocation.X , Y = location.Y},
                new Point { X = location.X, Y = location.Y},
                new Point { X = (location.X + _currentLocation.X)/2, Y = _currentLocation.Y}
            };
            _outline.Width = Math.Abs(_outline.Points[1].X - _outline.Points[0].X);
            _outline.Height = Math.Abs(_outline.Points[2].Y - _outline.Points[0].Y);

            return _outline;
        }
      
        protected override void OnPaint(PaintEventArgs e)
        {
           e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            for (int i = _shapes.Count - 1; i >= 0; i--)
            {
                _shapes[i].Paint(e.Graphics);
            }
          
            if (_captureMouse && _outline != null)
                _outline.Paint(e.Graphics);
          
        }


        private void FormBoard_MouseDown(object sender, MouseEventArgs e)
        {
            _outline = null;
           
            _captureMouse = true;
            _currentLocation = e.Location;

            foreach (var shape in _shapes)
            {
                shape.Selected = false;
            }
          
            
             selectedShape = _shapes
                .FirstOrDefault(r => r.Contains(e.Location));

            if (selectedShape != null)
            {
                selectedShape.Selected = true;
                if (e.Button == MouseButtons.Right && toolStripButtonPointer.Checked)
                {
                    _dragging = true;
                    _dragPoint = new Point(selectedShape.Location.X - e.Location.X, selectedShape.Location.Y - e.Location.Y);
                }
            }
           
        }

        private void FormBoard_MouseMove(object sender, MouseEventArgs e)
        {
            if (toolStripButtonRectangle.Checked)
            {
                _outline = new Rectangle();
                _outline.BorderSize = 4;
            }
            else if (toolStripButtonTriangle.Checked)
                _outline = new Triangle();
            else if (toolStripButtonEllipse.Checked)
                _outline = new Ellipse();
            else if (toolStripButtonPointer.Checked)
            {
                _outline = new Rectangle();
                _outline.BorderSize = 1;
            }
          

            if (e.Button == MouseButtons.Left)
            {
                if (!_captureMouse)
                    return;

                _outline = CreateFrameShape(e.Location);
                _outline.Fill = false;
                _outline.Color = Color.LightGray;

               
                if (toolStripButtonPointer.Checked)
                {
                    foreach (var shape in _shapes
                        .Where(s => s.Intersects(_outline)))
                        shape.Selected = true;
                }
                
            }
            else if (_dragging && toolStripButtonPointer.Checked)
            {
                selectedShape.Location = new Point(e.Location.X + _dragPoint.X, e.Location.Y + _dragPoint.Y);

                if (selectedShape is Triangle)
                {
                    DragTriangle(e.Location);
                }
            }
            else
            {
                return;
            }
            Invalidate();
        }

        private void FormBoard_MouseUp(object sender, MouseEventArgs e)
        { 
            if (!_captureMouse)
                return;

           
            if (e.Button == MouseButtons.Left && _outline != null && !toolStripButtonPointer.Checked)
            {
                _outline.Fill = true;
                _outline.Selected = true;
                _outline.Color = Color.Blue;

                _shapes.Insert(0, _outline);
            }
            if (_dragging)
            {
                selectedShape.Location = new Point(e.Location.X + _dragPoint.X, e.Location.Y + _dragPoint.Y);
            }

            _dragging = false;
            Invalidate();
            RefreshArea();
            _captureMouse = false;

        }
        private void DragTriangle(Point pt)
        {
            selectedShape.Points[0] = new Point(pt.X + _dragPoint.X, pt.Y + _dragPoint.Y);
            selectedShape.Points[1] = new Point(pt.X + _dragPoint.X + selectedShape.Width, pt.Y + _dragPoint.Y);

            if (selectedShape.Points[2].Y > selectedShape.Points[0].Y)
            {
                selectedShape.Points[2] = new Point
                    ((pt.X + _dragPoint.X + selectedShape.Width - selectedShape.Width / 2),
                     pt.Y + _dragPoint.Y + selectedShape.Height);
            }
            else
            {
                selectedShape.Points[2] = new Point
                    ((pt.X + _dragPoint.X + selectedShape.Width - selectedShape.Width / 2),
                     pt.Y + _dragPoint.Y - selectedShape.Height);
            }
        }

        private void Properties()
        {
            var shape = _shapes
               .FirstOrDefault(r => r.Selected);

            if (shape != null)
            {
                var fc = new FormProperties(FormProperties.Style.Properties);
                fc.MyWidth = shape.Width;
                fc.MyHeight = shape.Height;
                fc.MyColor = shape.Color;

                if (fc.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        shape.Width = fc.MyWidth.Value;
                        shape.Height = fc.MyHeight.Value;
                        shape.Color = fc.MyColor.Value;
                        if (shape is Triangle)
                        {
                            DragTriangle(new Point(selectedShape.Points[2].X, selectedShape.Points[0].Y));
                        }
                    }
                    catch (InvalidValueException e)
                    {
                        MessageBox.Show(
                            e.Message,
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    
                }

                RefreshArea();
                Invalidate();
            }
        }

        private void DeleteSelected()
        {
            for (int i = _shapes.Count - 1; i >= 0; i--)
                if (_shapes[i].Selected)
                    _shapes.RemoveAt(i);

            RefreshArea();
            Invalidate();
        }

        private void FormBoard_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete)
            {
                return;
            }
            
            DeleteSelected();
        }
        private void RefreshArea()
        {
            var area = _shapes
                .Sum(s => s.Area);

            toolStripStatusLabelArea.Text = area.ToString();
        }

        private void deleteSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelected();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties();
        }

        private void FormBoard_DoubleClick(object sender, EventArgs e)
        {
            if (toolStripButtonPointer.Checked)
            {
                Properties();
            }
            return;
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _shapes.Clear();
            RefreshArea();
            Invalidate();
        }
        private void FormBoard_Load(object sender, EventArgs e)
        {
            toolStripButtonPointer.Checked = true;
            _outline = new Rectangle();

            if (!File.Exists("data"))
                return;

            var formatter = new BinaryFormatter();

            using (var stream = new FileStream("data", FileMode.Open))
            {
                _shapes = (List<Shape>)formatter.Deserialize(stream);
            }
        }

        private void FormBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            var formatter = new BinaryFormatter();

            using (var stream = new FileStream("data", FileMode.Create))
            {
                formatter.Serialize(stream, _shapes);
            }
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var selectedCount = _shapes
               .Count(r => r.Selected);

            deleteSelectionToolStripMenuItem.Enabled = selectedCount > 0;

            propertiesToolStripMenuItem.Enabled = selectedCount == 1;
        }
    }
}
