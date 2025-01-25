namespace Whimsical.Debug
{
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class DebugExtensions
    {
        private const string Template = "[{0}][{1}] ({2}) => {3}";

        public static void Log(string message, [CallerFilePath] string filePath = "",
                               [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogFormat(Template, Path.GetFileName(filePath), memberName, lineNumber, message);
        }

        public static void LogError(string message, [CallerFilePath] string filePath = "",
                                    [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogErrorFormat(Template, Path.GetFileName(filePath), memberName, lineNumber, message);
        }

        public static void LogWarning(string message, [CallerFilePath] string filePath = "",
                                      [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogWarningFormat(Template, Path.GetFileName(filePath), memberName, lineNumber, message);
        }

        public static void LogAssertion(string message, [CallerFilePath] string filePath = "",
                                        [CallerMemberName] string memberName = "",
                                        [CallerLineNumber] int lineNumber = 0)
        {
            Debug.LogAssertionFormat(Template, Path.GetFileName(filePath), memberName, lineNumber, message);
        }
    }
}