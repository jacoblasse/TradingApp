namespace App;

public class Trade
{
  public string Sender;
  public string Receiver;
  public string RequestedItem;
  public string OfferedItem;
  public TradeStatus Status;

  public Trade(string sender, string receiver, string requesteditem, string offereditem, TradeStatus status)
  {
    Sender = sender;
    Receiver = receiver;
    RequestedItem = requesteditem;
    OfferedItem = offereditem;
    Status = status;
  }
}