using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAIniEditor
{
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
