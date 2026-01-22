using System.Drawing;

namespace VectorEditor.Models
{
    public class MyCircle : Shape
    {
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