using System.Text;
using System.Web;

namespace NewPlasmaDonorsAPI.utils
{
    public class NameUtils
    {
		public static string Appender(string delimiter, bool isName, params string[] names)
		{
			return Appender(delimiter, names.ToList(), isName);
		}

		public static string Appender(string delimiter, params string[] names)
		{
			return Appender(delimiter, names.ToList(), true);
		}

		public static string Appender(string delimiter, List<string> names, bool isName)
		{
			var name = new StringBuilder();

			if (names != null && names.Any())
			{
				foreach (var n in names)
				{
					if (IsValid(n))
					{
						name.Append(n.Trim() + delimiter);
					}
				}
			}

			name = name.Length > 0 ? new StringBuilder(name.ToString().Trim()) : new StringBuilder();
			if (name.Length > 0 && name[name.Length - 1] == ',')
			{
				name.Remove(name.Length - 1, 1);
			}

			if (isName)
			{
				name = new StringBuilder(Name(name.ToString()));
			}

			return name.ToString();
		}

		public static string UrlEncode(string text)
		{
			return text != null ? HttpUtility.UrlEncode(text) : text;
		}

		public static string UrlDecode(string text)
		{
			return text != null ? HttpUtility.UrlDecode(text) : text;
		}

		public static string Name(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}

			name = name.ToLower();
			return IsValid(name) ? Capitalize(name).Trim() : string.Empty;
		}

		private static string Capitalize(string name)
		{
			if (string.IsNullOrEmpty(name))
				return string.Empty;

			return string.Join(" ", name.Split(' ').Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
		}

		public static string StrVal(object o)
		{
			return o?.ToString();
		}

		public static string Gender(string strVal)
		{
			return strVal;
		}

		public static DateTime? DateVal(object o)
		{
			if (o == null)
			{
				return null;
			}
			return o is DateTime ? (DateTime?)o : null;
		}

		public static bool? BoolVal(object o)
		{
			if (o == null)
			{
				return null;
			}
			return bool.TryParse(o.ToString(), out bool result) ? (bool?)result : null;
		}

		public static long? LongVal(object obj)
		{
			if (obj != null && IsValid(obj.ToString()))
			{
				return long.TryParse(obj.ToString(), out long result) ? (long?)result : null;
			}
			return null;
		}

		public static double? DoubleVal(object? obj)
		{
			if (obj == null)
			{
				return null;
			}

			return double.TryParse(obj.ToString(), out double result) ? (double?)result : null;
		}

		private static bool IsValid(string value)
		{
			return !string.IsNullOrEmpty(value);
		}


	}
}
