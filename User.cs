namespace App;

public class User : IUser
{
  public string Username;
  public string Email;
  public string _Password;

  public User(string username, string email, string _password)
  {
    Username = username;
    Email = email;
    _Password = _password;
  }

  bool IUser.TryLogin(string username, string password)
  {
    return username == Username && password == _Password;
  }
}