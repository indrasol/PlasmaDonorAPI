using System.Globalization;

namespace NewPlasmaDonorsAPI.utils
{
    public class DateUtils
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        private static readonly string SqlDateFormat = "yyyy-MM-dd HH:mm:ss";
        private static readonly string UsDateFormat = "MM-dd-yyyy";
        private static readonly string DateFormatSlash = "MM/dd/yyyy";
        private static readonly string BlogDateFormat = "MMMM dd, yyyy";

        public static DateTime? ToShortDate(string dateInString)
        {
            if (DateTime.TryParseExact(dateInString, UsDateFormat, Culture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }

        public static DateTime? ToShortDateUsType(string dateInString)
        {
            if (DateTime.TryParseExact(dateInString, DateFormatSlash, Culture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }

        public static string ToStringUsType(DateTime date)
        {
            return date.ToString(UsDateFormat, Culture);
        }

        public static string ToShortString(DateTime date)
        {
            return date.ToString(UsDateFormat, Culture);
        }

        public static string? ToShortString(DateTime? date)
        {
            return date?.ToString(UsDateFormat, Culture);
        }

        public static string ToShortStringForBlog(DateTime date)
        {
            return date.ToString(BlogDateFormat, Culture);
        }

        public static string ToString(DateTime date)
        {
            return date.ToString(UsDateFormat, Culture);
        }

        public static string ToStringForSql(DateTime date)
        {
            return date.ToString(SqlDateFormat, Culture);
        }

        public static DateTime? ToDate(string dateInString)
        {
            if (string.IsNullOrWhiteSpace(dateInString))
                return null;

            string format = dateInString.Contains(".") ? "yyyy-MM-dd HH:mm:ss.fff" : "yyyy-MM-dd HH:mm:ss";
            if (DateTime.TryParseExact(dateInString, format, Culture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            return null;
        }

        public static DateTime? StringDateFormat(string date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return null;

            try
            {
                if (date.Contains("IST"))
                {
                    return DateTime.ParseExact(date, "ddd MMM dd HH:mm:ss 'IST' yyyy", Culture);
                }
                else if (date.Contains("UTC"))
                {
                    return DateTime.ParseExact(date, "ddd MMM dd HH:mm:ss zzz yyyy", Culture);
                }
                else
                {
                    return ToDate(date);
                }
            }
            catch
            {
                if (DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm:ss", Culture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }
            return null;
        }

        public static DateTime? GetDateBeforeNDays(int noOfDays, string format)
        {
            try
            {
                DateTime pastDate = DateTime.Now.AddDays(-noOfDays);
                return DateTime.ParseExact(pastDate.ToString(format, Culture), format, Culture);
            }
            catch
            {
                return null;
            }
        }
    }
}
    
