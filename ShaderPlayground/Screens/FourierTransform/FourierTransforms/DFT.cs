using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShaderPlayground.Screens.FourierTransform.FourierTransforms
{
    public class DFT
    {

        public Complex[,] CreateDFT(Complex[,] input)
        {
            int width = input.GetLength(0);
            int height = input.GetLength(1);
            
            Complex[,] output = new Complex[width, height];

            for (int u = 0; u < width; u++)
            {
                for (int v = 0; v < height; v++)
                {
                        //Sum
                    Complex funcVal = input[u, v];
                    Complex sum = 0.0;

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {

                            sum += GetForwardTerm(u, v, x, y, width, height);
                        }
                    }

                    output[u, v] = sum * funcVal * (1.0d / (width * height));
                }
            }

            return output;
        }

        private Complex GetForwardTerm(int u, int v, int x, int y, double width, double height)
        {
            //e ^ -i 2 * PI (x  * u / M + y / v / N)
            // ->

            Complex output = new Complex(
                Math.Cos(2 * Math.PI * (x * u / width + y * v / height)),
                 - Math.Sin(2 * Math.PI * (x * u / width + y * v / height))
                );
            return output;
        }
    }
}
