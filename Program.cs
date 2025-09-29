using App;

//A user needs to be able to register an account
//A user needs to be able to log out.
//A user needs to be able to log in.
//A user needs to be able to upload information about the item they wish to trade.
//A user needs to be able to browse a list of other users items.
//A user needs to be able to request a trade for other users items.
//A user needs to be able to browse trade requests.
//A user needs to be able to accept a trade request.
//A user needs to be able to deny a trade request.
//A user needs to be able to browse completed requests.


TradeSystem user = new TradeSystem();

bool running = true;

while (running)
{
  //Skapar variabel med en interface typ för att spara inloggnings information.
  IUser? active_user;

  //Meny val 
  Console.Write("1. Logga in\n2. Glömt lösenord?\n3. Skapa en konto\n4. Avsluta\n");

  //Skapar switch:case för att användaren ska kunna använda menyn.
  switch (Console.ReadLine())
  {

    case "1":
      //Kallar på Login() en funktion som loggar in användaren. Ifall användaren loggar in så sparas användarens information i active_user.

      active_user = user.Login();

      while (active_user != null)
      {
        Console.Clear();
        Console.WriteLine("1. Se mina grejer\n2. Kolla items\n 3. Se trade requests\n4. Logga ut\n5. Avsluta");
        switch (Console.ReadLine())
        {
          case "1":

            break;

          case "4":
            active_user = null;

            break;

          case "5":
            active_user = null;
            running = false;
            break;
        }
      }
      break;

    case "2":

      break;

    case "3":
      running = false;
      break;
  }
}
