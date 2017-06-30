using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgeBuilder
{
    public static class PointExtensions
    {
        public static PointF Add(this PointF point, PointF v)
        {
            point.X += v.X;
            point.Y += v.Y;
            return point;
        }
        public static PointF Sub(this PointF point, PointF v)
        {
            point.X -= v.X;
            point.Y -= v.Y;
            return point;
        }
        public static float MagSq(this PointF point)
        {
            return point.X*point.X + point.Y*point.Y;
        }
        public static float Mag(this PointF point)
        {
            return (float)Math.Sqrt(point.MagSq());
        }
        public static PointF MultiplyScalar(this PointF point, float s)
        {
            point.X *= s;
            point.Y *= s;
            return point;
        }
    }
}
