using System;

namespace Expressions {

    /// <summary>
    /// Internal extension methods.  Used to eliminate any framework dependencies
    /// </summary>
    internal static class Extensions {

        public static string ToNull(this string str) {
            if (string.IsNullOrEmpty(str)) {
                return null;
            }

            str = str.Trim();
            if (str.Length == 0) {
                return null;
            }
            else {
                return str;
            }
        }

        public static bool IsNull(this string str) {
            if (string.IsNullOrEmpty(str)) {
                return true;
            }
            else if (string.IsNullOrEmpty(str.Trim())) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
