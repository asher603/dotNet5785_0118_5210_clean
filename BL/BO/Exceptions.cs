namespace BO;
[Serializable]
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }
    public BlDoesNotExistException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlInvalidInputException : Exception
{
    public BlInvalidInputException(string? message) : base(message) { }
    public BlInvalidInputException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlCanNotCanceleException : Exception
{
    public BlCanNotCanceleException(string? message) : base(message) { }
    public BlCanNotCanceleException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException)
                : base(message, innerException) { }
}

[Serializable]
public class BlCallExpiredException : Exception
{
    public BlCallExpiredException(string? message) : base(message) { }
    public BlCallExpiredException(string message, Exception innerException)
                : base(message, innerException) { }
}

public class BlNullReferenceException : Exception
{
    public BlNullReferenceException(string? message) : base(message) { }
    public BlNullReferenceException(string message, Exception innerException)
                : base(message, innerException) { }
}

public class BlDeletionImpossibleException : Exception
{
    public BlDeletionImpossibleException(string? message) : base(message) { }
    public BlDeletionImpossibleException(string message, Exception innerException)
                : base(message, innerException) { }
}

public class BlCanNotEndCallException : Exception
{
    public BlCanNotEndCallException(string? message) : base(message) { }
    public BlCanNotEndCallException(string message, Exception innerException)
                : base(message, innerException) { }
}

public class BlCanNotUpdateException : Exception
{
    public BlCanNotUpdateException(string? message) : base(message) { }
    public BlCanNotUpdateException(string message, Exception innerException)
                : base(message, innerException) { }
}

public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException(string? message) : base(message) { }
    public BLTemporaryNotAvailableException(string message, Exception innerException)
                : base(message, innerException) { }
}

public class BlPasswordInCorrect : Exception
{
    public BlPasswordInCorrect(string? message) : base(message) { }
    public BlPasswordInCorrect(string message, Exception innerException)
                : base(message, innerException) { }
}
