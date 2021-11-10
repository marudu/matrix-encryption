using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    class Program
    {
        private static List<EncryptionResult> EncryptionResult = new List<EncryptionResult>();

        static void Main(string[] args)
        {
            //till 9x9 matrix working
            //string input = "hi i am cool";

            //16x16 matrix
            string input = "Writers write descriptive paragraphs because their purpose is to describe something. Their point is that something is beautiful or disgusting or strangely intriguing. Writers write persuasive and argument paragraphs because their purpose is to persuade or convince someone. Their point is that their reader should see things a particular way and possibly take action on that new way of seeing things. Writers write paragraphs of comparison because the comparison will make their point clear to their readers. Writers write descriptive paragraphs because their purpose is to describe something. Their point is that something is beautiful or disgusting or strangely intriguing. Writers write persuasive and argument paragraphs because their purpose is to persuade or convince someone. Their point is that their reader should see things a particular way and possibly take action on that new way of seeing things. Writers write paragraphs of comparison because the comparison will make their point clear to their readers.";

            //encrypt
            Encryption(input);

            //decrypt
            Decryption(EncryptionResult);


            Console.ReadKey();

            //Console.WriteLine("Please enter a string");

            //string source = Console.ReadLine();

        }

        private static void Encryption(string source)
        {


            IEnumerable<string> s = SplitByLength(source, 256);

            //convert IEnumerable to List
            List<string> list = new List<string>(s);


            List<string> en = new List<string>();

            en.Clear();
            EncryptionResult.Clear();

            foreach (string input in list)
            {
                Console.WriteLine(input);

                var encryptionResult = Encrypt(input);


                //Output String
                en.Add(encryptionResult.Result);

                //Output EncryptionResult
                EncryptionResult.Add(encryptionResult);

                //Console.WriteLine($"encrypted result {encryptionResult.Result}");

                //Console.WriteLine($"Decryption starts");

                //Decrpt(encryptionResult);

            }
            var result = String.Join("", en.ToArray());

            Console.WriteLine(result);
        }

        private static void Decryption(List<EncryptionResult> el)
        {
            List<string> de = new List<string>();
            de.Clear();

            foreach (EncryptionResult l in el)
            {
                //Console.WriteLine(l);

               string decryptionResult = Decrpt(l);

                de.Add(decryptionResult);

               //// Console.WriteLine($"encrypted result {encryptionResult.Result}");

                ////Console.WriteLine($"Decryption starts");

                ////Decrpt(encryptionResult);
            }

            var result = String.Join("", de.ToArray());

            Console.WriteLine(result);

        }

            private static string Decrpt(EncryptionResult encryptionResult)
        {
            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            var resultString = encryptionResult.Result;

            int dimension = (int)Math.Ceiling(Math.Sqrt(resultString.Length));

            string[,] sourceMatrix = GenerateSquareMatrix(resultString, dimension);

            Console.WriteLine("decrpt source matrix");

            PrintMatrix(dimension, sourceMatrix);

            int[,] asciiMatrix = ConvertToAscii(dimension, sourceMatrix);

            Console.WriteLine("ascii matrix");

            PrintMatrix(dimension, asciiMatrix);

            var grayCodeInverse = ConvertGrayCodeToDecimal(dimension, asciiMatrix);

            Console.WriteLine("graycode inverse");

            PrintMatrix(dimension, grayCodeInverse);


            string[,] rotateMatrix = RotateColumn(dimension, encryptionResult.OddColumnRotation, encryptionResult.EvenColumnRotation, grayCodeInverse, negate: true);

            Console.WriteLine("rotate matrix");

            PrintMatrix(dimension, rotateMatrix);

            string[,] interchangeMatrix = InterchangeMatrix(rotateMatrix, dimension);

            Console.WriteLine("interchange matrix");

            PrintMatrix(dimension, interchangeMatrix);

            string[,] characterMatrix = ConvertToCharacterMatrix(dimension, interchangeMatrix);

            Console.WriteLine("character matrix");

            PrintMatrix(dimension, characterMatrix);

            Console.WriteLine($"key {encryptionResult.Key}");

            string[,] columnarTransposition = InverseColumnarTransposition(characterMatrix, dimension, encryptionResult.Key);

            Console.WriteLine("columunar transposition matrix");

            PrintMatrix(dimension, columnarTransposition);

            var transposeMatrix = Transpose(columnarTransposition, dimension);

            Console.WriteLine("transpose matrix");

            PrintMatrix(dimension, transposeMatrix);

            string result = Regex.Replace(PrintMatrixToString(dimension, transposeMatrix), @"Þ+", " ");

            Console.WriteLine($"Decrpt Message :{result}");

            watch.Stop();


            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds / 1000} s");

                return result;

            //Console.ReadLine();

            }

        private static EncryptionResult Encrypt(string source)
        {
            var watch = new System.Diagnostics.Stopwatch();

            //Start
            watch.Start();

            EncryptionResult result = new EncryptionResult();

            //TODO: To update space with x character - should not have alphanumeric in decryption
            string textWithoutSpaces = Regex.Replace(source, @"\s+", "Þ");//change

            Console.WriteLine(textWithoutSpaces.Length);

            int dimension = (int)Math.Ceiling(Math.Sqrt(textWithoutSpaces.Length));

            string[,] sourceMatrix = GenerateSquareMatrix(textWithoutSpaces, dimension);

            Console.WriteLine("source matrix");

            PrintMatrix(dimension, sourceMatrix);

            var transposeMatrix = Transpose(sourceMatrix, dimension);

            Console.WriteLine("transpose matrix");

            PrintMatrix(dimension, transposeMatrix);

            var rnd = new Random();
            result.Key = string.Join("", Enumerable.Range(0, dimension).OrderBy(x => rnd.Next()).ToList());

            Console.WriteLine("random number " + result.Key);

            string[,] columnarTransposition = ColumnarTransposition(transposeMatrix, dimension, result.Key);

            Console.WriteLine("columnar transposition");

            PrintMatrix(dimension, columnarTransposition);

            string[,] interchangeMatrix = InterchangeMatrix(columnarTransposition, dimension);

            Console.WriteLine("interchange matrix");

            PrintMatrix(dimension, interchangeMatrix);

            int[,] asciiMatrix = ConvertToAscii(dimension, interchangeMatrix);

            Console.WriteLine("ascii matrix");

            PrintMatrix(dimension, asciiMatrix);

            string[,] grayCodeMatrix = ConvertToGrayCodeMatrix(dimension, asciiMatrix);

            Console.WriteLine("graycode matrix");

            PrintMatrix(dimension, grayCodeMatrix);

            result.OddColumnRotation = rnd.Next(1, dimension);
            result.EvenColumnRotation = rnd.Next(1, dimension);

            Console.WriteLine($"rotation odd : {result.OddColumnRotation} even : {result.EvenColumnRotation }");

            string[,] rotateMatrix = RotateColumn(dimension, result.OddColumnRotation, result.EvenColumnRotation, grayCodeMatrix);

            Console.WriteLine("rotate matrix");

            PrintMatrix(dimension, rotateMatrix);
            result.Result = ConvertToCharacter(dimension, rotateMatrix);

            watch.Stop();


            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds / 1000} s");

            return result;
        }

        private static string[,] RotateColumn(int dimension, int oddColumnRotate, int evenColumnRotate, string[,] grayCodeMatrix, bool negate = false)
        {
            var rotateMatrix = grayCodeMatrix;

            Console.WriteLine($"dimension {dimension} odd rotate {evenColumnRotate} even rotate {evenColumnRotate} \n");

            for (int col = 0; col < dimension; col++)
            {
                int noOfRotations = (col % 2) == 0 ? evenColumnRotate : oddColumnRotate;

                if (negate)
                {
                    SwapElements(rotateMatrix, col, 0, noOfRotations - 1);
                    SwapElements(rotateMatrix, col, noOfRotations, (dimension - 1));
                    SwapElements(rotateMatrix, col, 0, (dimension - 1));
                }
                else
                {
                    for (int i = 0; i < noOfRotations; i++)
                    {


                        string temp;

                        //left rotate
                        for (int row = 0; row < dimension - 1; row++)
                        {
                            temp = rotateMatrix[0, col];
                            rotateMatrix[0, col] = rotateMatrix[row + 1, col];
                            rotateMatrix[row + 1, col] = temp;
                        }


                    }

                }
            }

            Console.WriteLine("Rotate Matrix signature \n");
            //PrintMatrix(dimension, rotateMatrixSignature);

            return rotateMatrix;
        }

        static void SwapElements(string[,] arr, int colIndex, int startIndex, int endIndex)
        {
            while (startIndex < endIndex)
            {
                string temp = arr[startIndex, colIndex];
                arr[startIndex, colIndex] = arr[endIndex, colIndex];
                arr[endIndex, colIndex] = temp;
                startIndex++;
                endIndex--;
            }
        }

        private static string[,] GenerateSquareMatrix(string textWithoutSpaces, int dimension)
        {
            string[,] sourceMatrix = new string[dimension, dimension];

            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    int index = (row * dimension) + col;

                    if (index < textWithoutSpaces.Length)
                    {
                        sourceMatrix[row, col] = textWithoutSpaces.Substring(index, 1);
                    }
                }
            }

            return sourceMatrix;
        }

        private static string ConvertToCharacter(int dimension, string[,] matrix)
        {
            StringBuilder result = new StringBuilder();
            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    if (matrix[row, col] != null)
                        result.Append((char)(int.Parse(matrix[row, col].ToString())));
                }
            }
            return result.ToString();
        }


        private static string PrintMatrixToString(int dimension, string[,] matrix)
        {
            StringBuilder result = new StringBuilder();
            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    if (matrix[row, col] != null)
                        result.Append(matrix[row, col].ToString());
                }
            }
            return result.ToString();
        }

        private static string[,] ConvertToCharacterMatrix(int dimension, string[,] matrix)
        {
            string[,] charMatrix = new string[dimension, dimension];
            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    charMatrix[row, col] = ((char)(int.Parse(matrix[row, col]))).ToString();
                }
            }
            return charMatrix;
        }

        private static string[,] ConvertToGrayCodeMatrix(int dimension, int[,] asciiMatrix)
        {
            string[,] binaryMatrix = new string[dimension, dimension];

            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    binaryMatrix[row, col] = (asciiMatrix[row, col] ^ (asciiMatrix[row, col] >> 1)).ToString();
                }
            }

            return binaryMatrix;
        }


        private static string[,] ConvertGrayCodeToDecimal(int dimension, int[,] asciiMatrix)
        {
            string[,] decimalMatrix = new string[dimension, dimension];

            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    int inv = 0;
                    int n = asciiMatrix[row, col];
                    for (; n != 0; n >>= 1)
                        inv ^= n;
                    decimalMatrix[row, col] = inv.ToString();
                }
            }

            return decimalMatrix;
        }

        private static int[,] ConvertToAscii(int dimension, string[,] matrix)
        {
            var asciiMatrix = new int[dimension, dimension];

            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    if (matrix[row, col] != null)
                        asciiMatrix[row, col] = matrix[row, col].ToCharArray().FirstOrDefault();
                }
            }

            return asciiMatrix;
        }

        private static string[,] InterchangeMatrix(string[,] columnarTransposition, int dimension)
        {
            var interchangeMatrix = new string[dimension, dimension];

            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    interchangeMatrix[row, col] = columnarTransposition[dimension - 1 - row, dimension - 1 - col];
                }
            }

            return interchangeMatrix;
        }

        //TODO: Update ColumnarTransposition functionlaity to fit 16x16
        private static string[,] ColumnarTransposition(string[,] transposeMatrix, int dimension, string key)
        {
            var columnarTransposition = new string[dimension, dimension];

            var columnarSignatureTransposition = new string[dimension, dimension];

            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    columnarTransposition[row, col] = transposeMatrix[row, key.IndexOf(col.ToString())];//Index was outside the bounds of the array
                    columnarSignatureTransposition[row, col] = $"{row}{key.IndexOf(col.ToString())}";
                }
            }
            Console.WriteLine("columnar signature");
            PrintMatrix(dimension, columnarSignatureTransposition);
            return columnarTransposition;
        }


        private static string[,] InverseColumnarTransposition(string[,] transposeMatrix, int dimension, string key)
        {
            var columnarTransposition = new string[dimension, dimension];

            var columnarSignatureTransposition = new string[dimension, dimension];

            for (int row = 0; row < dimension; row++)
            {
                for (int col = 0; col < dimension; col++)
                {
                    columnarTransposition[row, col] = transposeMatrix[row, int.Parse(key.Substring(col, 1))];
                    columnarSignatureTransposition[row, col] = $"{row}{key.Substring(col, 1)}";
                }
            }
            Console.WriteLine("columnar signature");
            PrintMatrix(dimension, columnarSignatureTransposition);
            return columnarTransposition;
        }

        private static void PrintMatrix(int squareMatrixLength, string[,] sourceMatrix)
        {
            for (int row = 0; row < squareMatrixLength; row++)
            {
                for (int col = 0; col < squareMatrixLength; col++)
                {
                    Console.Write(sourceMatrix[row, col] + " ");
                }
                Console.WriteLine();
            }
        }

        private static void PrintMatrix(int squareMatrixLength, int[,] sourceMatrix)
        {
            for (int row = 0; row < squareMatrixLength; row++)
            {
                for (int col = 0; col < squareMatrixLength; col++)
                {
                    Console.Write(sourceMatrix[row, col] + " ");
                }
                Console.WriteLine();
            }
        }

        private static string[,] Transpose(string[,] matrix, int dimension)
        {
            for (int i = 0; i < dimension; i++)
                for (int j = i + 1; j < dimension; j++)
                {
                    string temp = matrix[i, j];
                    matrix[i, j] = matrix[j, i];
                    matrix[j, i] = temp;
                }
            return matrix;
        }

        public static IEnumerable<string> SplitByLength(string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }
    }


    public class EncryptionResult
    {
        public string Result { get; set; }
        public string Key { get; set; }
        public int OddColumnRotation { get; set; }
        public int EvenColumnRotation { get; set; }
    }
}
