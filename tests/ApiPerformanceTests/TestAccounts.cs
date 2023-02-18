namespace Api.PerformanceTests;

public static class TestAccounts
{
    public static Account TestAccount1()
    {
        return new Account
        {
            Email = "pkwola@gmail.com",
            Firstname = "Szymon",
            Surname = "Konski",
            Password = "Test1234!",
            Groups = new List<string>()
        };
    }
}

public class Account
{
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public string Password { get; set; }
    public string UserId { get; set; }
    public TokenModel Token { get; set; }
    public List<string> Groups { get; set; }
}