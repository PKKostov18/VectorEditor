using System.Drawing;
using System.Text.Json.Serialization;

// Student: Пламен Костов
// FN: F113851
// Course: CSCB579

namespace VectorEditor.Models
{
    /// <summary>
    /// Abstract base class representing a geometric shape.
    /// Defines common properties like position, size, color, and movement logic.
    /// </summary>
    [JsonDerivedType(typeof(MyRectangle), typeDiscriminator: "rect")]
    [JsonDerivedType(typeof(MyCircle), typeDiscriminator: "circle")]
    public abstract class Shape
    {
        // Properties for position and size
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        // Color property ignored by JSON, as we serialize the HTML string instead
        [JsonIgnore]
        public Color FillColor { get; set; } = Color.Blue;

        /// <summary>
        /// Property used for JSON serialization to store Color as a HEX string.
        /// </summary>
        public string ColorHtml
        {
            get => ColorTranslator.ToHtml(FillColor);
            set => FillColor = ColorTranslator.FromHtml(value);
        }

        // Movement vector (speed and direction)
        public float Dx { get; set; } = 2;
        public float Dy { get; set; } = 2;

        /// <summary>
        /// Abstract method that must be implemented by derived classes to draw specific shapes.
        /// </summary>
        public abstract void Draw(Graphics g);

        /// <summary>
        /// Calculates new position based on speed and handles wall collisions (bouncing).
        /// </summary>
        public virtual void Move(int canvasWidth, int canvasHeight)
        {
            X += Dx;
            Y += Dy;

            // Check for collision with left/right walls
            if (X < 0 || X + Width > canvasWidth) Dx = -Dx;

            // Check for collision with top/bottom walls
            if (Y < 0 || Y + Height > canvasHeight) Dy = -Dy;
        }
    }
}