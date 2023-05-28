using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public static class GraphDataPositionCalculator 
{
    public static float CalculateNormalizedPosition(string[] data, string value)
    {
        if (IsNumeric(value))
        {
            // Numeric data, calculate position based on range
            float minValue = GetMinNumericValue(data);
            float maxValue = GetMaxNumericValue(data);
            float numericValue = float.Parse(value);

            return Mathf.InverseLerp(minValue, maxValue, numericValue);
        }
        else if (IsDate(value))
        {
            // Date data, calculate position based on range of dates
            DateTime minDate = GetMinDateValue(data);
            DateTime maxDate = GetMaxDateValue(data);
            DateTime dateValue = DateTime.ParseExact(value, "dd/MM/yyyy", null);

            TimeSpan dateRange = maxDate - minDate;
            TimeSpan dateDifference = dateValue - minDate;

            return (float)dateDifference.TotalDays / (float)dateRange.TotalDays;
        }
        else
        {
            // Category data, calculate position based on number of unique categories
            HashSet<string> categories = GetUniqueCategories(data);
            int categoryIndex = GetCategoryIndex(categories, value);

            return (float)categoryIndex / (float)(categories.Count - 1);
        }
    }

    public static bool IsNumeric(string value)
    {
        float result;
        return float.TryParse(value, out result);
    }

    public static float GetMinNumericValue(string[] data)
    {
        float minValue = float.MaxValue;

        foreach (string value in data)
        {
            float numericValue;
            if (float.TryParse(value, out numericValue))
            {
                if (numericValue < minValue)
                    minValue = numericValue;
            }
        }

        return minValue;
    }

    public static float GetMaxNumericValue(string[] data)
    {
        float maxValue = float.MinValue;

        foreach (string value in data)
        {
            float numericValue;
            if (float.TryParse(value, out numericValue))
            {
                if (numericValue > maxValue)
                    maxValue = numericValue;
            }
        }

        return maxValue;
    }

    public static bool IsDate(string value)
    {
        DateTime dateValue;
        return DateTime.TryParseExact(value, "dd/MM/yyyy", null, DateTimeStyles.None, out dateValue);
    }

    public static DateTime GetMinDateValue(string[] data)
    {
        DateTime minDate = DateTime.MaxValue;

        foreach (string value in data)
        {
            DateTime dateValue;
            if (DateTime.TryParseExact(value, "dd/MM/yyyy", null, DateTimeStyles.None, out dateValue))
            {
                if (dateValue < minDate)
                    minDate = dateValue;
            }
        }

        return minDate;
    }

    public static DateTime GetMaxDateValue(string[] data)
    {
        DateTime maxDate = DateTime.MinValue;

        foreach (string value in data)
        {
            DateTime dateValue;
            if (DateTime.TryParseExact(value, "dd/MM/yyyy", null, DateTimeStyles.None, out dateValue))
            {
                if (dateValue > maxDate)
                    maxDate = dateValue;
            }
        }

        return maxDate;
    }

    public static HashSet<string> GetUniqueCategories(string[] data)
    {
        HashSet<string> categories = new HashSet<string>();

        foreach (string value in data)
        {
            categories.Add(value);
        }

        return categories;
    }

    public static int GetCategoryIndex(HashSet<string> categories, string value)
    {
        string[] sortedCategories = categories.OrderBy(x => x).ToArray();
        return Array.IndexOf(sortedCategories, value);
    }

}
