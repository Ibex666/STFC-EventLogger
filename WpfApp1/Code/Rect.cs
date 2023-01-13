using System;
using System.Collections.Generic;

namespace STFC_EventLogger
{
    public class Rect : IEquatable<Rect?>
    {
        public Rect() { Start = new Point(); End = new Point(); }
        public Rect(int x, int y, int width, int height)
        {
            Start = new Point(x, y);
            End = new Point(x + width, y + height);
        }

        public Point Start { get; set; }
        public Point End { get; set; }
        public int Width => End.X - Start.X;
        public int Height => End.Y - Start.Y;
        public int X => Start.X;
        public int Y => Start.Y;
        public int X1 => Start.X;
        public int Y1 => Start.Y;
        public int X2 => End.X;
        public int Y2 => End.Y;

        public override bool Equals(object? obj)
        {
            return Equals(obj as Rect);
        }

        public bool Equals(Rect? other)
        {
            return other is not null &&
                   Start == other.Start &&
                   End == other.End;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start.GetHashCode(), End.GetHashCode());
        }

        public override string? ToString()
        {
            return $"{Start.X}, {Start.Y}, {End.X}, {End.Y}";
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
            return new Tesseract.Rect(r.Start.X, r.Start.Y, r.Width, r.Height);
        }
        public static implicit operator Rect(string value)
        {
            var split = value.Split(',');
            Rect r = new();
            r.Start.X = int.Parse(split[0].Trim());
            r.Start.Y = int.Parse(split[1].Trim());
            r.End.X = int.Parse(split[2].Trim());
            r.End.Y = int.Parse(split[3].Trim());
            return r;
        }
    }
}
