namespace App;

public class TradeSystem
{
  public List<IUser> users = new List<IUser>();
  public List<Item> items = new List<Item>();

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

  public void MakeAccount()
  {
    string name;
    string email;
    string _password;

    Console.Write("Namn: ");
    name = Console.ReadLine();

    Console.Write("Email: ");
    email = Console.ReadLine();

    _password = Console.ReadLine();

    users.Add(new User(name, email, _password));

  }

  public void AddItem(IUser? active_user)
  {
    string name;
    string description;
    if (active_user is User u)
    {
      Console.Write("Namn på ditt Item: ");
      name = Console.ReadLine();

      Console.Write("Skriv en beskrivning om ditt Item: ");
      description = Console.ReadLine();

      items.Add(new Item(name, description, u.Email));
    }


  }

  public void ShowItems()
  {
    foreach (Item item in items)
    {
      Console.WriteLine(item.Name + item.Description + item.Owner);
    }
    Console.ReadKey(true);
  }

  public void ShowMyItems(IUser? active_user)
  {
    foreach (Item item in items)
    {
      if (active_user is User u && item.Owner == u.Email)
      {
        Console.WriteLine(item.Name + " " + item.Description);
      }
    }
  }
}