/*

Forked by Luciano Ferraro

*/

using System;

namespace Aiv.Engine
{
    public class Utils
    {
        public static float ConvertDegreeToRadians(int degree)
        {
            return (float) (Math.PI*degree/180f);
        }
    }
}