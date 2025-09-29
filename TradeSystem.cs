namespace App;

public class TradeSystem
{
  public List<IUser> users = new List<IUser>();

  public TradeSystem()
  {
    users.Add(new User("Jacob", "jake@", "pass"));
  }

  //Inloggings funktion, gör så att användaren kan logga in på sitt konto.
  public IUser? Login()
  {
    //Skapar en interface variabel som har koll på active user.
    IUser? active_user = null;

    //Kollar ifall det finns någon active user, ifall det inte är någon inloggad så kör den funktionen.
    if (active_user == null)
    {

      Console.Write("Username: ");

      //Användare skriver in sitt username
      string username = Console.ReadLine();

      Console.Write("Password: ");

      //Användare skriver in sitt lösenord
      string password = Console.ReadLine();


      //foreach funktion, som kollar igenom en lista av users som finns sparade.
      foreach (IUser user in users)
      {
        //Kollar ifall det användaren skrev innan matchar med vad som finns sparat i listan
        //ifall informationen stämmer så skickar den tillbaka informationen till det stället Login() funktionen blev kallad.
        //ifall informationen inte matchar så skickar den tillbaka null, alltså ingenting.
        if (user.TryLogin(username, password))
          return user;
      }
    }
    return null;
  }
}