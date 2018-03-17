using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Composition110110
{
    internal static class MyOperations
    {
        internal static Rgba32 Darken(this Rgba32 c1, Rgba32 c2)
        {
            return new Rgba32(
                Math.Min(c1.R, c2.R),
                Math.Min(c1.G, c2.G),
                Math.Min(c1.B, c2.B)
            );
        }

        internal static bool PartiallyBrighterThan(this Rgba32 first, Rgba32 second)
        {
            return first.R > second.R || first.G > second.G || first.B > second.B;
        }
    }
}
