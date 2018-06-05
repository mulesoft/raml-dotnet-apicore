using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Raml.Common
{
	public class NetNamingMapper
	{
		private static readonly string[] ReservedWords = {"Get", "Post", "Put", "Delete", "Options", "Head", "ApiClient"};

		public static string GetNamespace(string title)
		{
			return Capitalize(RemoveIndalidChars(title));
		}

		public static string GetObjectName(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return "NullInput";

            var name = ReplaceSpecialChars(input, "{mediaTypeExtension}");
            name = ReplaceSpecialChars(name, "-");
			name = ReplaceSpecialChars(name, "\\");
			name = ReplaceSpecialChars(name, "/");
			name = ReplaceSpecialChars(name, "_");
			name = ReplaceSpecialChars(name, ":");
            name = ReplaceSpecialChars(name, "(");
            name = ReplaceSpecialChars(name, ")");
            name = ReplaceSpecialChars(name, "'");
            name = ReplaceSpecialChars(name, "`");
			name = ReplaceSpecialChars(name, "{");
			name = ReplaceSpecialChars(name, "}");

			name = RemoveIndalidChars(name);

			if (ReservedWords.Contains(name))
				name += "Object";

			if (StartsWithNumber(name))
				name = "O" + name;

			return name;
		}

		private static string ReplaceSpecialChars(string key, string separator)
		{
			return ReplaceSpecialChars(key, new[] {separator});
		}

		private static string ReplaceSpecialChars(string key, string[] separator)
		{
			var name = String.Empty;
			var words = key.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			return words.Aggregate(name, (current, word) => current + Capitalize(word));
		}

		public static string Capitalize(string word)
		{
			return word.Substring(0, 1).ToUpper() + word.Substring(1);
		}

		public static string RemoveIndalidChars(string input)
		{
#if !(PORTABLE || NETSTANDARD1_3 )
            var validnamespace = Path.GetInvalidPathChars()
				.Aggregate(input, (current, invalidChar) => 
					current.Replace(invalidChar.ToString(CultureInfo.InvariantCulture), string.Empty));
#else
            var validnamespace = Path.GetInvalidPathChars()
                .Aggregate(input, (current, invalidChar) =>
                    current.Replace(invalidChar.ToString(), string.Empty));
#endif
			validnamespace = validnamespace.Replace(" ", string.Empty);
			validnamespace = validnamespace.Replace(".", string.Empty);
			return validnamespace;
		}

		public static bool HasIndalidChars(string input)
		{
#if !(PORTABLE || NETSTANDARD1_3 )
			return Path.GetInvalidPathChars().Any(input.Contains);
#else
            return (input.IndexOfAny(Path.GetInvalidPathChars()) >= 0);
#endif
		}
		public static string GetMethodName(string input)
		{
            var name = ReplaceSpecialChars(input, "{mediaTypeExtension}");
            name = ReplaceSpecialChars(name, "-");
			name = ReplaceSpecialChars(name, "\\");
			name = ReplaceSpecialChars(name, "/");
			name = ReplaceSpecialChars(name, "_");
            name = ReplaceSpecialChars(name, "(");
            name = ReplaceSpecialChars(name, ")");
            name = ReplaceSpecialChars(name, "'");
            name = ReplaceSpecialChars(name, "`");
			name = ReplaceUriParameters(name);
			name = name.Replace(":", string.Empty);
			name = RemoveIndalidChars(name);

			if (StartsWithNumber(name))
				name = "M" + name;

			return name;
		}

		private static bool StartsWithNumber(string name)
		{
			var startsWithNumber = new Regex("^[0-9]+");
			var nameStartsWithNumber = startsWithNumber.IsMatch(name);
			return nameStartsWithNumber;
		}

		private static string ReplaceUriParameters(string input )
		{
			if (!input.Contains("{"))
				return input;

#if !(PORTABLE || NETSTANDARD1_3 )
			input = input.Substring(0, input.IndexOf("{", StringComparison.InvariantCulture)) + "By" +
			        input.Substring(input.IndexOf("{", StringComparison.InvariantCulture));
#else
            input = input.Substring(0, input.IndexOf("{", StringComparison.Ordinal)) + "By" +
                    input.Substring(input.IndexOf("{", StringComparison.Ordinal));
#endif

			var name = String.Empty;
			var words = input.Split(new[] { "{", "}" }, StringSplitOptions.RemoveEmptyEntries);
			return words.Aggregate(name, (current, word) => current + Capitalize(word));
		}

		public static string GetPropertyName(string name)
		{
			var propName = name.Replace(":", string.Empty);
			propName = propName.Replace("/", string.Empty);
			propName = propName.Replace("-", string.Empty);
            propName = propName.Replace("`", string.Empty);
            propName = propName.Replace("+", "Plus");
            propName = propName.Replace(".", "Dot");
			propName = Capitalize(propName);

			if (StartsWithNumber(propName))
				propName = "P" + propName;

			return propName;
		}

	    public static string GetEnumValueName(string enumValue)
	    {
	        var value = enumValue
	            .Replace(":", string.Empty)
	            .Replace("/", string.Empty)
	            .Replace(" ", "_")
	            .Replace("-", "_")
                .Replace("+", string.Empty)
                .Replace(".", string.Empty);

            if (StartsWithNumber(value))
                value = "E" + value;

	        int number;
	        if (int.TryParse(enumValue, out number))
	            value = value + " = " + number;

            return value;
	    }
	}
}