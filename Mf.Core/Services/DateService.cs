using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mf.Core.Services
{
    public static class DateService
    {
        public static DateTime GetStartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
        public static string ToShamsi(this DateTime DateTime1)
        {
            try
            {
                PersianCalendar PersianCalendar1 = new PersianCalendar();
                return string.Format(@"{0}/{1}/{2} {3}:{4}:{5}",
                             PersianCalendar1.GetYear(DateTime1),
                             PersianCalendar1.GetMonth(DateTime1).ToString("00"),
                             PersianCalendar1.GetDayOfMonth(DateTime1).ToString("00"),
                             PersianCalendar1.GetHour(DateTime1).ToString("00"),
                             PersianCalendar1.GetMinute(DateTime1).ToString("00"),
                             PersianCalendar1.GetSecond(DateTime1).ToString("00")
                             );
            }
            catch (Exception ex)
            {
                return "Error";
            }

        }

        public static string ToShamsiDate(this DateTime DateTime1)
        {
            PersianCalendar PersianCalendar1 = new PersianCalendar();
            return string.Format(@"{0}/{1}/{2}",
                         PersianCalendar1.GetYear(DateTime1),
                         PersianCalendar1.GetMonth(DateTime1).ToString("00"),
                         PersianCalendar1.GetDayOfMonth(DateTime1).ToString("00"));
        }

        public static string ToShamsiClock(this DateTime dateTime)
        {
            PersianCalendar PersianCalendar1 = new PersianCalendar();
            return string.Format(@"{0}:{1}",
                         PersianCalendar1.GetHour(dateTime).ToString("00"),
                         PersianCalendar1.GetMinute(dateTime).ToString("00"));
        }

        public static DateTime StringToDateTime(this string value)
        {
            string[] dateTmp = value.Split(' ');
            string[] date = dateTmp[0].Split('/');
            string[] time;
            if (dateTmp.Length == 2)
            {
                time = dateTmp[1].Split(':');
            }
            else
                time = new string[2] { "0", "0" };
            //date = value.Split('/');
            PersianCalendar persianCal = new PersianCalendar();
            return persianCal.ToDateTime(Convert.ToInt16(date[0]), Convert.ToInt16(date[1]), Convert.ToInt16(date[2]), int.Parse(time[0]), int.Parse(time[1]), int.Parse(time[2]), 0);

            return new DateTime(Convert.ToInt16(date[0]), Convert.ToInt16(date[1]), Convert.ToInt16(date[2]));
            // return new JalaliDate(Convert.ToInt16(date[0]), Convert.ToByte(date[1]), Convert.ToByte(date[2])).ToDateTime().AddHours(double.Parse(time[0])).AddMinutes(double.Parse(time[1]));

        }



        public static DateTime StringDateToDateTime(this string value)
        {


            string[] dateTmp = value.Split(' ');
            string[] date = dateTmp[0].Split('/');
            string[] time;
            if (dateTmp.Length == 2)
            {
                time = dateTmp[1].Split(':');
            }
            else
                time = new string[2] { "0", "0" };
            //date = value.Split('/');
            PersianCalendar persianCal = new PersianCalendar();
            return persianCal.ToDateTime(Convert.ToInt16(date[0]), Convert.ToInt16(date[1]), Convert.ToInt16(date[2]), int.Parse(time[0]), int.Parse(time[1]), 0, 0);

            return new DateTime(Convert.ToInt16(date[0]), Convert.ToInt16(date[1]), Convert.ToInt16(date[2]));
            // return new JalaliDate(Convert.ToInt16(date[0]), Convert.ToByte(date[1]), Convert.ToByte(date[2])).ToDateTime().AddHours(double.Parse(time[0])).AddMinutes(double.Parse(time[1]));

        }



        public static TimeSpan StringToTimeSpan(this string value)
        {
            try
            {
                var split = value.Split(":");
                return new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                return new TimeSpan();
            }
        }

        public static string DayOfWeekShamsi(this DayOfWeek day)
        {
            switch (day)
            {
                case (DayOfWeek.Saturday):
                    return "شنبه";
                    break;

                case (DayOfWeek.Sunday):
                    return "یک شنبه";
                    break;
                case (DayOfWeek.Monday):
                    return "دو شنبه";
                case (DayOfWeek.Tuesday):
                    return "سه شنبه";
                case (DayOfWeek.Wednesday):
                    return "چهار شنبه";
                case (DayOfWeek.Thursday):
                    return "پنج شنبه";
                case (DayOfWeek.Friday):
                    return "جمعه";
                default:
                    return "";
            }
        }
    }


    //public static class DateConvertor
    //{
    //    public static string ToShamsi(this DateTime date)
    //    {
    //        PersianCalendar persian = new PersianCalendar();
    //        return persian.GetYear(date) + "/" + persian.GetMonth(date).ToString("00") + "/" + persian.GetDayOfMonth(date).ToString("00");
    //    }
    //}
}
