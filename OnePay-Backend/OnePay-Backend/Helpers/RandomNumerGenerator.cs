using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnePay_Backend.Helpers
{
    public static class RandomNumerGenerator
    {
        //Function to get random number
        private static readonly Random getrandom = new Random();

        public static string GetRandomNumber(int length)
        {
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, getrandom.Next(10).ToString());
            return s;
        }

    }
}
