namespace App;

public class Trade
{
  public string Sender;
  public string RequestedItem;
  public string OfferedItem;
  public TradeStatus Status;

  public Trade(string sender, string requesteditem, string offereditem, TradeStatus status)
  {
    Sender = sender;
    RequestedItem = requesteditem;
    OfferedItem = offereditem;
    Status = status;
  }
}