using System;

namespace obyv010;

public class Order
{
    public int id {get; set;}
    public DateTime dateOrder{get; set;}
    public DateTime dateDelivery{get; set;}
    public int pickUpPointId {get; set;}
    public string pickUpPoint {get; set;}
    public int userId {get; set;}
    public string code {get; set;}
    public int statusId {get; set;}
    public string status {get; set;}
}
