/*
 * Author: Babak Esfandiari
 * Date: February 16, 2015
 * Student ID: T00254580
 * Description: This code can be used to find the key to a Vigenere cipher and use it to decrypt encrypted text.
 *              It reads the encrypted text from input.txt and presents the key/decrypted text in output.txt (both in the same directory as the .exe).
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VigenereBreaker
{
    class Program
    {
        static void Main(string[] args)
        {
            //declare key length
            int keyLength = -1;
            string key = "";
            string input;
            
            bool correctInput = false;
            while (!correctInput)
            {
                try
                {
                    //get key length from user
                    Console.WriteLine("Please enter length of key.");
                    input = Console.ReadLine();

                    //convert and test to see if value is in range
                    if (Convert.ToInt32(input) < 1 || Convert.ToInt32(input) > 5)
                    { throw new Exception(); }
                    else
                    {
                        correctInput = true;
                        keyLength = Convert.ToInt32(input);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid entry. Value must be between 1 and 5 (inclusive).");
                }
            }

            //load cipher text into string builder from file
            Console.WriteLine("Reading encrypted data from input.txt...");
            StringBuilder cipherText = new StringBuilder();
            DirectoryInfo thisDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
            cipherText.Append(File.ReadAllText(thisDirectory + @"\input.txt"));
            Console.WriteLine("Read completed.");

            Console.WriteLine("Starting frequency analysis...");
            //iterate through each position to find the key letter
            for (int i = 0; i < keyLength; i++)
            {
                //run frequency analysis to discover key
                key += FrequencyAnalysis(i, keyLength, cipherText);
            }
            Console.WriteLine("Frequency analysis completed.");

            //display key
            Console.WriteLine("Key: " + key);

            //debug message
            //Console.WriteLine(Decrypt(cipherText.ToString(), key));
            //output key and decrypted text to file and display message
            File.WriteAllLines((thisDirectory + @"\output.txt"), new string[] { key, Decrypt(cipherText.ToString(), key) });
            Console.WriteLine(@"Text successfully decrypted in .\output.txt.");
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

        }

        public static string FrequencyAnalysis(int keyPosition, int keyLength, StringBuilder cipherText)
        {
            //declare and populate letter frequency array
            int[] alphabet = new int[26];
            for (int i = 0; i < 26; i++)
            {
                alphabet[i] = 0;
            }
            
            //another string builder used to display analysis data
            StringBuilder analysis = new StringBuilder();
            for (int i = keyPosition; i < cipherText.Length; i += keyLength)
            {
                analysis.Append(cipherText[i]);
                if (cipherText[i] < 65 || cipherText[i] > 90)
                {
                    throw new Exception("Invalid character detected in input file.");
                }
                else
                {
                    //increment the count for that letter in the alphabet array
                    alphabet[cipherText[i] - 65] += 1;
                }
            }
            
            int highest = -1;
            int highestLetter = -1;
            for (int i = 0; i < alphabet.Length; i++)
            {
                //debug message
                //Console.Write(alphabet[i] + " ");
                if (alphabet[i] > highest)
                {
                    highest = alphabet[i];
                    highestLetter = i;
                }
                    
            }
            //debug message
            //Console.WriteLine("Highest value is at index " + highestLetter);         

            //letter minus index for E
            int shift = highestLetter - 4;

            //loop around if necessary
            int keyValue;
            if (shift < 0)
                keyValue = 65 + shift + 26;
            else
                keyValue = 65 + shift;

            return ((char)keyValue).ToString();
 
        }

        //decrypt the cipher text using the key found using FrequencyAnalysis()
        //used the following source as a resource for this method: http://www.dreamincode.net/forums/topic/190726-secret-code-ii-vigenere-square/
        public static string Decrypt(string txt, string key)
        {
            StringBuilder plainText = new StringBuilder();
            char[] cipherText = txt.ToCharArray();
            char[] keyArr = key.ToCharArray();

            string helperKey = key;
            while (helperKey.Length < txt.Length)
                helperKey += key;
            char[] helperKeyArray = helperKey.ToCharArray();

            for (int i = 0; i < cipherText.Length; i++)
            {
                if (cipherText[i] < 'A' || cipherText[i] > 'Z')
                    continue;
                int shift = helperKeyArray[i] - 'A';
                cipherText[i] = Convert.ToChar(Convert.ToInt32(cipherText[i]) - shift);
                if (cipherText[i] < 'A')
                {
                   cipherText[i] = Convert.ToChar( Convert.ToInt32( cipherText[i] ) + 26);
                }

            }
            return new String( cipherText );
        }
    }
}
