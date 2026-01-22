using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using VectorEditor.Models;
using System.Resources;
using System.Globalization;

// Student: Пламен Костов
// FN: F113851
// Project: Vector Editor with Animation & Serialization

namespace VectorEditor
{
    public partial class Form1 : Form
    {
        // Collection to store all shapes in the scene
        private List<Shape> _shapes = new List<Shape>();

        // Timer for handling the animation loop
        private System.Windows.Forms.Timer _timer;

        // Variables for mouse interaction (drawing new shapes)
        private Point _startPoint;
        private bool _isDrawing = false;
        private Shape _currentShape = null;

        // Editor Settings
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public string SelectedTool { get; set; } = "Rectangle";

        private Color _currentFillColor = Color.Blue; // Currently selected color
        private bool _isAnimating = true;             // Animation state flag
        private string _currentCulture = "en";        // Current language code

        public Form1()
        {
            InitializeComponent();

            // Enable Double Buffering to prevent flickering during animation
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // Manually wire up mouse events for drawing
            this.MouseDown += Form1_MouseDown;
            this.MouseMove += Form1_MouseMove;
            this.MouseUp += Form1_MouseUp;

            InitializeTimer();
        }

        /// <summary>
        /// Initializes and starts the animation timer.
        /// </summary>
        private void InitializeTimer()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 30; // ~30 FPS
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        /// <summary>
        /// Main game loop. Moves shapes and refreshes the screen.
        /// </summary>
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_isAnimating)
            {
                foreach (var shape in _shapes)
                {
                    shape.Move(this.ClientSize.Width, this.ClientSize.Height);
                }
                this.Invalidate(); // Triggers OnPaint
            }
        }

        // --- MOUSE EVENT HANDLERS ---

        /// <summary>
        /// Starts the drawing process when mouse button is pressed.
        /// </summary>
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            _isDrawing = true;
            _startPoint = e.Location;

            // Factory logic: Create shape based on selected tool
            if (SelectedTool == "Circle")
            {
                _currentShape = new MyCircle();
            }
            else
            {
                _currentShape = new MyRectangle();
            }

            // Initialize shape properties
            _currentShape.X = e.X;
            _currentShape.Y = e.Y;
            _currentShape.Width = 0;
            _currentShape.Height = 0;
            _currentShape.FillColor = _currentFillColor;
            _currentShape.Dx = 0;
            _currentShape.Dy = 0;
        }

        /// <summary>
        /// Updates the size of the shape being drawn while dragging.
        /// </summary>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDrawing || _currentShape == null) return;

            // Calculate position and dimensions allowing dragging in any direction
            _currentShape.X = Math.Min(_startPoint.X, e.X);
            _currentShape.Y = Math.Min(_startPoint.Y, e.Y);
            _currentShape.Width = Math.Abs(_startPoint.X - e.X);
            _currentShape.Height = Math.Abs(_startPoint.Y - e.Y);

            this.Invalidate();
        }

        /// <summary>
        /// Finalizes the drawing process and adds the shape to the list.
        /// </summary>
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_isDrawing && _currentShape != null)
            {
                // Only add if the shape has a significant size
                if (_currentShape.Width > 5 && _currentShape.Height > 5)
                {
                    // Assign random velocity for the animation
                    Random rnd = new Random();
                    _currentShape.Dx = rnd.Next(-5, 6);
                    _currentShape.Dy = rnd.Next(-5, 6);

                    // Ensure the shape isn't stationary
                    if (_currentShape.Dx == 0) _currentShape.Dx = 2;
                    if (_currentShape.Dy == 0) _currentShape.Dy = 2;

                    _shapes.Add(_currentShape);
                }
            }

            _isDrawing = false;
            _currentShape = null;
            this.Invalidate();
        }

        /// <summary>
        /// Renders all shapes to the form using GDI+.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Draw existing shapes
            foreach (var shape in _shapes)
            {
                shape.Draw(e.Graphics);
            }

            // Draw the shape currently being created (ghost shape)
            if (_isDrawing && _currentShape != null)
            {
                _currentShape.Draw(e.Graphics);
            }
        }

        // --- MENU & UI LOGIC ---

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveScene();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _timer.Stop(); // Pause animation during load
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

        private void pickColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                _currentFillColor = colorDialog1.Color;
            }
        }

        private void startStopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _isAnimating = !_isAnimating;
        }

        /// <summary>
        /// Displays information about the author, localized based on current culture.
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var rm = new ResourceManager("VectorEditor.Strings", typeof(Form1).Assembly);
            var culture = CultureInfo.CreateSpecificCulture(_currentCulture);

            string message = rm.GetString("MsgAbout", culture);
            string title = rm.GetString("ItemAbout", culture);

            MessageBox.Show(message, title);
        }

        // --- LOCALIZATION LOGIC ---

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateUILanguage("en");
        }

        private void bulgarianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateUILanguage("bg");
        }

        /// <summary>
        /// Updates all UI text elements based on the selected culture (en/bg).
        /// </summary>
        private void UpdateUILanguage(string cultureCode)
        {
            _currentCulture = cultureCode;
            var rm = new ResourceManager("VectorEditor.Strings", typeof(Form1).Assembly);
            var culture = CultureInfo.CreateSpecificCulture(cultureCode);

            // Menu Items
            fileToolStripMenuItem.Text = rm.GetString("MenuFile", culture);
            toolsToolStripMenuItem.Text = rm.GetString("MenuTools", culture);

            if (animationToolStripMenuItem != null)
                animationToolStripMenuItem.Text = rm.GetString("MenuAnim", culture);

            languageToolStripMenuItem.Text = rm.GetString("MenuLang", culture);
            helpToolStripMenuItem.Text = rm.GetString("MenuHelp", culture);

            // Sub-menus
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

        // --- SERIALIZATION (JSON) ---

        /// <summary>
        /// Serializes the list of shapes to a JSON file.
        /// </summary>
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

        /// <summary>
        /// Deserializes shapes from a JSON file and refreshes the scene.
        /// </summary>
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
    }
}