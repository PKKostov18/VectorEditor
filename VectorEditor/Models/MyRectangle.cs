using System.Drawing;

namespace VectorEditor.Models
{
    public class MyRectangle : Shape
    {
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