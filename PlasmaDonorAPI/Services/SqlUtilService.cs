using NewPlasmaDonorsAPI.utils;

namespace NewPlasmaDonorsAPI.Services
{
    public class SqlUtilService
    {
        public string AgeGroupQuery(int ageGroup)
        {
            DateTime cal1 = DateTime.Now;
            DateTime cal2 = DateTime.Now;
            string dobQuery = "";

            if (ageGroup == 20)
            {
                cal1 = cal1.AddYears(-20);
                dobQuery += $" (a.dob >= '{ToSqlDate(cal1)}')";
            }
            else if (ageGroup == 30)
            {
                cal1 = cal1.AddYears(-30);
                cal2 = cal2.AddYears(-20);
                dobQuery += $" (a.dob >= '{ToSqlDate(cal1)}' AND a.dob <= '{ToSqlDate(cal2)}') ";
            }
            else if (ageGroup == 40)
            {
                cal1 = cal1.AddYears(-40);
                cal2 = cal2.AddYears(-30);
                dobQuery += $" (a.dob >= '{ToSqlDate(cal1)}' AND a.dob <= '{ToSqlDate(cal2)}') ";
            }
            else if (ageGroup == 60)
            {
                cal1 = cal1.AddYears(-60);
                cal2 = cal2.AddYears(-40);
                dobQuery += $" (a.dob >= '{ToSqlDate(cal1)}' AND a.dob <= '{ToSqlDate(cal2)}') ";
            }
            else if (ageGroup == 80)
            {
                cal1 = cal1.AddYears(-80);
                cal2 = cal2.AddYears(-60);
                dobQuery += $" (a.dob >= '{ToSqlDate(cal1)}' AND a.dob <= '{ToSqlDate(cal2)}') ";
            }
            else if (ageGroup == 81)
            {
                cal1 = cal1.AddYears(-80);
                dobQuery += $" (a.dob <= '{ToSqlDate(cal1)}') ";
            }

            Console.WriteLine($"::{dobQuery}");
            return dobQuery;
        }

        private string ToSqlDate(DateTime date)
        {
            return DateUtils.ToStringForSql(date);
        }
    }
}
