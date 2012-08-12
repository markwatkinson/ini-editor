using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TAIniEditor
{

    public class Validation
    {
        public static bool IsBool(string value) 
        {
            string s = value.ToLower();
            return s == "true" || s == "false";
        }
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, "^[0-9]+$");
        }
        public static bool IsFloat(string value)
        {
            return Regex.IsMatch(value, @"^[0-9]*\.[0-9]+$");
        }
        public static bool IsString(string value)
        {
            //...
            return true;
        }

        public static OptionType GuessType(string value)
        {
            if (IsBool(value)) return OptionType.Bool;
            else if (IsFloat(value)) return OptionType.Float;
            else if (IsInt(value)) return OptionType.Int;
            else return OptionType.String;
        }
    }

    interface IValidator<T>
    {
        bool Test(T input, out string message);
    }

    class BoolValidator : IValidator<bool>
    {
        public bool AllowTrue = true;
        public bool AllowFalse = true;

        public bool Test(bool input, out string message)
        {
            message = null;
            if (AllowFalse && !input) return true;
            if (AllowTrue && input) return true;
            return false;
        }
    }

    class RangeValidator<T> : IValidator<T>
        where T : IComparable
    {

        public T Lower;
        public T Upper;

        public RangeValidator(T lower, T upper)
        {
            Lower = lower;
            Upper = upper;
        }

        public bool Test(T input, out string message)
        {
            message = null;
            return input.CompareTo(Lower) >= 0 && input.CompareTo(Upper) <= 0;
        }

    }
}
