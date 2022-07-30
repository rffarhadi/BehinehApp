using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.Utility
{
    public static class PersionToJalali
    {
        public static DateTime ConvertToGregorian(string PersianDate)
        {
            try
            {
                var pd = PersianDate.ToPersianNumber();
                var year = pd.Substring(0, 4);
                var month = pd.Substring(5, 2);
                var day = pd.Substring(8, 2);
                var hour = pd.Substring(11, 2);
                var minute = pd.Substring(14, 2);
                var second = pd.Substring(17, 2);
                PersianCalendar pc = new PersianCalendar();
                DateTime dt = new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), int.Parse(hour), int.Parse(minute), int.Parse(second), pc);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DateTime ConvertToShortGregorian(string PersianDate)
        {
            try
            {
                var pd = PersianDate.ToPersianNumber();
                int yearShamsi = Convert.ToInt32(pd.Substring(0, 4));
                int monthShamsi = Convert.ToInt32(pd.Substring(5, 2));
                int dayShamsi = Convert.ToInt32(pd.Substring(8, 2));
                DateTime miladi = new DateTime(yearShamsi, monthShamsi, dayShamsi, 23, 59, 59, 998, new System.Globalization.PersianCalendar());
                return miladi;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ConvertPersianToWithoutSlash(string persianDate)
        {
            string year = persianDate.Substring(0, 4);
            string month = persianDate.Substring(5, 2);
            string day = persianDate.Substring(8, 2);
            string date = year + month + day;
            return date;
        }

        public static DateTime DynamicPersianToGregorian(string persianDate)
        {
            DateTime tarikhMiladi;
            if (persianDate.Length <= 10)
            {
                tarikhMiladi = PersionToJalali.ConvertToShortGregorian(persianDate);
            }
            else
            {
                tarikhMiladi = PersionToJalali.ConvertToGregorian(persianDate);
            }
            return tarikhMiladi;
        }

        public static string ToPersianNumber(this string input)
        {
            if (input.Trim() == "") return "";

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




    }
}
