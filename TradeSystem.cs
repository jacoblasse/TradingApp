namespace App;

public class TradeSystem
{
  public List<IUser> users = new List<IUser>();

  public TradeSystem()
  {
    users.Add(new User("Jacob", "jake@", "pass"));
  }

  public IUser? Login()
  {
    IUser? active_user = null;
    if (active_user == null)
    {
      Console.Write("Username: ");
      string username = Console.ReadLine();

      Console.Write("Password: ");
      string password = Console.ReadLine();

      foreach (IUser user in users)
      {
        if (user.TryLogin(username, password))
          return user;
      }
    }
    return null;
  }
}