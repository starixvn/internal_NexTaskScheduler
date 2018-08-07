namespace NexTaskScheduler.Core.Extentions
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    public static class Extentions
    {
        public static string ToShortFormatedDate(this DateTime date)
        {
            string dateStr = date.ToString(@"dd MMM, yyyy - HH") + "h" + date.ToString(@"mm\'");
            return dateStr;
        }

        public static string GetDisplayName(this Enum enumeration)
        {
            var type = enumeration.GetType();
            var memberInfo = type.GetMember(enumeration.ToString());

            // if there is member information
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);

                if (attributes.Length > 0)
                {
                    return ((DisplayAttribute)attributes.First()).GetName();
                }
            }

            return enumeration.ToString();
        }
    }
}