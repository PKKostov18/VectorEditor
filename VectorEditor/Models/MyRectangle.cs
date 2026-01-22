using System.Drawing;

// Student: Пламен Костов
// FN: F113851

namespace VectorEditor.Models
{
    /// <summary>
    /// Concrete implementation of a Rectangle shape.
    /// </summary>
    public class MyRectangle : Shape
    {
        /// <summary>
        /// Draws a filled rectangle with a black outline.
        /// </summary>
        public override void Draw(Graphics g)
        {
            using (Brush brush = new SolidBrush(FillColor))
            {
                g.FillRectangle(brush, X, Y, Width, Height);
            }
            using (Pen pen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(pen, X, Y, Width, Height);
            }
        }
    }
}