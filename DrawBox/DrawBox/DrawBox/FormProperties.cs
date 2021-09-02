using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawBox
{
    public partial class FormProperties : Form
    {
        public FormProperties()
        {
            InitializeComponent();
        }

        public enum Style
        {
            Properties,
            Select
        }

        public FormProperties(Style style)
        {
            InitializeComponent();

            switch (style)
            {
                case Style.Properties:
                    Text = "Properties";
                    buttonOK.Text = "OK";
                    break;
                case Style.Select:
                    Text = "Select";
                    buttonOK.Text = "Select";
                    break;
            }
        }
        private int? _width;
        private int? _height;
        private Color? _color;

        public int? MyWidth
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                textBoxWidth.Text = _width.ToString();
            }
        }
        public int? MyHeight
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                textBoxHeight.Text = _height.ToString();
            }
        }

        public Color? MyColor
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                buttonColor.BackColor = _color ?? BackColor;
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            _width = int.TryParse(textBoxWidth.Text, out int width)
                ? width
                : (int?)null;

            _height = int.TryParse(textBoxHeight.Text, out int height)
                ? height
                : (int?)null;

            DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            var cd = new ColorDialog();
            MyColor = cd.ShowDialog() == DialogResult.OK
                ? cd.Color
                : (Color?)null;

        }
    }
}
