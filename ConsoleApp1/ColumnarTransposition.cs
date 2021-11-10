using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
     public class ColumnarTransposition
     {
          string originalMessage;
          string key;

          string[,] messageMatrix;
          string[,] columnarMatrix;
          string[,] interchangeMatrix;
          int dimension;

          Random random;
          List<int> randomKeyIndex;
         public EncryptionResult encryptionResult;
          public ColumnarTransposition()
          {
               Init();
          }

          public ColumnarTransposition(string message, string key = null)
          {
               Init();
               encryptionResult= InilitizeMessage(message, key);
          }

          /// <summary>
          /// init sequence 
          /// </summary>
          private void Init()
          {
               random = new Random(DateTime.Now.Millisecond);
          }

          /// <summary>
          /// print message matrix
          /// </summary>
          public void PrintMessageMatrix()
          {
               Console.WriteLine("Message Matrix-----------------");
               PrintMatrix(messageMatrix, dimension);
          }

          /// <summary>
          /// print columnr matrix
          /// </summary>
          public void PrintColumnarMatrix()
          {
               Console.WriteLine("Columnar Matrix-----------------");
               PrintMatrix(columnarMatrix, dimension);
          }

          /// <summary>
          /// print Transpose matrix
          /// </summary>
          public void PrintTransposeMatrixMatrix()
          {
               Console.WriteLine("Transpose Matrix-----------------");
               PrintMatrix(interchangeMatrix, dimension);
          }


          /// <summary>
          /// initialize the matrix with the message and key
          /// </summary>
          /// <param name="message"></param>
          /// <param name="key"></param>
          private EncryptionResult InilitizeMessage(String message, string key)
          {
               EncryptionResult result = new EncryptionResult();
               this.originalMessage = message;
               this.key = key;
               int length = message.Length;
               dimension = (int)Math.Ceiling(Math.Sqrt(length));
               messageMatrix = GenerateSquareMatrix(message, dimension);
               key = GenerateKeyIfNotFound(key);
               randomKeyIndex = GenerateRandomKey(dimension, key);
               columnarMatrix = BuildColumnarMatrix(messageMatrix, dimension, randomKeyIndex);
               interchangeMatrix = BuildTransposeMatrix(columnarMatrix, dimension);



               //your conversion
               int[,] asciiMatrix = ConvertToAscii(dimension, interchangeMatrix);
               result.grayCodeMatrix = ConvertToGrayCodeMatrix(dimension, asciiMatrix);
               result.OddColumnRotation = random.Next(1, dimension);
               result.EvenColumnRotation = random.Next(1, dimension);
               result.rotateMatrix = RotateColumn(dimension, result.OddColumnRotation, result.EvenColumnRotation, result.grayCodeMatrix);
               result.Result = ConvertToCharacter(dimension, result.rotateMatrix); 
               return result;

          }

          /// <summary>
          /// get message back from the matrix
          /// </summary>
          /// <returns></returns>
          public String GetMessage(EncryptionResult encryptionResult)
          {
               var resultString = encryptionResult.Result;
               int dimension = (int)Math.Ceiling(Math.Sqrt(resultString.Length));
               string[,] sourceMatrix = GenerateSquareMatrix(resultString, dimension);
               int[,] asciiMatrix = ConvertToAscii(dimension, sourceMatrix);
               
               string[,] grayCodeInverse = ConvertGrayCodeToDecimal(dimension, asciiMatrix);
               string[,] rotateMatrix = RotateColumn(dimension, encryptionResult.OddColumnRotation, encryptionResult.EvenColumnRotation, grayCodeInverse, negate: true);
               string[,] interchangeMatrix = InterchangeMatrix(rotateMatrix, dimension);
               string[,] characterMatrix = ConvertToCharacterMatrix(dimension, interchangeMatrix);
               var transposeMatrix = Transpose(characterMatrix, dimension);

              
               Console.WriteLine("from class Matrix-----------------");
               PrintMatrix(this.interchangeMatrix, dimension);
               Console.WriteLine("from local Matrix-----------------");
               PrintMatrix(transposeMatrix, dimension);


                
               return GetMessage(characterMatrix, dimension);
          }


          //for upto transpose
          public String GetMessage()
          { 
               return GetMessage(interchangeMatrix, dimension);
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

          private static string[,] RotateColumn(int dimension, int oddColumnRotate, int evenColumnRotate, string[,] grayCodeMatrix, bool negate = false)
          {
               var rotateMatrix = grayCodeMatrix;

               //Console.WriteLine($"dimension {dimension} odd rotate {evenColumnRotate} even rotate {evenColumnRotate} \n");

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
                              for (int row = 0; row < dimension - 1; row++)
                              {
                                   temp = rotateMatrix[0, col];
                                   rotateMatrix[0, col] = rotateMatrix[row + 1, col];
                                   rotateMatrix[row + 1, col] = temp;
                              }


                         }

                    }
               }  
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

          /// <summary>
          /// get the message
          /// </summary>
          /// <param name="transposeMatrix"></param>
          /// <param name="dimension"></param>
          /// <returns></returns>
          private string GetMessage(string[,] transposeMatrix, int dimension)
          {
               string[,] columnarMatrixTmp = BuildTransposeMatrix(transposeMatrix, dimension);
               string[,] messageMatrixTmp = ReverseColumnarMatrix(columnarMatrixTmp, dimension, randomKeyIndex);
               string message = BuildMessageString(messageMatrixTmp, dimension);
               return message;
          }

          /// <summary>
          /// key generate if not supplied
          /// </summary>
          /// <param name="key"></param>
          /// <returns></returns>
          private string GenerateKeyIfNotFound(string key)
          {
               if (string.IsNullOrEmpty(key))
               {
                    for (int i = 0; i < dimension; i++)
                    {
                         int keyIndex = random.Next(0, 25);
                         char alpha = Convert.ToChar(keyIndex + (int)'A');
                         key += alpha;
                    }
               }
               if (key.Length < dimension)
               {
                    throw new Exception($"In adequate key length. The required key length is {dimension}");
               }

               return key;
          }

          /// <summary>
          /// generate random number based on the key
          /// </summary>
          /// <param name="dimension"></param>
          /// <param name="key"></param>
          /// <returns></returns>
          private List<int> GenerateRandomKey(int dimension, string key)
          {

               List<int> randomKeyIndex = new List<int>();
               for (int i = 0; i < 26; i++)
               {
                    char alpha = Convert.ToChar(i + (int)'a');
                    char alphaBig = Convert.ToChar(i + (int)'A');
                    for (int j = 0; j < dimension; j++)
                    {
                         if (alpha == key[j] || alphaBig == key[j])
                         {
                              randomKeyIndex.Add(j);
                         }
                    }
               }
               return randomKeyIndex;
          }

          /// <summary>
          /// building string from message
          /// </summary>
          /// <param name="messageMatrix"></param>
          /// <param name="dimension"></param>
          /// <returns></returns>
          private string BuildMessageString( string[,] messageMatrix, int dimension)
          {
               String message="";
               string[,] columnarTransposition = new string[dimension, dimension]; 
               for (int row = 0; row < dimension; row++)
               {
                    for (int col = 0; col < dimension; col++)
                    {
                         message += messageMatrix[row, col];
                    }
               }
               return message;
          }


          /// <summary>
          /// build message matrix
          /// </summary>
          /// <param name="message"></param>
          /// <param name="messageMatrix"></param>
          /// <param name="dimension"></param>
          private string[,] GenerateSquareMatrix(String message,  int dimension)
          {
               string[,] messageMatrix = new string[dimension, dimension];
               int length = message.Length;
               for (int row = 0; row < dimension; row++)
               {
                    for (int col = 0; col < dimension; col++)
                    {
                         int index = row * dimension + col;
                         if (index < length)
                         {
                              messageMatrix[row, col] = message[index].ToString();
                         }
                    }
               }
               return messageMatrix;
          }



          /// <summary>
          /// build columnr matrix
          /// </summary>
          /// <param name="messageMatrix"></param>
          /// <param name="dimension"></param>
          /// <param name="randomKeyIndex"></param>
          /// <returns></returns>
          private string[,] BuildColumnarMatrix(string[,] messageMatrix, int dimension, List<int> randomKeyIndex)
          {
               string[,] columnarTransposition = new string[dimension, dimension];
               for (int row = 0; row < dimension; row++)
               {
                    int colIndex = randomKeyIndex[row];
                    for (int col = 0; col < dimension; col++)
                    {
                         columnarTransposition[row, col] = messageMatrix[col, colIndex];
                    }
               }
               return columnarTransposition;
          }

          /// <summary>
          /// reversing columner matrix
          /// </summary>
          /// <param name="messageMatrix"></param>
          /// <param name="dimension"></param>
          /// <param name="randomKeyIndex"></param>
          /// <returns></returns>
          private string[,] ReverseColumnarMatrix(string[,] messageMatrix, int dimension, List<int> randomKeyIndex)
          {
               string[,] reverseColumnarMatrix = new string[dimension, dimension];
               for (int row = 0; row < dimension; row++)
               {
                    int colIndex = randomKeyIndex[row];
                    for (int col = 0; col < dimension; col++)
                    {
                         reverseColumnarMatrix[col, colIndex] = messageMatrix[row, col];
                    }
               }
               return reverseColumnarMatrix;
          }

          /// <summary>
          /// build transpose matrix
          /// </summary>
          /// <param name="matrix"></param>
          /// <param name="dimension"></param>
          /// <returns></returns>
          private string[,] BuildTransposeMatrix(string[,] matrix, int dimension)
          {
               string[,] destinationMatrix = new string[dimension, dimension];
               for (int row = 0; row < dimension; row++)
               {
                    for (int col = 0; col < dimension; col++)
                    {
                         destinationMatrix[row, col] = matrix[col, row];
                    }
               }

               for (int row = 0; row < dimension; row++)
               {
                    for (int col = 0; col < dimension; col++)
                    {
                         if (row == col)
                         {
                              destinationMatrix[row, col] = matrix[dimension - row - 1, dimension - col - 1];
                         }
                    }
               }
               return destinationMatrix;
          }

          /// <summary>
          /// printing matrix
          /// </summary>
          /// <param name="matrix"></param>
          /// <param name="dimension"></param>
          private void PrintMatrix(String[,] matrix, int dimension)
          {
               for (int row = 0; row < dimension; row++)
               {
                    for (int col = 0; col < dimension; col++)
                    {
                         Console.Write(matrix[row, col] + " ");
                    }
                    Console.WriteLine();
               }
          }


     }
}
