namespace Application.Security;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthenticateAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AuthorizeAttribute" /> class.
    /// </summary>
    public AuthenticateAttribute()
    {
    }
}