using App;

TradeSystem user = new TradeSystem();

bool running = true;

while (running)
{

  Console.Write("1. Logga in\n2. Glömt lösenord?\n3. Quit\n");

  switch (Console.ReadLine())
  {

    case "1":
      user.Login();
      break;

    case "2":

      break;

    case "3":

      break;
  }
}
