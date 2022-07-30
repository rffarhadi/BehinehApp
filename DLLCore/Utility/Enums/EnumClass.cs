using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.Utility.Enums
{
    public static class EnumClass
    {
        //public enum UtilityEnum { StarterCounter = 1, ValuationCounter = 2 }

        //برای اینکه کد تفصیل حسابهای ارزشیابی با کد تفصیل خود سهام متمایز شود و فقط عدد 2 به انتهای کد تفصیل اضافه شود
        public static int ValuationCounter { get; } = 2;
        public static int StarterCounter { get; } = 1;

        //کد کلیه حساب‌های سرمایه‌گذاری کوتاه مدت در دفتر کل
        public static int InvestmentsRelatedIdInRawLedger { get; } = 2;

        //کد حساب سهام در دفتر معین
        public static int StockIdInSubLedger { get; } = 2;
        //کد حساب ارزشیابی سهام در دفتر معین
        public static int StockValuationIdInSubLedger { get; } = 3;
        //کد حساب سود سهام در دفتر معین
        public static int StockProfitIdInSubLedger { get; } = 37;
        //کد حساب سود حق تقدم در دفتر معین
        public static int StockPrecedenceProfitIdInSubLedger { get; } = 38;

        //کد حساب حق تقدم سهام در دفتر معین
        public static int StockPrecedenceIdInSubLedger { get; } = 4;
        //کد حساب ارزشیابی حق تقدم سهام در دفتر معین
        public static int StockPrecedenceValuationIdInSubLedger { get; } = 5;

        public static int BrokerIdIdInSubLedger { get; } = 28;


    }
}
