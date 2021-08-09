using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Asm3
{
    class Program
    {

        static KeyValuePair<double, double> Encode(string content)
        {
            double high = 1;
            double low = 0;

            content = content.ToLowerInvariant();

            foreach (var c in content)
            {
                double cdf;
                if (c == 'a')
                {
                    cdf = 0.5;
                }
                else if (c == 'b')
                {
                    cdf = 1;
                }
                else
                {
                    throw new Exception("Input contain characters other than 'a' and 'b'.");
                }
                double range = high - low;
                high = low + range * cdf;
                low = low + range * (cdf - 0.5);
                Console.WriteLine($"[{low}, {high})");
            }
            return new KeyValuePair<double, double>(low, high);
        }

        static double[,] Transpose(double[,] matrix)
        {
            int len = (int)Math.Sqrt(matrix.Length);
            double[,] res = new double[len, len];
            for (int i = 0; i < len; ++i)
            {
                for (int j = 0; j < len; ++j)
                {
                    res[i, j] = matrix[j, i];
                }
            }
            return res;
        }

        static double[,] Multiply(double[,] A, double[,] B)
        {
            int len = (int)Math.Sqrt(A.Length);
            double[,] res = new double[len, len];

            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < len; j++)
                {
                    res[i, j] = 0;
                    for (int k = 0; k < len; k++)
                    {
                        res[i, j] += A[i, k] * B[k, j];
                    }
                }
            }
            return res;
        }

        static double[,] GetDCTTransformMatrix(int n)
        {
            double[,] dct = new double[n, n];

            for (int i = 0; i < n; ++i)
            {
                double a = Math.Sqrt((i == 0 ? 1d : 2d) / n);

                for (int j = 0; j < n; ++j)
                {
                    dct[i, j] = a * Math.Cos(((2d * j + 1d) * i * Math.PI) / (2d * n));
                }
            }
            return dct;
        }

        static int[,] Round(double[,] matrix, int n)
        {
            int[,] res = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    res[i, j] = (int)Math.Round(matrix[i, j]);
                }
            }
            return res;
        }

        static string MPrint(int[,] matrix, int n)
        {
            string res = "{\n";
            for (int i = 0; i < n; i++)
            {
                res += "\t{ ";
                for (int j = 0; j < n; j++)
                {
                    res += matrix[i, j];
                    res += ",\t";
                }
                res += "\t},\n";
            }
            res += "}";
            return res;
        }

        static string MPrint(double[,] matrix, int n)
        {
            string res = "{\n";
            for (int i = 0; i < n; i++)
            {
                res += "\t{ ";
                for (int j = 0; j < n; j++)
                {
                    res += matrix[i, j];
                    res += ",\t";
                }
                res += "\t},\n";
            }
            res += "}";
            return res;
        }

        static string MPrint2(int[,] matrix, int n)
        {
            string res = "";
            for (int i = 0; i < n; i++) 
            {
            
                for (int j = 0; j < n; j++)
                {
                    res += matrix[i, j].ToString("0.####");
                    res += "&";
                }
                res += "\\\\\n";
            }
            return res;
        }

        static double[,] RandomMatrix(int n)
        {
            var rnd = new Random();
            double[,] matrix = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = rnd.Next(0, 255);
                }

            }
            return matrix;
        }

        static int[,] DCTRow(double[,] input, int n)
        {
            var T = GetDCTTransformMatrix(n);

            var TT = Transpose(T);
            var B = Multiply(input, TT);

            return Round(Multiply(T, B), n);
        }


        static int[,] DCTCol(double[,] input, int n)
        {
            var dct = GetDCTTransformMatrix(n);

            var A = Multiply(dct, input);
            var TT = Transpose(dct);

            return Round(Multiply(A, TT), n);
        }

        static double[,] ReadInput(string filename, out int size)
        {
            var lines = File.ReadAllLines(filename);
            size = lines.Length;
            double[,] res = new double[size, size];
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                Regex.Replace(line, @"\s+", " ");
                var eles = line.Split(" ");
                if (eles.Length != size)
                {
                    throw new InvalidOperationException("Input format invalid");
                }
                for (int j = 0; j < eles.Length; ++j)
                {
                    res[i, j] = double.Parse(eles[j]);
                }
            }
            return res;
        }


        static void Main(string[] args)
        {
            var method = args[0].Trim().ToLowerInvariant();
            var filename = args[1];

            if (method == "q1")
            {
                var line = File.ReadAllLines(filename)[0];
                var pair = Encode(line);
                Console.WriteLine($"Final result: Low - {pair.Key}, High - {pair.Value}");
            }
            else if (method == "q2")
            {
                var operation = args[2].Trim().ToLowerInvariant();

                int size = 8;
                var input = ReadInput(filename, out size);
                int[,] output = null;

                if (operation == "column")
                {
                    output = DCTCol(input, size);

                }
                else if (operation == "row")
                {

                    output = DCTRow(input, size);
                }

                Console.WriteLine("Input:");
                Console.WriteLine(MPrint(input, size));
                Console.WriteLine("Output:");
                Console.WriteLine(MPrint(output, size));
            }


            //double[,] matrix =  {
            //  { 255, 255, 255, 255, 255, 255, 255, 255 },
            //  { 255, 255, 255, 255, 255, 255, 255, 255 },
            //  { 255, 255, 255, 255, 255, 255, 255, 255 },
            //  { 255, 255, 255, 255, 255, 255, 255, 255 },
            //  { 255, 255, 255, 255, 255, 255, 255, 255 },
            //  { 255, 255, 255, 255, 255, 255, 255, 255 },
            //  { 255, 255, 255, 255, 255, 255, 255, 255 },
            //  { 255, 255, 255, 255, 255, 255, 255, 255 }
            //};

            //double [, ] matrix2 = {
            //    { 89,78,76,75,70,82,81,82},
            //    { 122,95,86,80,80,76,74,81},
            //    { 184,153,126,106,85,76,71,75},
            //    { 221,205,180,146,97,71,68,67},
            //    { 225,222,217,194,144,95,78,82},
            //    { 228,225,227,220,193,146,110,108},
            //    { 223,224,225,224,220,197,156,120},
            //    { 217,219,219,224,230,220,197,151}
            //};

            //var res = DCTRow(matrix2, 8);
            //Console.WriteLine(MPrint(res, 8));
        }
    }
}
