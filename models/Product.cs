using System;

namespace obyv010;

public class Product
{
    public int id {get; set;}
    public string article {get; set;}
    public int titleProductId {get; set;}
    public string titleProduct {get; set;}
    public string unit {get; set;}
    public double price {get; set;}
    public int supplierId {get; set;}
    public string supplier {get; set;}
    public int manufactureId {get; set;}
    public string manufacture {get; set;}
    public int categoryId {get; set;}
    public string category {get; set;}
    public int discount {get; set;}
    public int count {get; set;}
    public string description {get; set;}
    public string image {get; set;}

    public string info => $"{article} {titleProduct}";
}
