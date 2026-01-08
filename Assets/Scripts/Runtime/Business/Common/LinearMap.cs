using System;
using UnityEngine;

namespace Common
{
    public class LinearMap : MathFunction
    {
        
        /// <summary>
        /// 斜率
        /// </summary>
        private float K;
        
        /// <summary>
        /// 截距
        /// </summary>
        private float B;

        public LinearMap(float k, float b)
        {
            K = k;
            B = b;
        }
        
        public LinearMap(float x1, float y1, float x2, float y2)
        {
            if (Mathf.Approximately(x1, x2))
            {
                throw new Exception("x1 is equal to x2");
            }

            K = (y2 - y1) / (x2 - x1);
            B = y1 - K * x1;
        }

        public override float Evaluate(float x)
        {
            return K * x + B;
        }
    }
}