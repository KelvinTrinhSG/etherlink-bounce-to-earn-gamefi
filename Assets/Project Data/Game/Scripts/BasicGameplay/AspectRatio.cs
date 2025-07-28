using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AspectRatio
{
    public static Ratio GetRatio(Camera camera)
    {
        double aspect = System.Convert.ToDouble(camera.aspect.ToString("0.00"));

        if (aspect == 1.33)
        {
            return Ratio.AspectRatio4x3;
        }
        else if (aspect == 1.5)
        {
            return Ratio.AspectRatio3x2;
        }
        else if (aspect == 1.6)
        {
            return Ratio.AspectRatio16x10;
        }
        else if (aspect == 1.67)
        {
            return Ratio.AspectRatio5x3;
        }
        else if (aspect == 1.78)
        {
            return Ratio.AspectRatio16x9;
        }
        else if (aspect >= 1.83)
        {
            return Ratio.AspectRatio18x9;
        }

        return Ratio.AspectRatio4x3;
    }

    public enum Ratio
    {
        AspectRatio18x9,
        AspectRatio16x10,
        AspectRatio16x9,
        AspectRatio5x3,
        AspectRatio3x2,
        AspectRatio4x3
    }
}
