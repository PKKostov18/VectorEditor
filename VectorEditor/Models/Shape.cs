using System.Drawing;
using System.Text.Json.Serialization;

namespace VectorEditor.Models
{
    [JsonDerivedType(typeof(MyRectangle), typeDiscriminator: "rect")]
    [JsonDerivedType(typeof(MyCircle), typeDiscriminator: "circle")]
    public abstract class Shape
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        [JsonIgnore]
        public Color FillColor { get; set; } = Color.Blue;

        public string ColorHtml
        {
            get => ColorTranslator.ToHtml(FillColor);
            set => FillColor = ColorTranslator.FromHtml(value);
        }

        public float Dx { get; set; } = 2;
        public float Dy { get; set; } = 2;

        public abstract void Draw(Graphics g);

        public virtual void Move(int canvasWidth, int canvasHeight)
        {
            X += Dx;
            Y += Dy;

            if (X < 0 || X + Width > canvasWidth) Dx = -Dx;
            if (Y < 0 || Y + Height > canvasHeight) Dy = -Dy;
        }
    }
}