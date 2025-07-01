using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace OTELSerilogDemo.Common;

public static class LoggerExtensions
{
    /// <summary>
    ///     According to https://opentelemetry.io/docs/specs/semconv/code/#attributes there is a canonical way to log
    ///     the code locaion.
    ///     This is how it could be done
    /// </summary>
    public static void LogInformationWithCallerInfo(
        this ILogger logger,
        string message,
        object? arg,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var state = new Dictionary<string, object>
        {
            {"code.function", memberName},
            {"code.filepath", sourceFilePath},
            {"code.lineno", sourceLineNumber}
        };

        using (logger.BeginScope(state))
        {
            // ReSharper disable once TemplateIsNotCompileTimeConstantProblem
            logger.LogInformation(message, arg);
        }
    }
}