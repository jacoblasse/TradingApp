namespace App;

public class Trade
{
  public string Sender;
  public string Receiver;
  public string Status;
  public string Items;

  public Trade(string sender, string receiver, string status, string items)
  {
    Sender = sender;
    Receiver = receiver;
    Status = status;
    Items = items;
  }
}