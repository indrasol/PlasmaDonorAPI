using System.Text.Json.Nodes;

namespace NewPlasmaDonorsAPI.utils
{
    public static class NullUtils
    {
        // Check if string is valid (non-null, non-empty, and not "null")
        public static bool IsValid(string s)
        {
            return !string.IsNullOrWhiteSpace(s) && !s.Equals("null", StringComparison.OrdinalIgnoreCase);
        }

        // Check if number is valid (non-null)
        public static bool IsValid(int? s)
        {
            return s.HasValue;
        }

        // Check if collection is valid (non-null and not empty)
        public static bool IsValid<T>(ICollection<T> cols)
        {
            return cols != null && cols.Count > 0;
        }

        // Check if JsonNode (JObject) is valid (non-null)
        public static bool IsValid(JsonNode jsonNode)
        {
            return jsonNode != null;
        }

        // Check if object is valid (non-null)
        public static bool IsValid(object o)
        {
            return o != null;
        }

        // Return the value or default if the value is not valid
        public static string GetOrDefault(string val, string defaultVal)
        {
            return IsValid(val) ? val : defaultVal;
        }

        // Check if array is valid (non-null and non-empty)
        public static bool IsValid<T>(T[] arr)
        {
            return arr != null && arr.Length > 0;
        }

        public static long? GetNumValue(object obj)
        {
            if (obj != null && IsValid(Convert.ToString(obj)))
            {
                if (long.TryParse(Convert.ToString(obj), out long result))
                {
                    return result;
                }
            }
            return null;
        }

    }
}
