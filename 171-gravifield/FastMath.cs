using System;

namespace _171_gravifield
{
    public class FastMath
    {
        private static int ATAN2_BITS = 7;

        private static int ATAN2_BITS2 = ATAN2_BITS << 1;
        private static int ATAN2_MASK = ~(-1 << ATAN2_BITS2);
        private static int ATAN2_COUNT = ATAN2_MASK + 1;
        private static int ATAN2_DIM = (int)Math.Sqrt(ATAN2_COUNT);

        private static double INV_ATAN2_DIM_MINUS_1 = 1.0f / (ATAN2_DIM - 1);
        private static double DEG = 180.0f / (float)Math.PI;

        private static double[] atan2 = new double[ATAN2_COUNT];



        public FastMath()
   {
      for (int i = 0; i<ATAN2_DIM; i++)
      {
         for (int j = 0; j<ATAN2_DIM; j++)
         {
            double x0 = (double)i / ATAN2_DIM;
        double y0 = (double)j / ATAN2_DIM;

        atan2[j * ATAN2_DIM + i] =  Math.Atan2(y0, x0);
    }
}
   }


   /**
    * ATAN2
    */


public double calcatan2(double y, double x)
{
    float add, mul;

    if (x < 0.0f)
    {
        if (y < 0.0f)
        {
            x = -x;
            y = -y;

            mul = 1.0f;
        }
        else
        {
            x = -x;
            mul = -1.0f;
        }

        add = -3.141592653f;
    }
    else
    {
        if (y < 0.0f)
        {
            y = -y;
            mul = -1.0f;
        }
        else
        {
            mul = 1.0f;
        }

        add = 0.0f;
    }

    double invDiv = 1.0f / (((x < y) ? y : x) * INV_ATAN2_DIM_MINUS_1);

    int xi = (int)(x * invDiv);
    int yi = (int)(y * invDiv);

    return (atan2[yi * ATAN2_DIM + xi] + add) * mul;
}
    }
}