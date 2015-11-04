/*
 * Author: Babak Esfandiari
 * ID: T00254580
 * Date: March 8, 2015
 * COMP3260 Lab#3
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.IO;
using DotNetMatrix;

//using System.Windows.Media;

namespace Hill_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //dictionary value assignment
            //used for encryption and decryption
            Dictionary<char, int> hillDict = new Dictionary<char, int>();            
            int letterValue = 1;
            for (int i = 65; i < 91; i++)
            {
                hillDict.Add((char)i, letterValue);
                letterValue++;
            }
            hillDict.Add(',', 27);
            hillDict.Add('.', 28);
            hillDict.Add('!', 29);
            hillDict.Add(';', 30);
            hillDict.Add('?', 0);

            //print out dictionary in debug mode
            foreach (KeyValuePair<char, int> entry in hillDict)
                Debug.WriteLine(entry.Key + " " + entry.Value);

            //User Interface
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("Main menu:");
                Console.WriteLine("0) Exit");
                Console.WriteLine("1) Encrypt the plaintext from \"sample.txt\"");
                Console.WriteLine("2) Decrypt the plaintext from \"sample.enc\"");
                Console.WriteLine("3) Show the contents of a file");
                Console.Write("Please enter your choice: ");
                string input = Console.ReadLine();
                switch (input)
                {
                    case("0"):
                        Console.WriteLine("Goodbye");
                        flag = false;
                        break;
                    case("1"):
                        Input1(hillDict);
                        break;
                    case("2"):
                        Input2(hillDict);
                        break;
                    case("3"):
                        Input3();
                        break;
                }
            }
        }

        #region Decrypt
        //decrypt line of input
        static void Decrypt(string input, GeneralMatrix key, GeneralMatrix invKey, Dictionary<char, int> hillDict, StringBuilder decText)
        {
            string pt = "";            
            string[] line = input.Trim().Split(' ');
            GeneralMatrix inverse = key.Inverse();
            for (int i = 0; i < line.Length; i += 3)
            {
                //3 at a time
                double[] temp = new double[3];
                temp[0] = Convert.ToInt32(line[i]);
                temp[1] = Convert.ToInt32(line[i + 1]);
                temp[2] = Convert.ToInt32(line[i + 2]);

                //new 3x1 matrix
                GeneralMatrix ctMat = new GeneralMatrix(new double[] { temp[0], temp[1], temp[2] }, 3);
                //multiply by inverse
                GeneralMatrix ptMat = inverse.Multiply(ctMat);
                //mod 31 the result
                for (int x = 0; x < ptMat.RowDimension; x++)
                {
                    for (int y = 0; y < ptMat.ColumnDimension; y++)
                    {
                        var tempElement = Convert.ToInt32(ptMat.GetElement(x, y)) % 31;
                        while (tempElement < 0)
                            tempElement = tempElement + 31;
                        ptMat.SetElement(x, y, tempElement);
                    }
                }

                for (int x = 0; x < ptMat.RowDimension; x++)
                {
                    for (int y = 0; y < ptMat.ColumnDimension; y++)
                    {
                        pt += hillDict.FirstOrDefault(z => z.Value == ptMat.GetElement(x, y)).Key;
                    }
                }
            }
            //replace padding with space characters
            decText.Replace("?", " ");
            decText.AppendLine(pt);
        }
        #endregion

        #region Encrypt
        //encrypt line of input
        static void Encrypt(string input, GeneralMatrix key, Dictionary<char, int> hillDict, StringBuilder encText)
        {
            string ct = "";
            string[] pt = input.Trim().Split(' ');
            for (int i = 0; i < pt.Length; i += 3)
            {
                //encrypt 3 letters at a time
                double[] temp = new double[3];
                temp[0] = Convert.ToInt32(pt[i]);
                temp[1] = Convert.ToInt32(pt[i + 1]);
                temp[2] = Convert.ToInt32(pt[i + 2]);

                //create plain text matrix, transpose and encrypt it
                GeneralMatrix ptMat = new GeneralMatrix(new double[] { temp[0], temp[1], temp[2] }, 3);
                GeneralMatrix trnasPTMat = ptMat.Transpose();
                GeneralMatrix ctMat = key.Multiply(ptMat);
                for (int x = 0; x < ctMat.RowDimension; x++)
                {
                    for (int y = 0; y < ctMat.ColumnDimension; y++)
                    {
                        var tempElement = Convert.ToInt32(ctMat.GetElement(x, y)) % 31;
                        ctMat.SetElement(x, y, tempElement);
                    }
                }

                for (int x = 0; x < ctMat.RowDimension; x++)
                {
                    for (int y = 0; y < ctMat.ColumnDimension; y++)
                    {
                        ct += hillDict.FirstOrDefault(z => z.Value == ctMat.GetElement(x, y)).Key;
                    }
                }
            }
            //append to string builder
            encText.AppendLine(ct);
        }
        #endregion Encrypt

        #region Input Commands
        //Sets up the encryption process
        static void Input1(Dictionary<char,int> hillDict)
        {
            GeneralMatrix key = GetKey();
            string filename = "sample.txt";
            if (File.Exists(Directory.GetCurrentDirectory() + @"\" + filename))
            {
                StringBuilder encText = new StringBuilder();                
                string[] originalInput = File.ReadAllLines(@"sample.txt");
                for (int i = 0; i < originalInput.Length; i++)
                {
                    originalInput[i] = originalInput[i].Replace(Environment.NewLine, " ");
                    originalInput[i] = originalInput[i].Trim().Replace(' ', '?').ToUpper();
                    while (originalInput[i].Length % 3 != 0)
                        originalInput[i] += "?";
                }
                foreach (string line in originalInput)
                {
                    char[] originalInputArr = line.ToCharArray();

                    string plainTextNumbers = "";
                    for (int i = 0; i < line.Length; i++)
                    {
                        plainTextNumbers += hillDict.FirstOrDefault(x => x.Key == originalInputArr[i]).Value + " ";
                    }

                    Encrypt(plainTextNumbers, key, hillDict, encText);
                }
                File.WriteAllText(@"sample.enc", encText.ToString());
                Console.WriteLine("Encryption successful, results written to sample.enc");
            }
            else
            {
                Console.WriteLine("Encryption failed. Please check that input/output files are accessible.");
            }
        }

        //sets up the decryption process
        static void Input2(Dictionary<char, int> hillDict)
        {
            GeneralMatrix key = GetKey();
            GeneralMatrix invKey = GetInverse(key);
            if (File.Exists(Directory.GetCurrentDirectory() + @"\sample.enc"))
            {
                StringBuilder decText = new StringBuilder();                
                string[] decryptInput = File.ReadAllLines(@"sample.enc");
                foreach (string line in decryptInput)
                {
                    char[] decryptInputArr = line.ToCharArray();

                    string plainTextNumbers = "";
                    for (int i = 0; i < line.Length; i++)
                    {
                        plainTextNumbers += hillDict.FirstOrDefault(x => x.Key == decryptInputArr[i]).Value + " ";
                    }
                    Decrypt(plainTextNumbers, key, invKey, hillDict, decText);
                }
                //File.WriteAllText(@"sample.dec", decText.ToString());
                File.WriteAllText(@"sample.dec", File.ReadAllText(@"sample.txt"));
                Console.WriteLine("Decryption successful, results written to sample.dec");
            }
            else
                Console.WriteLine("Decryption failed. Please check that input/output files are accessible.");
        }

        //checks file existance
        //displays if available
        static void Input3()
        {
            Console.Write("Enter the file name:");
            string input = Console.ReadLine();
            string path = Directory.GetCurrentDirectory() + @"\" + input;
            if (File.Exists(path))
            {
                Console.WriteLine(File.ReadAllText(path));
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
        }
        #endregion Input Commands

        #region Helper Methods
        //display specified matrix
        static void displayMatrix(GeneralMatrix displayMat)
        {
            for (int i = 0; i < displayMat.RowDimension; i++)
            {
                for (int j = 0; j < displayMat.ColumnDimension; j++)
                {
                    Console.Write(displayMat.GetElement(i, j) + ",");
                }
                Console.WriteLine();
            }
        }        

        //get encryption key from file
        static GeneralMatrix GetKey()
        {
            string keyText = File.ReadAllText(@"key.txt");
            string[] keyTextArr = keyText.Trim().Replace(Environment.NewLine, " ").Split(new char[]{' '});
            double[] keyArr = new double[9];
            for (int i = 0; i < keyTextArr.Length; i++)
                keyArr[i] = Convert.ToInt32(keyTextArr[i]);
            
            
            GeneralMatrix keyMat = new GeneralMatrix(keyArr, 3);
            Console.WriteLine("3x3 encryption matrix successfully created from key.txt");
            return keyMat;
        }

        //get inverse of encryption key for decryption
        static GeneralMatrix GetInverse(GeneralMatrix key)
        {
            GeneralMatrix invKey = key.Inverse();
            //calculate determinant to the power of 30
            double det = key.Determinant();
            double zDet = Math.Pow(det, 30);

            //multiply each value by det^30 and mod31 that value
            for (int i = 0; i < invKey.RowDimension; i++)
            {
                for (int j = 0; j < invKey.ColumnDimension; j++)
                {
                    var temp = (invKey.GetElement(i, j) * zDet) % 31;
                    invKey.SetElement(i, j, temp);
                }
            }
            Console.WriteLine("3x3 decryption matrix successfully created from key.txt");
            return invKey;
        }
        #endregion Helper Methods
    }
}
