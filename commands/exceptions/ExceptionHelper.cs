namespace CoarUtils.commands.exceptions {

  public static class ExceptionHelper {
    /// <summary>
    /// Gets the root exception message by traversing through all inner exceptions
    /// until finding one that doesn't suggest looking deeper
    /// </summary>
    public static string GetRootExceptionMessage(Exception exception) {
      if (exception == null)
        return string.Empty;

      var currentException = exception;
      var visitedExceptions = new HashSet<Exception>();

      while (currentException != null) {
        // Prevent infinite loops in case of circular references
        if (visitedExceptions.Contains(currentException))
          break;

        visitedExceptions.Add(currentException);

        var message = currentException.Message?.Trim() ?? string.Empty;

        // Check if this message suggests looking deeper
        if (!SuggestsLookingDeeper(message) && !string.IsNullOrEmpty(message)) {
          return message;
        }

        // Move to the next inner exception
        currentException = currentException.InnerException;
      }

      // If we couldn't find a "final" message, return the original exception message
      return exception.Message ?? string.Empty;
    }

    /// <summary>
    /// Gets all exception messages in the chain for debugging purposes
    /// </summary>
    public static List<string> GetAllExceptionMessages(Exception exception) {
      var messages = new List<string>();
      var currentException = exception;
      var visitedExceptions = new HashSet<Exception>();

      while (currentException != null) {
        if (visitedExceptions.Contains(currentException))
          break;

        visitedExceptions.Add(currentException);

        if (!string.IsNullOrEmpty(currentException.Message)) {
          messages.Add($"{currentException.GetType().Name}: {currentException.Message}");
        }

        currentException = currentException.InnerException;
      }

      return messages;
    }

    /// <summary>
    /// Determines if an exception message suggests looking at inner exceptions or status
    /// </summary>
    private static bool SuggestsLookingDeeper(string message) {
      if (string.IsNullOrEmpty(message))
        return false;

      var lowerMessage = message.ToLowerInvariant();

      // Common phrases that suggest looking deeper
      var deeperIndicators = new[]
      {
            "see innerexception",
            "see inner exception",
            "see status",
            "one or more errors occurred",
            "for more information",
            "check innerexception",
            "check inner exception",
            "refer to innerexception",
            "refer to inner exception",
            "see the inner exception",
            "additional information"
        };

      return deeperIndicators.Any(indicator => lowerMessage.Contains(indicator));
    }

    /// <summary>
    /// Gets a formatted string showing the exception chain
    /// </summary>
    public static string GetFormattedExceptionChain(Exception exception) {
      var messages = GetAllExceptionMessages(exception);
      var rootMessage = GetRootExceptionMessage(exception);

      var result = $"Root Cause: {rootMessage}\n\n";
      result += "Exception Chain:\n";

      for (int i = 0; i < messages.Count; i++) {
        result += $"{i + 1}. {messages[i]}\n";
      }

      return result;
    }
  }

  // Usage examples:
  public class ExceptionUsageExamples {
    public static void DemonstrateUsage() {
      try {
        // Your code that might throw exceptions
        ThrowNestedExceptions();
      } catch (Exception ex) {
        // Get just the root cause message
        string rootMessage = ExceptionHelper.GetRootExceptionMessage(ex);
        Console.WriteLine($"Root cause: {rootMessage}");

        // Get all messages for debugging
        var allMessages = ExceptionHelper.GetAllExceptionMessages(ex);
        Console.WriteLine("\nAll exception messages:");
        foreach (var msg in allMessages) {
          Console.WriteLine($"- {msg}");
        }

        // Get formatted output
        Console.WriteLine("\n" + ExceptionHelper.GetFormattedExceptionChain(ex));
      }
    }

    private static void ThrowNestedExceptions() {
      try {
        try {
          throw new InvalidOperationException("The actual root cause: Invalid API key provided");
        } catch (Exception ex) {
          throw new ApplicationException("There was an error processing the geocoding request. See Status or InnerException for more information.", ex);
        }
      } catch (Exception ex) {
        throw new AggregateException("One or more errors occurred. (There was an error processing the geocoding request. See Status or InnerException for more information.)", ex);
      }
    }
  }
}