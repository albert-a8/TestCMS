using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SitefinityWebApp.Custom.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            // If the attribute has no value, use the name of the enum instead.
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}