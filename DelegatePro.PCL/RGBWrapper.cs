using System;
using System.Globalization;

namespace DelegatePro.PCL
{
    public class RGBWrapper : IEquatable<RGBWrapper>
    {
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }
        public int A { get; set; } = 255;

        public RGBWrapper(int r, int g, int b)
        {
            R = r;
            B = b;
            G = g;
        }

        public RGBWrapper(int r, int g, int b, int a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Croc.PCL.RGBWrapper"/> class.
        /// </summary>
        /// <param name="hex">#115588</param>
        public RGBWrapper(string hex)
        {
            this.R = int.Parse(hex.Substring(1, 2), NumberStyles.HexNumber);
            this.G = int.Parse(hex.Substring(3, 2), NumberStyles.HexNumber);
            this.B = int.Parse(hex.Substring(5, 2), NumberStyles.HexNumber);
        }

        public bool Equals(RGBWrapper other)
        {
            return other.R == this.R && other.G == this.G && other.B == this.B && other.A == this.A;
        }
    }
}