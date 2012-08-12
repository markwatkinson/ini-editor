using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TAIniEditor
{

    public enum OptionType
    {
        String,
        Bool,
        Int,
        Float
    }

    public class RootContext
    {

        public List<Category> Categories { get; set; }
        /// <summary>
        /// Any options defined in the root, i.e. not under a category.
        /// It is necessarily true, since categories cannot nest, that
        /// any options defined in this list occur BEFORE any categories
        /// in the source.
        /// </summary>
        public List<Option> Options { get; set; }

        public RootContext()
        {
            Categories = new List<Category>();
            Options = new List<Option>();
        }
    }
    public class Category
    {
        public string Name { get; set; }
        public List<Option> Options { get; set; }

        public Category()
        {
            Options = new List<Option>();
        }
    }

    public class Option
    {
        private string _value;
        private bool typeOverriden = false;

        public string Name { get; set; }
        public string Value { 
            get { return _value; } 
            set { 
                _value = value;
                if (!typeOverriden)
                {
                    GuessType();
                    typeOverriden = true;
                }
            }
        }
        public OptionType Type { get; set; }
        public string Description;

        public Category Category { get; set; }

        public int LineNumber { get; set; }

        public void LockType(OptionType type)
        {
            Type = type;
            typeOverriden = true;
        }

        private void GuessType()
        {
            string lowerVal = Value.ToLower();
            if (Regex.IsMatch(Value, "^[0-9]+$"))
            {
                Type = OptionType.Int;
            }
            else if (Regex.IsMatch(Value, @"^[0-9]*\.[0-9]+$"))
            {
                Type = OptionType.Float;
            }
            else if (lowerVal == "true" || lowerVal == "false")
            {
                Type = OptionType.Bool;
            }
            else
            {
                Type = OptionType.String;
            }
        }

    }


}
