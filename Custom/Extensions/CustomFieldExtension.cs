using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Sitefinity.DynamicModules.Model;
using Telerik.Sitefinity.Model;

namespace SitefinityWebApp.Custom.Extensions
{
    public static class CustomFieldExtension
    {
        public static string GetStringFieldValue(this IDynamicFieldsContainer item, string fieldName, bool required = false)
        {
            bool fieldExists = item.DoesFieldExist(fieldName);

            if (fieldExists == false && required)
            {
                throw new Exception("Field '" + fieldName + "' does not exists.");
            }
            else if (fieldExists == false)
            {
                return string.Empty;
            }

            object valueObject = item.GetValue(fieldName);

            // Validation of the custom property field nullable.
            if (valueObject == null && required == true)
            {
                throw new Exception("The custom field '" + fieldName + "' has NULL value.");
            }
            else if (valueObject == null)
            {
                return string.Empty;
            }

            return item.GetValue(fieldName).ToString().Trim();
        }

        public static int GetIntFieldValue(this IDynamicFieldsContainer item, string fieldName, bool required = false)
        {
            string fieldValue = GetStringFieldValue(item, fieldName, required);

            // If null but not required, return a default value.
            if (string.IsNullOrEmpty(fieldValue) && required == false)
            {
                return 0;
            }

            int integerValue = 0;

            bool converted = int.TryParse(fieldValue, out integerValue);

            // If the field is required. We cannot return a default value if the conversion failed.
            if (converted == false && required == true)
            {
                string errorMessage = string.Format("The dynamicContent custom field `{0}` cannot be converted to integer, because the value is `{1}`",
                    fieldName,
                    fieldValue);

                throw new Exception(errorMessage);
            }

            return integerValue;
        }

        public static decimal GetDecimalFieldValue(this IDynamicFieldsContainer item, string fieldName, bool required = false)
        {
            string fieldValue = GetStringFieldValue(item, fieldName, required);

            if (string.IsNullOrEmpty(fieldValue) && required == false)
            {
                return 0;
            }

            decimal decimalValue = 0;

            bool converted = decimal.TryParse(fieldValue, out decimalValue);

            // If the field is required. We cannot return a default value if the conversion failed.
            if (converted == false && required == true)
            {
                string errorMessage = string.Format("The dynamicContent custom field `{0}` cannot be converted to decimal, because the value is `{1}`",
                    fieldName,
                    fieldValue);

                throw new Exception(errorMessage);
            }

            return decimalValue;
        }

        public static bool GetBoolFieldValue(this IDynamicFieldsContainer item, string fieldName, bool required = false)
        {
            return Convert.ToBoolean(GetStringFieldValue(item, fieldName, required));
        }

        public static string GetChoiceFieldValue(this IDynamicFieldsContainer item, string fieldName, bool required = false)
        {
            bool fieldExists = item.DoesFieldExist(fieldName);

            if (fieldExists == false && required)
            {
                throw new Exception("Field '" + fieldName + "' does not exists.");
            }
            else if (fieldExists == false)
            {
                return string.Empty;
            }

            return item.GetValue<ChoiceOption>(fieldName).PersistedValue;
        }

        public static DateTime GetDateTimeFieldValue(this IDynamicFieldsContainer item, string fieldName, bool required = false)
        {
            string dateTimeValue = GetStringFieldValue(item, fieldName, required);

            if (string.IsNullOrEmpty(dateTimeValue) && required == false)
            {
                return new DateTime();
            }

            return Convert.ToDateTime(dateTimeValue);
        }
    }
}