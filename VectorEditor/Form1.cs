using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using VectorEditor.Models;
using System.Resources;
using System.Globalization;

namespace VectorEditor
{
    public partial class Form1 : Form
    {
        private List<Shape> _shapes = new List<Shape>();
        private System.Windows.Forms.Timer _timer;

        // Mouse interaction fields
        private Point _startPoint;
        private bool _isDrawing = false;
        private Shape _currentShape = null;

        // Settings
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string SelectedTool { get; set; } = "Rectangle";

        private Color _currentFillColor = Color.Blue;
        private bool _isAnimating = true;
        private string _currentCulture = "en";

        public Form1()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // Wire up mouse events manually
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;

            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 30;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void UpdateUILanguage(string cultureCode)
        {
            _currentCulture = cultureCode;

            var rm = new ResourceManager("VectorEditor.Strings", typeof(Form1).Assembly);
            var culture = CultureInfo.CreateSpecificCulture(cultureCode);

            fileToolStripMenuItem.Text = rm.GetString("MenuFile", culture);
            toolsToolStripMenuItem.Text = rm.GetString("MenuTools", culture);

            if (animationToolStripMenuItem != null)
                animationToolStripMenuItem.Text = rm.GetString("MenuAnim", culture);

            languageToolStripMenuItem.Text = rm.GetString("MenuLang", culture);

            helpToolStripMenuItem.Text = rm.GetString("MenuHelp", culture);

            saveToolStripMenuItem.Text = rm.GetString("ItemSave", culture);
            loadToolStripMenuItem.Text = rm.GetString("ItemLoad", culture);
            exitToolStripMenuItem.Text = rm.GetString("ItemExit", culture);

            rectangleToolStripMenuItem.Text = rm.GetString("ItemRect", culture);
            circleToolStripMenuItem.Text = rm.GetString("ItemCircle", culture);
            pickColorToolStripMenuItem.Text = rm.GetString("ItemColor", culture);

            if (startStopToolStripMenuItem != null)
                startStopToolStripMenuItem.Text = rm.GetString("ItemStartStop", culture);

            aboutToolStripMenuItem.Text = rm.GetString("ItemAbout", culture);

            this.Text = cultureCode == "bg" ? "Векторен Редактор" : "Vector Editor";
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isAnimating)
            {
                foreach (var shape in _shapes)
                {
                    shape.Move(this.ClientSize.Width, this.ClientSize.Height);
                }
                this.Invalidate();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            _isDrawing = true;
            _startPoint = e.Location;

            if (SelectedTool == "Circle")
            {
                _currentShape = new MyCircle();
            }
            else
            {
                _currentShape = new MyRectangle();
            }

            _currentShape.X = e.X;
            _currentShape.Y = e.Y;
            _currentShape.Width = 0;
            _currentShape.Height = 0;

            // Apply the selected color
            _currentShape.FillColor = _currentFillColor;

            _currentShape.Dx = 0;
            _currentShape.Dy = 0;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || _currentShape == null) return;

            _currentShape.X = Math.Min(_startPoint.X, e.X);
            _currentShape.Y = Math.Min(_startPoint.Y, e.Y);
            _currentShape.Width = Math.Abs(_startPoint.X - e.X);
            _currentShape.Height = Math.Abs(_startPoint.Y - e.Y);

            this.Invalidate();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDrawing && _currentShape != null)
            {
                if (_currentShape.Width > 5 && _currentShape.Height > 5)
                {
                    Random rnd = new Random();
                    _currentShape.Dx = rnd.Next(-5, 6);
                    _currentShape.Dy = rnd.Next(-5, 6);

                    if (_currentShape.Dx == 0) _currentShape.Dx = 2;
                    if (_currentShape.Dy == 0) _currentShape.Dy = 2;

                    _shapes.Add(_currentShape);
                }
            }

            _isDrawing = false;
            _currentShape = null;
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

            if (_isDrawing && _currentShape != null)
            {
                _currentShape.Draw(e.Graphics);
            }
        }

        // --- MENU EVENT HANDLERS ---

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveScene();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Stop();
            LoadScene();
            _timer.Start();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedTool = "Rectangle";
        }

        private void circleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedTool = "Circle";
        }

        // Color Picker Logic
        private void pickColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _currentFillColor = colorDialog1.Color;
            }
        }

        // Animation Toggle Logic
        private void startStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _isAnimating = !_isAnimating;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rm = new ResourceManager("VectorEditor.Strings", typeof(Form1).Assembly);
            var culture = System.Globalization.CultureInfo.CreateSpecificCulture(_currentCulture);

            string message = rm.GetString("MsgAbout", culture);
            string title = rm.GetString("ItemAbout", culture);

            MessageBox.Show(message, title);
        }

        // --- SERIALIZATION LOGIC ---

        private void SaveScene()
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "JSON Files|*.json";
            saveDialog.Title = "Save Project";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(_shapes, options);
                File.WriteAllText(saveDialog.FileName, jsonString);
            }
        }

        private void LoadScene()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "JSON Files|*.json";
            openDialog.Title = "Load Project";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string jsonString = File.ReadAllText(openDialog.FileName);
                try
                {
                    var loadedShapes = JsonSerializer.Deserialize<List<Shape>>(jsonString);
                    if (loadedShapes != null)
                    {
                        _shapes = loadedShapes;
                        this.Invalidate();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading file: " + ex.Message);
                }
            }
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateUILanguage("en");
        }

        private void българскиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateUILanguage("bg");
        }
    }
}