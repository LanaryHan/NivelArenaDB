using System;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Common
{
    public class AffineMap : MathFunction
    {
        public float A, B, C;

        public AffineMap(float a, float b, float c)
        {
            A = a;
            B = b;
            C = c;
        }

        public AffineMap(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            var ma = DenseMatrix.OfArray(new[,]
            {
                { x1 * x1, x1, 1 },
                { x2 * x2, x2, 1 },
                { x3 * x3, x3, 1 }
            });
            var mb = DenseVector.OfArray(new[] { y1, y2, y3 });
            var x = ma.Solve(mb);
            A = x[0];
            B = x[1];
            C = x[2];
        }

        public override float Evaluate(float x)
        {
            return A * MathF.Pow(x, 2) + B * x + C;
        }
    }
}