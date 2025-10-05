using System.Runtime.InteropServices;

namespace App;

public class TradeSystem
{
  public List<IUser> users = new List<IUser>();
  public List<Item> items = new List<Item>();

  public List<Trade> trades = new List<Trade>();

  public TradeSystem()
  {
    users.Add(new User("Jacob", "jake@", "pass"));
    users.Add(new User("Kevin", "Kevv@", "pass"));
    items.Add(new Item("Vit Ps5", "Ett vit ps5, köpt när det kom ut", "Kevv@"));
    items.Add(new Item("Xbox 360", "Ett vit xbox 360 från 2006, fungerar fortfarande", "jake@"));


    trades.Add(new Trade
    (
      "jake@",
      "Kevv@",
      "Vit Ps5",
      "Xbox 360",
      TradeStatus.Pending
    ));

    trades.Add(new Trade
    (
      "jake@",
      "Kevv@",
      "Svart Ps5",
      "Xbox 360",
      TradeStatus.Completed
    ));
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

  public List<Item> ShowItems(IUser? active_user)
  {

    if (active_user is not User u)
    {
      Console.WriteLine("Logga in eller skapa ett konto för att se items");
      return null;
    }

    //Skapar en lista "otheritems" som sparar alla items förutom den aktiva användaren.
    List<Item> otheritems = new List<Item>();
    foreach (Item item in items)
    {
      if (item.Owner != u.Email)
      {
        otheritems.Add(item);
      }
    }
    if (otheritems.Count == 0)
    {
      Console.WriteLine("Det finns inga items att byta just nu");
      return null;
    }
    //Visar inte upp aktiva användarens items, men bara andra användares items
    for (int i = 0; i < otheritems.Count; i++)
    {
      Console.WriteLine($"[{i + 1}] - {otheritems[i].Name} - {otheritems[i].Description} - (Ägare {otheritems[i].Owner})");
    }
    return otheritems;


  }

  public void ShowMyItems(IUser? active_user)
  {
    int i = 0;
    foreach (Item item in items)
    {
      if (active_user is User u && item.Owner == u.Email)
      {
        i += 1;
        Console.WriteLine($"[{i}]\n{item.Name} \n{item.Description}");
      }
      else Console.WriteLine("Du har inga items på din profil\n Lägg till nya items?");
    }
  }

  public void MakeTrade(IUser? active_user)
  {
    List<Item> otheritems = ShowItems(active_user);
    Console.Clear();
    if (active_user is not User u)
    {
      Console.WriteLine("Du kan inte byta items för att du är inte en användare.");
      return;
    }

    if (otheritems == null)
    {
      Console.WriteLine("Det finns inga items att byta");
      return;
    }

    Console.WriteLine("Välj ett item att byta till");
    for (int i = 0; i < otheritems.Count; i++)
    {
      Console.WriteLine($"[{i + 1}] {otheritems[i].Name} - {otheritems[i].Description} (Ägare: {otheritems[i].Owner})");
    }

    string input = Console.ReadLine();
    int itemChoice;
    if (!int.TryParse(input, out itemChoice) || itemChoice < 1 || itemChoice > otheritems.Count)
    {
      Console.WriteLine("Oglitligt val.");
      return;
    }

    Item chosenItem = otheritems[itemChoice - 1];

    List<Item> myItems = new List<Item>();
    foreach (Item item in items)
      if (item.Owner == u.Email)
      {
        myItems.Add(item);
      }

    if (myItems.Count == 0)
    {
      Console.WriteLine("Du har inga items att byta");
      return;
    }

    Console.WriteLine("Välj ett av dina items att erbjuda i bytet: ");
    for (int i = 0; i < myItems.Count; i++)
    {
      Console.WriteLine($"[{i + 1}] {myItems[i].Name} - {myItems[i].Description}");
    }

    input = Console.ReadLine();
    int myItemChoice;
    if (!int.TryParse(input, out myItemChoice) || myItemChoice < 1 || myItemChoice > myItems.Count)
    {
      Console.WriteLine("Ogiltligt val");
      return;
    }
    Item offeredItem = myItems[myItemChoice - 1];

    trades.Add(new Trade
    (
      u.Email,
      chosenItem.Owner,
      chosenItem.Name,
      offeredItem.Name,
      TradeStatus.Pending
    ));

    Console.WriteLine("Trade-förfrågan har skickats");
  }

  public void ActiveTrades(IUser? active_user)
  {
    int ChoosenIndex;
    if (active_user is not User u)
    {
      Console.WriteLine("Du kan inte ha några aktiva byten för du är inte en inloggad användare");
      return;
    }

    while (true)
    {
      int index = 0;
      for (int i = 0; i < trades.Count; i++)
      {
        //Ser till så att det endast visas trades som inte är färdiga.
        //Ser till att det visas bara trades som har skickats till den aktiva användaren.
        if (trades[i].Status == TradeStatus.Pending && u.Email == trades[i].Receiver)
        {
          Console.WriteLine($"[{i + 1}] - {trades[i].Sender} vill byta sitt item: {trades[i].OfferedItem} mot ditt item: {trades[i].RequestedItem}");
          index += 1;


        }
        else if (trades[i].Status == TradeStatus.Pending && u.Email != trades[i].Receiver || index == 0)
        {
          Console.WriteLine("Du har inga aktiva byten just nu.");
          return;
        }
      }

      Console.WriteLine("Välj en av dina aktiva trades för att kunna acceptera eller avböja, Eller tryck enter för att gå tillbaka");
      string choosen = Console.ReadLine().ToLower();

      if (!int.TryParse(choosen, out int choosenint) || choosenint < 1 || choosenint > index)
      {
        Console.Clear();
      }
      else if (int.TryParse(choosen, out choosenint) || choosenint > 1 || choosenint < index)
      {
        Console.Clear();
        ChoosenIndex = choosenint;
        break;
      }
      if (choosen == "" || choosen == null)
        return;

    }




    Console.WriteLine("Om du vill acceptera detta bytet skriv (ja) om du inte accepterar skriv (nej)");
    Console.WriteLine("Du kan alltid gå tillbaka genom att trycka enter");

    while (true)
    {
      string input = Console.ReadLine().ToLower();
      if (input == "ja")
      {
        Console.WriteLine("Byte accepterat");
        trades[ChoosenIndex - 1].Status = TradeStatus.Accepted;
        break;
      }
      else if (input == "nej")
      {
        Console.WriteLine("Byte inte accepterat");
        trades[ChoosenIndex - 1].Status = TradeStatus.Denied;
        break;
      }
      else if (input == "" || input == null)
      {
        return;
      }
    }




    /*
    foreach (Trade trade in trades)
    {
      Console.WriteLine($"{trade.Sender} vill byta sitt item: {trade.OfferedItem} mot ditt item: {trade.RequestedItem}.");

    }
    */
  }
}