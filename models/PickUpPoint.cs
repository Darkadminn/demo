using System;

namespace obyv010;

public class PickUpPoint
{
    public int id {get; set;}
    public string code {get; set;}
    public string city {get; set;}
    public string street {get; set;}
    public string home {get; set;}
    public string info => $"{code} г. {city} ул. {street} {home}";
}
