namespace ConsoleApp1
{
     public class EncryptionResult
     {
          public string Result { get; set; }
          public string Key { get; set; }
          public int OddColumnRotation { get; set; }
          public int EvenColumnRotation { get; set; }


          public string[,] grayCodeMatrix { get; set; }
          public string[,] rotateMatrix { get; set; }
     }
}
