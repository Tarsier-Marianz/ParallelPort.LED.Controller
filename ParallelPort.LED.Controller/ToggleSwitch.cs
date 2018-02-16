using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ParallelPort.LED.Controller {
    public class ToggleSwitch : CheckBox {
        public ToggleSwitch() {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            Padding = new Padding(6);
        }

        private Color _checkedColor = Color.Green;
        private Color _uncheckedColor = Color.White;
        private bool _inlineCircle = false;

        [Browsable(true)]
        [Description("Checkbox UI display"), Category("CheckBox")]
        public bool InlineCircle {
            get {
                return this._inlineCircle;
            }
            set {
                this._inlineCircle = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or Sets the color of the LED light
        /// </summary>
        [DefaultValue(typeof(Color), "24, 106, 59")]
        [Description("Circle color if checked"), Category("CheckBox")]
        public Color CheckedColor {
            get { return this._checkedColor; }
            set {
                this._checkedColor = value;
                this.Invalidate();	// Redraw the control
            }
        }

        /// <summary>
        /// Gets or Sets the color of the LED light OFF
        /// </summary>
        //[DefaultValue(typeof(Color), "255, 255, 255")]
         [Description("Circle color if unchecked"), Category("CheckBox")]
        public Color UncheckedColor {
            get { return this._uncheckedColor; }
            set {
                this._uncheckedColor = value;
                this.Invalidate();	// Redraw the control
            }
        }
        protected override void OnPaint(PaintEventArgs e) {
            this.OnPaintBackground(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = new GraphicsPath()) {
                var d = Padding.All;
                var r = this.Height - 2 * d;
                Rectangle rect ;
                path.AddArc(d, d, r, r, 90, 180);
                path.AddArc(this.Width - r - d, d, r, r, -90, 180);
                path.CloseFigure();
                e.Graphics.FillPath(Checked ? Brushes.DarkGray : Brushes.LightGray, path);
                if (this._inlineCircle) {
                    r = (Height / 2);
                    rect = Checked ? new Rectangle(Width - (r + d + 2), r / 2, r, r)
                                   : new Rectangle(d + 2, r / 2, r, r);
                } else {
                    r = Height - 1;
                    rect = Checked ? new Rectangle(Width - r - 1, 0, r, r)
                                        : new Rectangle(0, 0, r, r);
                }
                Brush brushChecked = new SolidBrush(this._checkedColor);
                Brush brushUnchecked = new SolidBrush(this._uncheckedColor);
                e.Graphics.FillEllipse(Checked ? brushChecked : brushUnchecked, rect);               
            }
        }
    }
}