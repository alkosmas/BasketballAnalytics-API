namespace BasketballAnalytics.Application.Exceptions;

public class UserAlreadyExistsException : Exception
{
    public UserAlreadyExistsException(string username) 
        : base($"Username '{username}' already exists")
    {
    }
}