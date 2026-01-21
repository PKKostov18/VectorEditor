using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using VectorEditor.Models;

namespace VectorEditor
{
    public partial class Form1 : Form
    {
        private List<Shape> _shapes = new List<Shape>();
        private System.Windows.Forms.Timer _timer;

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            InitializeDefaultShapes();
            InitializeTimer();
        }

        private void InitializeDefaultShapes()
        {
            _shapes.Add(new MyRectangle
            {
                X = 50,
                Y = 50,
                Width = 100,
                Height = 60,
                FillColor = Color.Red,
                Dx = 3,
                Dy = 3
            });

            _shapes.Add(new MyCircle
            {
                X = 200,
                Y = 100,
                Width = 80,
                Height = 80,
                FillColor = Color.Green,
                Dx = -4,
                Dy = 2
            });
        }

        private void InitializeTimer()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 30;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            foreach (var shape in _shapes)
            {
                shape.Move(this.ClientSize.Width, this.ClientSize.Height);
            }
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var shape in _shapes)
            {
                shape.Draw(e.Graphics);
            }
        }
    }
}