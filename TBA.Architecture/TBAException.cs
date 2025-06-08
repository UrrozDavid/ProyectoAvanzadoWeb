/// <summary>
/// Custom exception class for PAW-specific exceptions.
/// </summary>
public class TBAException : Exception
{
    private readonly Exception exception;

    /// <summary>
    /// Initializes a new instance of the <see cref="PAWException"/> class without an inner exception.
    /// </summary>
    public TBAException()
    {
        exception = new Exception();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PAWException"/> class with an exception message.
    /// </summary>
    public TBAException(string message)
    {
        exception = new Exception(message);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PAWException"/> class with a specified inner exception.
    /// </summary>
    /// <param name="ex">The inner exception that caused this exception.</param>
    public TBAException(Exception ex)
    {
        exception = ex;
    }

    /// <summary>
    /// Throws a new instance of <see cref="PAWException"/>.
    /// </summary>
    /// <returns>This method does not return a value.</returns>
    public static TBAException MustThrow()
    {
        return new TBAException();
    }

    /// <summary>
    /// Throws a new instance of <see cref="TBAException"/>.
    /// </summary>
    /// <returns>This method does not return a value.</returns>
    public static TBAException MustThrow(string message)
    {
        return new TBAException(message);
    }

    /// <summary>
    /// Throws a new instance of <see cref="PAWException"/> with a specified inner exception.
    /// </summary>
    /// <param name="ex">The inner exception that caused this exception.</param>
    /// <returns>This method does not return a value.</returns>
    public static TBAException MustThrow(Exception ex)
    {
        return new TBAException(ex);
    }

    /// <summary>
    /// Throws an exception if the specified value is null.
    /// </summary>
    /// <typeparam name="TOut">The type of the value to check.</typeparam>
    /// <param name="deseralized">The value to check for null.</param>
    /// <param name="message">The message to include in the exception if the value is null.</param>
    /// <exception cref="Exception">Thrown if <paramref name="deseralized"/> is null.</exception>
    internal static void ThrowIfNull<TOut>(TOut? deseralized, string message)
    {
        if (deseralized is null)
        {
            throw new Exception(message);
        }
    }

    /// <summary>
    /// Throws an exception if the specified object is null.
    /// </summary>
    /// <param name="deseralized">The object to check for null.</param>
    /// <param name="message">The message to include in the exception if the object is null.</param>
    /// <exception cref="Exception">Thrown if <paramref name="deseralized"/> is null.</exception>
    internal static void ThrowIfNull(object deseralized, string message)
    {
        if (deseralized is null)
        {
            throw new Exception(message);
        }
    }

    /// <summary>
    /// Throws an exception if the specified string is null or empty.
    /// </summary>
    /// <param name="clientBaseUrl">The string to check for null or empty.</param>
    /// <param name="message">The message to include in the exception if the string is null or empty.</param>
    /// <exception cref="Exception">Thrown if <paramref name="clientBaseUrl"/> is null or empty.</exception>
    internal static void ThrowIfNullOrEmpty(string? clientBaseUrl, string message)
    {
        if (string.IsNullOrEmpty(clientBaseUrl))
        {
            throw new Exception(message);
        }
    }
}
