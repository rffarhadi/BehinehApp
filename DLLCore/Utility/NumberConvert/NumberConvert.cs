using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.Utility
{
    public static class NumberConvert
    {
        public static string ConvertPersianToNumber(this string input)
        {
            if (input.Trim() == "") return "0";
            input = input.Replace(",", "");
            //۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹
            input = input.Replace("۰", "0");
            input = input.Replace("۱", "1");
            input = input.Replace("۲", "2");
            input = input.Replace("۳", "3");
            input = input.Replace("۴", "4");
            input = input.Replace("۵", "5");
            input = input.Replace("۶", "6");
            input = input.Replace("۷", "7");
            input = input.Replace("۸", "8");
            input = input.Replace("۹", "9");
            return String.Format(input);
        }


        public static string ToPersianNumberWithoutSlash(string input)
        {
            if (input.Trim() == "") return "0";

            //۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹
            input = input.Replace(",", "");
            return String.Format(input);
        }
    }
}
