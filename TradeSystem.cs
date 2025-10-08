using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace App;

public class TradeSystem
{
  // Lista för alla användare 
  public List<IUser> users = new List<IUser>();
  // Lista för alla items 
  public List<Item> items = new List<Item>();

  // Lista över alla trade-förfrågningar 
  public List<Trade> trades = new List<Trade>();

  public TradeSystem()
  {


    string[] users_csv = File.ReadAllLines("users.csv");
    foreach (string user_data in users_csv)
    {
      string[] split_user_data = user_data.Split("|");
      if (split_user_data.Length >= 3)
      {
        users.Add(new User(split_user_data[0], split_user_data[1], split_user_data[2]));
      }
    }

    string[] items_csv = File.ReadAllLines("items.csv");
    foreach (string item_data in items_csv)
    {
      string[] split_item_data = item_data.Split("|");
      if (split_item_data.Length >= 2)
      {
        items.Add(new Item(split_item_data[0], split_item_data[1], split_item_data[2]));
      }
    }

    string[] trades_csv = File.ReadAllLines("trades.csv");
    foreach (string trade_data in trades_csv)
    {
      string[] split_trade_data = trade_data.Split("|");
      if (split_trade_data.Length >= 5)
      {
        TradeStatus status;
        if (!Enum.TryParse(split_trade_data[4], out status))
        {
          // Om status inte kan läsas, sätt till Pending
          status = TradeStatus.Pending;
        }
        trades.Add(new Trade(split_trade_data[0], split_trade_data[1], split_trade_data[2], split_trade_data[3], status));
      }
    }
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

  //Funktion för att skapa konto.
  public void MakeAccount()
  {
    //Skapar variabler för användare information.
    string name;
    string email;
    string _password;

    //Sparar information som användare skriver in i variablerna.
    Console.Write("Namn: ");
    name = Console.ReadLine();

    Console.Write("Email: ");
    email = Console.ReadLine();

    _password = Console.ReadLine();

    //Lägger till information från variablerna ovan, så det sparas i User klassen i User.cs.
    users.Add(new User(name, email, _password));

    //Lägger till samma information i users.csv filen så att information är sparad även efter programmet stängs av.
    File.AppendAllLines("users.csv", new[] { $"{name}|{email}|{_password}" });

  }

  //Lägga till item funktion
  public void AddItem(IUser? active_user)
  {
    string name;
    string description;
    //Kollar ifall aktiva användaren är en User och skapar variabel med information från klassen User
    if (active_user is User u)
    {
      Console.Write("Namn på ditt Item: ");
      name = Console.ReadLine();

      Console.Write("Skriv en beskrivning om ditt Item: ");
      description = Console.ReadLine();

      //Lägger till information från variablerna ovan, så det sparas i Item klassen i Item.cs
      items.Add(new Item(name, description, u.Email));
      //Lägger till samma information i items.csv filen så att information är sparad även efter programmet stängs av.
      File.AppendAllLines("items.csv", new[] { $"{name}|{description}|{u.Email}" });
    }


  }
  //Skapar showitem funktion som visar alla items som inte tillhör aktiv användare med en List<Item> typ så jag kan spara informationen och använda i en annan funktion senare i koden.
  public List<Item> ShowItems(IUser? active_user)
  {

    //Kollar ifall användaren inte är en User och skapar en variabel u med information från klassen User i User.cs
    if (active_user is not User u)
    {
      Console.WriteLine("Logga in eller skapa ett konto för att se items");

      //Skickar tillbaka användaren till vart funktionen blev kallad ifrån utan någon information.
      return null;
    }

    //Skapar en lista "otheritems" som sparar alla items förutom den aktiva användaren.
    List<Item> otheritems = new List<Item>();

    //gör en foreach som kollar igenom all information från klassen Item sparad i listan items.
    foreach (Item item in items)
    {
      //Ifall ägaren av ett item inte är samma som aktiva användaren så spara all information om det specifika itemet i listan otheritems.
      if (item.Owner != u.Email)
      {
        otheritems.Add(item);
      }
    }
    //Om det inte finns några items sparade i other items, så finns det inga items som användaren kan byta mot.
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

  //Skapar funktion som visar aktiva användarens items.
  public void ShowMyItems(IUser? active_user)
  {
    //skapar en int i som ska hålla koll på hur många items användaren har, den har ingen annan funktion.
    int i = 0;
    foreach (Item item in items)
    {
      //Kollar ifall den aktiva användaren är en User och skapar variabel u med information från User klassen i User.cs.
      //Kollar så att den som äger itemet är samma person som är inloggad.
      if (active_user is User u && item.Owner == u.Email)
      {
        i += 1;
        Console.WriteLine($"[{i}] - {item.Name} - {item.Description}");
      }
    }
    if (i == 0)
    {
      Console.WriteLine("Du har inga items");
      return;
    }
    if (i < 2)
    {
      Console.WriteLine($"Du har {i} item i ditt föråd");
    }
    else
    {
      Console.WriteLine($"Du har {i} items i ditt föråd");
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

    File.AppendAllLines("trades.csv", new[] { $"{u.Email}|{chosenItem.Owner}|{chosenItem.Name}|{offeredItem.Name}|{TradeStatus.Pending}" });

    Console.WriteLine("Trade-förfrågan har skickats");
  }

  public void ReceivedActiveTrades(IUser? active_user)
  {
    if (active_user is not User u)
    {
      Console.WriteLine("Du kan inte ha några aktiva byten för du är inte en inloggad användare");
      return;
    }

    //Skapar en lista som sparar alla trades som är Pending och skickade till användaren
    List<int> tradeIndex = new List<int>();

    for (int i = 0; i < trades.Count; i++)
    {
      //Kollar efter trades som har Pending status och är skickade till användaren
      if (trades[i].Status == TradeStatus.Pending && u.Email == trades[i].Receiver)
      {
        //Sparar informationen i listan
        tradeIndex.Add(i);
      }
    }

    //Kollar ifall det finns några trades som sparades i listan
    //Ifall inga trades sparades betyder det att det inte finns några skickade byten till den aktiva användaren.
    if (tradeIndex.Count == 0)
    {
      Console.WriteLine("Du har inga aktiva byten just nu");

      //Skickar tillbaka användaren till vart funktionen blev kallad på ifrån.
      return;
    }


    for (int i = 0; i < tradeIndex.Count; i++)
    {
      int tradeidx = tradeIndex[i];
      Console.WriteLine($"[{i + 1}] - {trades[tradeidx].Sender} vill byta sitt item: {trades[tradeidx].OfferedItem} mot ditt item: {trades[tradeidx].RequestedItem}");
    }
    Console.WriteLine("Välj en av dina aktiva trades för att kunna acceptera eller avböja, Eller tryck enter för att gå tillbaka");
    string input = Console.ReadLine();
    if (input == null || input == "")
    {
      return;
    }

    if (!int.TryParse(input, out int choice) || choice < 1 || choice > tradeIndex.Count)
    {
      Console.WriteLine("fel val");
      return;
    }

    int selectedTradeIndex = tradeIndex[choice - 1];

    Console.WriteLine("Vill du acceptera bytet? Skriv (ja) för att acceptera, (nej) för att avböja, eller tryck enter för att gå tillbaka");
    while (true)
    {
      string answer = Console.ReadLine().ToLower();
      if (answer == "ja")
      {
        trades[selectedTradeIndex].Status = TradeStatus.Accepted;

        // Spara ändrade trades till trades.csv
        List<string> tradeRows = new List<string>();
        foreach (Trade currentTrade in trades)
        {
          string row = currentTrade.Sender + "|" + currentTrade.Receiver + "|" + currentTrade.RequestedItem + "|" + currentTrade.OfferedItem + "|" + currentTrade.Status;
          tradeRows.Add(row);
        }
        File.WriteAllLines("trades.csv", tradeRows);

        TradeAccepted(trades[selectedTradeIndex]);
        Console.WriteLine("Byte accepterat");
        break;
      }
      else if (answer == "nej")
      {
        trades[selectedTradeIndex].Status = TradeStatus.Denied;
        Console.WriteLine("Byte avböjt");
        break;
      }
      else if (answer == null || answer == "")
      {
        return;
      }
    }

  }

  public void OfferedActiveTrades(IUser? active_user)
  {
    int ChoosenIndex;
    if (active_user is not User u)
    {
      Console.WriteLine("Du kan inte ha några aktiva byten för du är inte en inloggad användare");
      return;
    }

    //Skapa lista med index för trades som användaren har skickat till någon och är pending
    List<int> OfferedTradeIndex = new List<int>();


    for (int i = 0; i < trades.Count; i++)
    {
      if (trades[i].Status == TradeStatus.Pending && u.Email == trades[i].Sender)
      {
        OfferedTradeIndex.Add(i);
      }

    }

    if (OfferedTradeIndex.Count == 0)
    {
      Console.WriteLine("Det finns inga trades som du har skickat som är aktiva just nu");
      return;
    }

    for (int i = 0; i < OfferedTradeIndex.Count; i++)
    {
      int tradeidx = OfferedTradeIndex[i];
      Console.WriteLine($"[{i + 1}] - Du vill byta ditt item: {trades[tradeidx].OfferedItem} mot {trades[tradeidx].Receiver} item: {trades[tradeidx].RequestedItem}");
    }
    Console.WriteLine("Vill du avbryta ett av dina trade offers?");
    Console.WriteLine("Välj isåfall indexet av det bytet du vill avbryta, eller tryck Enter för att gå tillbaka");

    string input = Console.ReadLine();
    if (input == null || input == "")
    {
      return;
    }

    if (!int.TryParse(input, out int choice) || choice < 1 || choice > OfferedTradeIndex.Count)
    {
      Console.WriteLine("fel val");
      return;
    }

    int selectedTradeIndex = OfferedTradeIndex[choice - 1];

    Console.WriteLine("Om du är säker på att du vill avbryta bytet skriv (ja), om du inte vill skriv (nej). Du kan alltid gå tillbaka genom att trycka Enter.");

    while (true)
    {
      string confirm = Console.ReadLine().ToLower();
      if (confirm == "ja")
      {
        trades[selectedTradeIndex].Status = TradeStatus.Canceled;

        // Spara ändrade trades till trades.csv
        List<string> tradeRows = new List<string>();
        foreach (Trade currentTrade in trades)
        {
          string row = currentTrade.Sender + "|" + currentTrade.Receiver + "|" + currentTrade.RequestedItem + "|" + currentTrade.OfferedItem + "|" + currentTrade.Status;
          tradeRows.Add(row);
        }
        File.WriteAllLines("trades.csv", tradeRows);

        Console.WriteLine("Byte avbrutet");
        break;
      }
      else if (confirm == "nej")
      {
        Console.WriteLine("Byte inte avbrutet");
        break;
      }
      else if (confirm == null || confirm == "")
      {
        return;
      }
    }

  }

  public void CompletedTrades(IUser? active_user)
  {
    if (active_user is not User u)
    {
      Console.WriteLine("Du måste vara inloggad för att se avklarade byten");
      return;
    }

    bool found = false;
    for (int i = 0; i < trades.Count; i++)
    {
      if ((trades[i].Sender == u.Email || trades[i].Receiver == u.Email) && (trades[i].Status == TradeStatus.Accepted || trades[i].Status == TradeStatus.Completed || trades[i].Status == TradeStatus.Denied))
      {
        found = true;
        Console.WriteLine($"Byte mellan: {trades[i].Sender} och: {trades[i].Receiver}: {trades[i].OfferedItem} mot: {trades[i].RequestedItem}  | Status: {trades[i].Status}");
      }
    }
    if (found == false)
    {
      Console.WriteLine("Du har inga avklarade byten");
    }
  }

  public void TradeAccepted(Trade trade)
  {
    Item offereditem = null;
    foreach (Item item in items)
    {
      if (item.Name == trade.OfferedItem && item.Owner == trade.Sender)
      {
        offereditem = item;
        break;
      }
    }

    Item requesteditem = null;
    foreach (Item item in items)
    {
      if (item.Name == trade.RequestedItem && item.Owner == trade.Receiver)
      {
        requesteditem = item;
        break;
      }
    }
    if (offereditem == null || requesteditem == null)
    {
      Console.WriteLine("Kunde inte hitta items");
      return;
    }

    string temporaryOwner = offereditem.Owner;

    offereditem.Owner = requesteditem.Owner;
    requesteditem.Owner = temporaryOwner;

    trade.Status = TradeStatus.Completed;

    List<string> itemRows = new List<string>();
    foreach (Item item in items)
    {
      string row = item.Name + "|" + item.Description + "|" + item.Owner;
      itemRows.Add(row);
    }
    File.WriteAllLines("items.csv", itemRows);

    List<string> tradeRows = new List<string>();
    foreach (Trade currentTrade in trades)
    {
      string row = currentTrade.Sender + "|" + currentTrade.Receiver + "|" + currentTrade.RequestedItem + "|" + currentTrade.OfferedItem + "|" + currentTrade.Status;
      tradeRows.Add(row);
    }
    File.WriteAllLines("trades.csv", tradeRows);
  }
}