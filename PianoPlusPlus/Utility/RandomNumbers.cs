using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PianoPlusPlus.Utility
{
    public class RandomNumbers
    {
        internal static string GetRandomNumbers(int length = 6)
        {
            var values = new byte[length];
            var rnd = new Random();
            rnd.NextBytes(values);
            return values.Aggregate(string.Empty, (current, v) => current + v.ToString());
        }
    }
}