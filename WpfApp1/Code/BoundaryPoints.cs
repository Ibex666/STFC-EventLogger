using System;
using System.Collections.Generic;

namespace STFC_EventLogger
{
    public class BoundaryPoints : IEquatable<BoundaryPoints?>
    {
        public BoundaryPoints() { }
        public BoundaryPoints(int x1, int x2, int x3, int x4)
        {
            X1 = x1;
            X2 = x2;
            X3 = x3;
            X4 = x4;
        }

        /// <summary>
        /// point left from Alliance/Event Member Name
        /// </summary>
        public int X1 { get; set; }

        /// <summary>
        /// point right from Alliance/Event Member Name
        /// </summary>
        public int X2 { get; set; }

        /// <summary>
        /// point left from Alliance/Event Member Power/Score
        /// </summary>
        public int X3 { get; set; }

        /// <summary>
        /// point right from Alliance/Event Member Power/Score
        /// </summary>
        public int X4 { get; set; }

        /// <summary>
        /// width between X1 and X4
        /// </summary>
        public int Width => X4 - X1;

        /// <summary>
        /// width between X1 and X2
        /// </summary>
        public int WidthX1X2 => X2 - X1;

        /// <summary>
        /// width between X3 and X4
        /// </summary>
        public int WidthX3X4 => X4 - X3;

        public override string? ToString()
        {
            return $"{X1}, {X2}, {X3}, {X4}, {Width}, {WidthX1X2}, {WidthX3X4}";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BoundaryPoints);
        }

        public bool Equals(BoundaryPoints? other)
        {
            return other is not null &&
                   X1 == other.X1 &&
                   X2 == other.X2 &&
                   X3 == other.X3 &&
                   X4 == other.X4 &&
                   WidthX1X2 == other.WidthX1X2 &&
                   WidthX3X4 == other.WidthX3X4;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X1, X2, X3, X4, WidthX1X2, WidthX3X4);
        }

        public static bool operator ==(BoundaryPoints? left, BoundaryPoints? right)
        {
            return EqualityComparer<BoundaryPoints>.Default.Equals(left, right);
        }

        public static bool operator !=(BoundaryPoints? left, BoundaryPoints? right)
        {
            return !(left == right);
        }

        public static implicit operator BoundaryPoints(string value)
        {
            var split = value.Split(',');
            return new BoundaryPoints()
            {
                X1 = int.Parse(split[0].Trim()),
                X2 = int.Parse(split[1].Trim()),
                X3 = int.Parse(split[2].Trim()),
                X4 = int.Parse(split[3].Trim())
            };
        }
    }
}
