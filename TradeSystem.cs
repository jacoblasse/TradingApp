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


//Konstruktor som läser in data från mina csv filer när programmet körs.
  public TradeSystem()
  {

    //Läser in alla mina användare från users.csv
    string[] users_csv = File.ReadAllLines("users.csv");
    foreach (string user_data in users_csv)
    {
      //Delar informationen i varje rad och sepererar med "|"
      string[] split_user_data = user_data.Split("|");
      //Kollar ifall det finns tillräckligt med information i raden, (Useername, Email, _Password) innan den lägger till det i listan users.
      if (split_user_data.Length >= 3)
      {
        users.Add(new User(split_user_data[0], split_user_data[1], split_user_data[2]));
      }
    }

    //Läser in alla items från items.csv
    string[] items_csv = File.ReadAllLines("items.csv");
    foreach (string item_data in items_csv)
    {
      //Delar informationen i varje rad och sepererar med "|"
      string[] split_item_data = item_data.Split("|");
      //Kollar ifall det finns tillräckligt med information i raden, (Name, Description, Owner) innan den lägger till det i listan items.
      if (split_item_data.Length >= 3)
      {
        items.Add(new Item(split_item_data[0], split_item_data[1], split_item_data[2]));
      }
    }

    //Läser in alla items från trades.csv
    string[] trades_csv = File.ReadAllLines("trades.csv");
    foreach (string trade_data in trades_csv)
    {
       //Delar informationen i varje rad och sepererar med "|"
      string[] split_trade_data = trade_data.Split("|");
      //Kollar ifall det finns tillräckligt med information i raden, (Sender, Receiver, RequestedItem, OfferedItem, Status) innan den lägger till det i listan trades.
      if (split_trade_data.Length >= 5)
      {
        TradeStatus status;
        //Försöker läsa in status från string till enum TradeStatus
        if (!Enum.TryParse(split_trade_data[4], out status))
        {
          // Om status inte kan läsas, sätt till Pending
          status = TradeStatus.Pending;
        }
        //Lägger till trade i listan trades
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
        
        if (user.TryLogin(username, password))
          return user;
      }
    }
    //ifall informationen inte matchar så skickar den tillbaka null, alltså ingenting.
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
    //Kollar ifall aktiva användaren är en User och skapar variabel med information från klassen User i User.cs
    if (active_user is User u)
    {
      //Ber användaren att skriva in namnet på sitt item och sparar det i variablen
      Console.Write("Namn på ditt Item: ");
      name = Console.ReadLine();

      //Ber användaren att skriva in en beskrivning på sitt item och sparar det i variablen
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

    //Skapar en lista "otheritems" som sparar alla items förutom den aktiva användarens items.
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
    //Skickar tillbaka listan otheritems till vart funktionen blev kallad ifrån.
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
        //Lägger till +1 varje gång den hittar ett item som tillhör den aktiva användaren.
        i += 1;
        Console.WriteLine($"[{i}] - {item.Name} - {item.Description}");
      }
    }
    //Kollar ifall användaren inte har några items, och ifall den inte har några så skriver den ut meddalandet och skickar tillbaka användaren till vart funktionen blev kallad ifrån.
    if (i == 0)
    {
      Console.WriteLine("Du har inga items");
      return;
    }
    //Kollar bara ifall användaren har 1 item eller flera för att skriva item eller items.
    if (i < 2)
    {
      Console.WriteLine($"Du har {i} item i ditt förråd");
    }
    else
    {
      Console.WriteLine($"Du har {i} items i ditt förråd");
    }

  }


  //Funktion för att skicka trade förfrågan.
  public void MakeTrade(IUser? active_user)
  {
    //Hämtar lista med andra användares items genom att kalla på ShowItems funktionen.
    List<Item> otheritems = ShowItems(active_user);
    //Gör en console clear för att inte visa alla items igen när användaren ska välja ett item att byta mot.
    Console.Clear();

    //Kollar ifall användaren är inloggad och är en User och skapar variabel u med information från User klassen i User.cs.
    if (active_user is not User u)
    {
      Console.WriteLine("Du kan inte byta items för att du är inte en användare.");
      return;
    }

    //Kollar ifall det finns items att byta mot.
    if (otheritems == null)
    {
      Console.WriteLine("Det finns inga items att byta");
      return;
    }
    //Visar tillgängliga items att byta mot.
    Console.WriteLine("Välj ett item att byta till");
    for (int i = 0; i < otheritems.Count; i++)
    {
      Console.WriteLine($"[{i + 1}] {otheritems[i].Name} - {otheritems[i].Description} (Ägare: {otheritems[i].Owner})");
    }

    //Läser användarens val av item att byta mot.
    string input = Console.ReadLine();
    int itemChoice;
    //Kollar ifall användarens val är godkänt och sparar det i itemChoice variabelen.
    //Ifall det inte är godkänt så skickar den tillbaka användaren till vart funktionen blev kallad ifrån.
    if (!int.TryParse(input, out itemChoice) || itemChoice < 1 || itemChoice > otheritems.Count)
    {
      Console.WriteLine("Ogiltigt val.");
      return;
    }

    //Sparar det valda itemet i chosenItem variabelen.
    Item chosenItem = otheritems[itemChoice - 1];

    //Skapar en ny lista som sparar alla items av den aktiva användaren.
    List<Item> myItems = new List<Item>();
    foreach (Item item in items)
      if (item.Owner == u.Email)
      {
        myItems.Add(item);
      }

    //Kollar ifall den aktiva användaren har några items att byta med.
    if (myItems.Count == 0)
    {
      Console.WriteLine("Du har inga items att byta");
      return;
    }

    //Visar den aktiva användarens items som dom kan erbjuda i bytet.
    Console.WriteLine("Välj ett av dina items att erbjuda i bytet: ");
    for (int i = 0; i < myItems.Count; i++)
    {
      Console.WriteLine($"[{i + 1}] {myItems[i].Name} - {myItems[i].Description}");
    }

    //Läser vilket av sina items avnvändaren har valt att erbjuda i bytet.
    input = Console.ReadLine();
    int myItemChoice;
    //Kontrollerar ifall användarens val är godkänt och sparar det i myItemChoice variabelen.
    //Ifall det inte är godkänt så skickar den tillbaka användaren till vart funktionen blev kallad ifrån.
    if (!int.TryParse(input, out myItemChoice) || myItemChoice < 1 || myItemChoice > myItems.Count)
    {
      Console.WriteLine("Ogiltigt val");
      return;
    }

    //Sparar det valda itemet i offeredItem variabelen.
    Item offeredItem = myItems[myItemChoice - 1];

    //Skapar ett nytt trade request med informationen från variablerna ovan och sparar det i listan trades.
    trades.Add(new Trade
    (
      u.Email,
      chosenItem.Owner,
      chosenItem.Name,
      offeredItem.Name,
      TradeStatus.Pending
    ));

    //Sparar samma information i trades.csv filen så att information är sparad även efter programmet stängs av.
    //Skapar en array med en rad som innehåller all information som behövs för ett trade request.
    File.AppendAllLines("trades.csv", new[] { $"{u.Email}|{chosenItem.Owner}|{chosenItem.Name}|{offeredItem.Name}|{TradeStatus.Pending}" });

    Console.WriteLine("Trade-förfrågan har skickats");
  }


  //Funktion för att se aktiva trade requests som är skickade till användaren.
  public void ReceivedActiveTrades(IUser? active_user)
  {
    //Kollar så att användaren är inloggad och skapar en variabel u med User information
    if (active_user is not User u)
    {
      Console.WriteLine("Du kan inte ha några aktiva byten för du är inte en inloggad användare");
      return;
    }

    //Skapar en lista som sparar alla trades som är Pending och skickade till användaren
    List<int> tradeIndex = new List<int>();

    //Gör en for loop som kollar igenom alla trades i listan trades
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

    //Visar alla aktiva trades som är skickade till användaren.
    for (int i = 0; i < tradeIndex.Count; i++)
    {
      int tradeidx = tradeIndex[i];
      Console.WriteLine($"[{i + 1}] - {trades[tradeidx].Sender} vill byta sitt item: {trades[tradeidx].OfferedItem} mot ditt item: {trades[tradeidx].RequestedItem}");
    }

    //Ber användaren att välja en av sina aktiva trades för att kunna acceptera eller avböja, eller tryck enter för att gå tillbaka.
    Console.WriteLine("Välj en av dina aktiva trades för att kunna acceptera eller avböja, Eller tryck enter för att gå tillbaka");
    string input = Console.ReadLine();

    //Kollar ifall användaren tryckte enter för att gå tillbaka.
    if (input == null || input == "")
    {
      return;
    }
    //Kollar ifall användarens val är godkänt och sparar det i choice variabelen.
    //Ifall det inte är godkänt så skickar den tillbaka användaren till vart funktionen blev kallad ifrån.
    if (!int.TryParse(input, out int choice) || choice < 1 || choice > tradeIndex.Count)
    {
      Console.WriteLine("fel val");
      return;
    }

    //Sparar det valda trade requestet i selectedTradeIndex variabelen.
    int selectedTradeIndex = tradeIndex[choice - 1];

    //Ber användaren att acceptera eller avböja bytet, eller tryck enter för att gå tillbaka.
    Console.WriteLine("Vill du acceptera bytet? Skriv (ja) för att acceptera, (nej) för att avböja, eller tryck enter för att gå tillbaka");
    while (true)
    {
      string answer = Console.ReadLine().ToLower();
      //Accepterar bytet.
      if (answer == "ja")
      {
        //Ändrar statusen på bytet till Accepted
        trades[selectedTradeIndex].Status = TradeStatus.Accepted;

        //Skapar en lista med alla trades för att kunna skriva över hela filen med den nya informationen.
        //Annars skulle bara den accepterade tradens status ändras i filen och alla andra trades skulle försvinna.
        List<string> tradeRows = new List<string>();
        foreach (Trade currentTrade in trades)
        {
          string row = currentTrade.Sender + "|" + currentTrade.Receiver + "|" + currentTrade.RequestedItem + "|" + currentTrade.OfferedItem + "|" + currentTrade.Status;
          tradeRows.Add(row);
        }
        File.WriteAllLines("trades.csv", tradeRows);



        //Kallar på TradeAccepted funktionen som byter ägaren av itemsen.
        TradeAccepted(trades[selectedTradeIndex]);
        Console.WriteLine("Byte accepterat");
        break;
      }
      //Avböjer bytet.
      else if (answer == "nej")
      {
        trades[selectedTradeIndex].Status = TradeStatus.Denied;
        Console.WriteLine("Byte avböjt");
        break;
      }
      //Skickar tillbaka användaren till vart funktionen blev kallad ifrån.
      else if (answer == null || answer == "")
      {
        return;
      }
    }

  }

  //Funktion för att se aktiva trade requests som användaren har skickat till någon annan, och kunna avbryta dom.
  public void OfferedActiveTrades(IUser? active_user)
  {
    //Kollar så att användaren är inloggad och skapar en variabel u med User information.
    if (active_user is not User u)
    {
      Console.WriteLine("Du kan inte ha några aktiva byten för du är inte en inloggad användare");
      return;
    }

    //Skapa lista med index för trades som användaren har skickat till någon och är pending
    List<int> OfferedTradeIndex = new List<int>();

    //Gör en for loop som kollar igenom alla trades i listan trades
    for (int i = 0; i < trades.Count; i++)
    {
      //Kollar efter trades som har Pending status och är skickade av användaren och lägger till dom i listan.
      if (trades[i].Status == TradeStatus.Pending && u.Email == trades[i].Sender)
      {
        OfferedTradeIndex.Add(i);
      }

    }

    //Kollar ifall det finns några trades som sparades i listan
    //Ifall det inte finns några trades så skickas an användaren tillbaka till vart funktionen blev kallad ifrån.
    if (OfferedTradeIndex.Count == 0)
    {
      Console.WriteLine("Det finns inga trades som du har skickat som är aktiva just nu");
      return;
    }

    //Visar alla aktiva trades som användaren har skickat till någon annan.
    for (int i = 0; i < OfferedTradeIndex.Count; i++)
    {
      int tradeidx = OfferedTradeIndex[i];
      Console.WriteLine($"[{i + 1}] - Du vill byta ditt item: {trades[tradeidx].OfferedItem} mot {trades[tradeidx].Receiver} item: {trades[tradeidx].RequestedItem}");
    }

    //Ber användaren att välja en av sina aktiva trades för att kunna avbryta, eller tryck enter för att gå tillbaka.
    Console.WriteLine("Vill du avbryta ett av dina trade offers?");
    Console.WriteLine("Välj isåfall indexet av det bytet du vill avbryta, eller tryck Enter för att gå tillbaka");


    //Läser användarens val
    string input = Console.ReadLine();
    //Kollar ifall användaren tryckte enter för att gå tillbaka.
    if (input == null || input == "")
    {
      return;
    }

    //Kollar ifall användarens val är godkänt och sparar det i choice variabelen.
    if (!int.TryParse(input, out int choice) || choice < 1 || choice > OfferedTradeIndex.Count)
    {
      Console.WriteLine("fel val");
      return;
    }

    //Sparar det valda trade requestet i selectedTradeIndex variabelen.
    int selectedTradeIndex = OfferedTradeIndex[choice - 1];


    Console.WriteLine("Om du är säker på att du vill avbryta bytet skriv (ja), om du inte vill skriv (nej). Du kan alltid gå tillbaka genom att trycka Enter.");

    //Startar en loop som kollar användarens svar.
    while (true)
    {
      string confirm = Console.ReadLine().ToLower();
      //Avbryter traden.
      if (confirm == "ja")
      {
        trades[selectedTradeIndex].Status = TradeStatus.Canceled;

        //Skapar en lista med alla trades för att kunna skriva över hela filen med den nya informationen.
        //Annars skulle bara den avbrutna tradens status ändras i filen och alla andra trades skulle försvinna.
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
      //avbryter inte traden.
      else if (confirm == "nej")
      {
        Console.WriteLine("Byte inte avbrutet");
        break;
      }
      //Skickar tillbaka användaren till vart funktionen blev kallad ifrån ifall dom trycker enter.
      else if (confirm == null || confirm == "")
      {
        return;
      }
    }

  }

  //Funktion för att se alla avklarade trades som användaren har varit med i.
  public void CompletedTrades(IUser? active_user)
  {

    //Kollar så att användaren är inloggad och skapar en variabel u med User information.
    if (active_user is not User u)
    {
      Console.WriteLine("Du måste vara inloggad för att se avklarade byten");
      return;
    }

    //Skapar en bool variabel som kollar ifall det finns några avklarade trades att visa.
    bool found = false;

    //Gör en for loop som kollar igenom alla trades i listan trades
    for (int i = 0; i < trades.Count; i++)
    {
      //Kollar ifall den aktiva användaren är antingen avsändare eller mottagare i en trade som har status Accepted, Completed eller Denied.
      //Ifall det stämmer så visar den informationen om traden.
      if ((trades[i].Sender == u.Email || trades[i].Receiver == u.Email) && (trades[i].Status == TradeStatus.Accepted || trades[i].Status == TradeStatus.Completed || trades[i].Status == TradeStatus.Denied))
      {
        found = true;
        Console.WriteLine($"Byte mellan: {trades[i].Sender} och: {trades[i].Receiver}: {trades[i].OfferedItem} mot: {trades[i].RequestedItem}  | Status: {trades[i].Status}");
      }
    }

    //Ifall inga trades hittades så skriver den ut ett meddelande.
    if (found == false)
    {
      Console.WriteLine("Du har inga avklarade byten");
    }
  }


  //Funktion som körs när en trade blir accepterad, den byter ägaren av itemsen och uppdaterar items.csv och trades.csv filerna.
  public void TradeAccepted(Trade trade)
  {
    //Skapar variabler för att spara de items som ska byta ägare.
    Item offereditem = null;
    //Hittar det item som den som skickade traden erbjuder.
    foreach (Item item in items)
    {
      //Kollar så att itemets namn och ägare matchar med informationen i traden och sparar det i offereditem variabelen.
      if (item.Name == trade.OfferedItem && item.Owner == trade.Sender)
      {
        offereditem = item;
        break;
      }
    }

    //Hittar det item som den som mottog traden äger och som den som skickade traden vill ha.
    Item requesteditem = null;
    foreach (Item item in items)
    {
      //Kollar så att itemets namn och ägare matchar med informationen i traden och sparar det i requesteditem variabelen.
      if (item.Name == trade.RequestedItem && item.Owner == trade.Receiver)
      {
        requesteditem = item;
        break;
      }
    }

    //Kollar ifall båda itemsen hittades, ifall inte så skriver den ut ett meddelande och avslutar funktionen.
    if (offereditem == null || requesteditem == null)
    {
      Console.WriteLine("Kunde inte hitta items");
      return;
    }

    //Byter ägare av itemsen genom att spara ägaren av offereditem i en temporär variabel, sedan sätter ägaren av offereditem till ägaren av requesteditem, och slutligen sätter ägaren av requesteditem till den temporära variabelen.
    string temporaryOwner = offereditem.Owner;

    //Byter ägare
    offereditem.Owner = requesteditem.Owner;
    requesteditem.Owner = temporaryOwner;

    //Ändrar statusen på traden till Completed
    trade.Status = TradeStatus.Completed;


    //Uppdaterar items.csv och trades.csv filerna med den nya informationen.
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