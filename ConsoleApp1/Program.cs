using System;
using System.Collections.Generic;

interface IPay
{
    decimal ValueToPay();
}

abstract class Product : IPay
{
    public int Id { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public float Tax { get; set; }

    public abstract decimal ValueToPay();

    public override string ToString()
    {
        return $"{Id} {Description} Price: {Price:C2} Tax: {Tax:P2}";
    }
}

class FixedPriceProduct : Product
{
    public override decimal ValueToPay()
    {
        return Price * (1 + (decimal)Tax);
    }
}

class VariablePriceProduct : Product
{
    public string Measurement { get; set; }
    public float Quantity { get; set; }

    public override decimal ValueToPay()
    {
        return (Price * (decimal)Quantity) * (1 + (decimal)Tax);
    }

    public override string ToString()
    {
        return $"{base.ToString()} Measurement: {Measurement} Quantity: {Quantity:N2} Value to pay: {ValueToPay():C2}";
    }
}

class ComposedProduct : Product
{
    public List<Product> Products { get; set; }
    public float Discount { get; set; }

    public override decimal ValueToPay()
    {
        decimal total = 0;
        foreach (var product in Products)
        {
            total += product.ValueToPay();
        }
        return total * (1 - (decimal)Discount);
    }

    public override string ToString()
    {
        return $"{base.ToString()} Discount: {Discount:P2} Value to pay: {ValueToPay():C2}";
    }
}

class Invoice : IPay
{
    private List<Product> _products = new List<Product>();

    public void AddProduct(Product product)
    {
        _products.Add(product);
    }

    public decimal ValueToPay()
    {
        decimal total = 0;
        foreach (var product in _products)
        {
            total += product.ValueToPay();
        }
        return total;
    }

    public override string ToString()
    {
        string invoiceDetails = "INVOICE\n------------------------------\n";
        foreach (var product in _products)
        {
            invoiceDetails += product + "\n";
        }
        invoiceDetails += $"TOTAL: {ValueToPay():C2}";
        return invoiceDetails;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("PRODUCTS");
        Console.WriteLine("-------------------------------------------------");

        Product product1 = new FixedPriceProduct() { Description = "Vino Gato Negro", Id = 1010, Price = 46000M, Tax = 0.19F };
        Product product2 = new FixedPriceProduct() { Description = "Pan Bimbo Artesanal", Id = 2020, Price = 1560M, Tax = 0.19F };
        Product product3 = new VariablePriceProduct() { Description = "Queso Holandes", Id = 3030, Measurement = "Kilo", Price = 32000M, Quantity = 0.536F, Tax = 0.19F };
        Product product4 = new VariablePriceProduct() { Description = "Cabano", Id = 4040, Measurement = "Kilo", Price = 18000M, Quantity = 0.389F, Tax = 0.19F };
        Product product5 = new ComposedProduct() { Description = "Ancheta #1", Discount = 0.12F, Id = 5050, Products = new List<Product>() { product1, product2, product3, product4 } };

        Console.WriteLine(product1);
        Console.WriteLine(product2);
        Console.WriteLine(product3);
        Console.WriteLine(product4);
        Console.WriteLine(product5);

        Invoice invoice = new Invoice();
        invoice.AddProduct(product1);
        invoice.AddProduct(product3);
        invoice.AddProduct(product5);
        Console.WriteLine(invoice);
    }
}
