using System.Drawing;

// Student: Пламен Костов
// FN: F113851

namespace VectorEditor.Models
{
    /// <summary>
    /// Concrete implementation of a Circle (Ellipse) shape.
    /// </summary>
    public class MyCircle : Shape
    {
        /// <summary>
        /// Draws a filled ellipse/circle with a black outline.
        /// </summary>
        public override void Draw(Graphics g)
        {
            using (Brush brush = new SolidBrush(FillColor))
            {
                g.FillEllipse(brush, X, Y, Width, Height);
            }
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawEllipse(pen, X, Y, Width, Height);
            }
        }
    }
}