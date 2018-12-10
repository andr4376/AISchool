using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIFramework
{
    [Serializable]
    public class AIVector
    {
        public float X;
        public float Y;

        public AIVector(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public AIVector Normalize()
        {
            double distance = Math.Sqrt(X*X + Y*Y);
            return new AIVector((float)(X / distance), (float)(Y / distance));
        }

        public static AIVector operator +(AIVector vector, float other)
        {
            return new AIVector(vector.X + other, vector.Y + other);
        }

        public static AIVector operator -(AIVector vector, float other)
        {
            return new AIVector(vector.X - other, vector.Y - other);
        }

        public static AIVector operator *(AIVector vector, float other)
        {
            return new AIVector(vector.X * other, vector.Y * other);
        }

        public static AIVector operator +(AIVector vector, AIVector other)
        {
            return new AIVector(vector.X + other.X, vector.Y + other.Y);
        }

        public static AIVector operator -(AIVector vector, AIVector other)
        {
            return new AIVector(vector.X - other.X, vector.Y - other.Y);
        }



        public static float Distance(AIVector vector1, AIVector vector2)
        {
            float a = Math.Abs(vector1.X - vector2.X);
            float b = Math.Abs(vector1.Y - vector2.Y);
            return (float)Math.Sqrt(a*a + b*b);
        }
    }
}
