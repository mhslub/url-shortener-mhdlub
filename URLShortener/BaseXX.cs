/*
 * This class is a general form that takes an alphabet and do encoding and decoding
 * Encoding is done by repeatedly dividing the input number by the alphabet size and mapping the modulo to a charachter from the alphabet.
 * Decoding is reverse encoding.
 * 
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace URLShortener
{
    class BaseXX
    {
        private string alphabet_string;

        public string Alphabet_string1
        {
            get { return alphabet_string; }
            set { alphabet_string = value; }
        }      

        private char[] alphabet_array;

        public BaseXX(string alpabet)
        {
            //alphabet for base62 is small letters + capital letters + numbers (26 + 26 + 10) 
            //alphabet_string = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            //alphabet for base 36 is only small letters + numbers (26 + 10)
            //alphabet_string = "0123456789abcdefghijklmnopqrstuvwxyz"
            alphabet_string = new string(alpabet.ToCharArray());
            alphabet_array = alphabet_string.ToCharArray();


        }

        public string Encode(long num)
        {
            bool neg = false; //for saving negative sign
            if (num < 0)
            {
                neg = true;
                num = Math.Abs(num);// take the absolute value for modulo operations
            }

            
            ArrayList buffer = new ArrayList();
            do
            {
                buffer.Add(alphabet_array[num % alphabet_string.Length]);
                num /= alphabet_string.Length;
            } while (num > 0);

            //return the results backwards
            string res = (neg) ? "-" : "";
            for (int i = buffer.Count - 1; i >= 0; i--)
                res += buffer[i].ToString();

            return res;
        }

        public long Decode(string input) 
        {
            //here we reverse the encoding procedure
            bool neg = false;
            if (input[0] == '-')
            {
                neg = true;
                input = input.Substring(1);
            }

            long res = 0;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                int pos = alphabet_string.IndexOf(input[i]);//0 the string is empty. -1 not found or index of the first occurance
                if (pos >= 0)
                {
                    res += pos * (long)Math.Pow(alphabet_string.Length, (input.Length - 1) - i);
                }
                else
                    throw new Exception("Wrong input value!");

            }

            return (neg) ? res * -1 : res;
        }
    }
}
