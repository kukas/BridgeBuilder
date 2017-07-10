using System;
using System.Drawing;

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
            return point.X * point.X + point.Y * point.Y;
        }
        public static float Mag(this PointF point)
        {
            return (float)Math.Sqrt(point.MagSq());
        }
        public static PointF Normalize(this PointF point)
        {
            return point.DivideScalar(point.Mag());
        }
        public static PointF MultiplyScalar(this PointF point, float s)
        {
            point.X *= s;
            point.Y *= s;
            return point;
        }
        public static PointF DivideScalar(this PointF point, float s)
        {
            point.X /= s;
            point.Y /= s;
            return point;
        }
        public static float Dot(this PointF point, PointF v)
        {
            return point.X * v.X + point.Y * v.Y;
        }
    }
}
