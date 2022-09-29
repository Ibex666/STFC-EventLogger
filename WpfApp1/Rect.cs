using System;
using System.Collections.Generic;

namespace STFC_EventLogger
{
    public class Rect : IEquatable<Rect?>
    {
        public Rect() { }
        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int X1 => X;
        public int Y1 => Y;
        public int X2 => X + Width;
        public int Y2 => Y + Height;
        public int Width { get; set; }
        public int Height { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Rect);
        }

        public bool Equals(Rect? other)
        {
            return other is not null &&
                   X == other.X &&
                   Y == other.Y &&
                   Width == other.Width &&
                   Height == other.Height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height);
        }

        public override string? ToString()
        {
            return $"{X}, {Y}, {Width}, {Height}";
        }

        public static bool operator ==(Rect? left, Rect? right)
        {
            return EqualityComparer<Rect>.Default.Equals(left, right);
        }

        public static bool operator !=(Rect? left, Rect? right)
        {
            return !(left == right);
        }

        public static implicit operator Tesseract.Rect(Rect r)
        {
            return new Tesseract.Rect(r.X, r.Y, r.Width, r.Height);
        }
    }
}
