using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace obyv010;

public partial class ManagerListProduct : Window
{
    DB dB = new DB();
    public ManagerListProduct()
    {
        InitializeComponent();

        var suppliers = dB.GetSuppliers();

        suppliers.Insert(0, new Supplier
        {
           id = 0,
           name = "Все поставщики" 
        });

        Filter.ItemsSource = suppliers;
        Filter.DisplayMemberBinding = new Binding("name");

        Search.TextChanging += (s, args) =>
        {
            LoadProducs();
        };

        Filter.SelectionChanged += (s, args) =>
        {
            LoadProducs();
        };

        Sort.SelectionChanged += (s, args) =>
        {
            LoadProducs();
        };

        LoadProducs();
    }

    private void ButtonClickBack(object? sender, RoutedEventArgs e)
    {
        
        var window = new ManagerMenu();
        window.Show();
        this.Close();
                
    }

    private void UpdateListProduct(object? sender, RoutedEventArgs e)
    {      
        LoadProducs();             
    }

    private void LoadProducs()
    {
        Products.Children.Clear();

        var products = dB.GetProducts();

        if (!string.IsNullOrWhiteSpace(Search.Text))
        {
            products = products.Where(p => p.article.ToLower().Contains(Search.Text.ToLower()) || p.titleProduct.ToLower().Contains(Search.Text.ToLower())
                                    || p.supplier.ToLower().Contains(Search.Text.ToLower()) || p.manufacture.ToLower().Contains(Search.Text.ToLower())
                                    || p.category.ToLower().Contains(Search.Text.ToLower()) || p.description.ToLower().Contains(Search.Text.ToLower())).ToList();
        }
        

        if(Filter.SelectedItem != null)
        {
            var filter = Filter.SelectedItem as Supplier;

            if(filter.id != 0)
            {
                products = products.Where(p => p.supplierId == filter.id).ToList();
            }
           
        }

        if(Sort.SelectedIndex != -1)
        {
            if(Sort.SelectedIndex == 1)
            {
                products = products.OrderBy(p => p.count).ToList();
            }
            else if(Sort.SelectedIndex == 2)
            {
                products = products.OrderByDescending(p => p.count).ToList();
            }
        }

        foreach(var product in products)
        {
            var stackPanelMain = new StackPanel
            {
                Width = 550,
                Height = 110,
                Margin = new Thickness(0,0,0,10),
                Orientation = Avalonia.Layout.Orientation.Horizontal  
            };

            var image = new Image
            {
                Width = 80,
                Height = 80,
                Margin = new Thickness(5),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            var stackPanelMiddle = new StackPanel
            {
                Width = 400,
                Orientation = Avalonia.Layout.Orientation.Vertical,
                Margin = new Thickness(5),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center  
            };

            var stackPanelRight = new StackPanel
            {
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Margin = new Thickness(5)
            };

            try
            {
                var bitmap = new Bitmap($"images/{product.image}");
                image.Source = bitmap;
            }
            catch
            {
                var bitmap = new Bitmap($"images/picture.png");
                image.Source = bitmap;
            }

            var productName = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"{product.category} | {product.titleProduct}"
            };

            var productDescription = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"Описание товара: {product.description}"
            };

            var productManufacture = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"Производитель: {product.manufacture}"
            };

            var productSupplier = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"Поставщик: {product.supplier}"
            };

            var productPrice = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };

            var productOldPrice = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };

            var productNewPrice = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            };

            var stackPanelPrice = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal  
            };

            var productUnit = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"Единица измерения: {product.unit}"
            };

            var productCount = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"Количество на складе: {product.count}"
            };

            var productDiscount = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"{product.discount}%"
            };


            var borderMain = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };

            var borderMiddle = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };

            var borderRight = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                Margin = new Thickness(5)
            };


            if(product.count == 0)
            {
                borderMiddle.Background = Brushes.AliceBlue;
            }
            else if(product.discount > 15)
            {
                borderMiddle.Background = Brush.Parse("#2E8B57");
            }

            if(product.discount > 0)
            {
                productPrice.Text = "Цена: ";

                productOldPrice.Text = $"{product.price}";
                productOldPrice.TextDecorations = TextDecorations.Strikethrough;
                productOldPrice.Foreground = Brushes.Red;

                productNewPrice.Text = $" {Math.Round(product.price / 100 * (100 - product.discount), 2)}";

                stackPanelPrice.Children.Add(productPrice);
                stackPanelPrice.Children.Add(productOldPrice);
                stackPanelPrice.Children.Add(productNewPrice);
            }
            else
            {
                productPrice.Text = $"Цена: {product.price}";
            }


            stackPanelMiddle.Children.Add(productName);
            stackPanelMiddle.Children.Add(productDescription);
            stackPanelMiddle.Children.Add(productManufacture);
            stackPanelMiddle.Children.Add(productSupplier);
            
            if(stackPanelPrice.Children.Count != 0)
            {
                stackPanelMiddle.Children.Add(stackPanelPrice); 
            }
            else
            {
                stackPanelMiddle.Children.Add(productPrice); 
            }

            stackPanelMiddle.Children.Add(productUnit);
            stackPanelMiddle.Children.Add(productCount);  

            stackPanelRight.Children.Add(productDiscount);          

            borderMiddle.Child = stackPanelMiddle;
            borderRight.Child = stackPanelRight;

            stackPanelMain.Children.Add(image);
            stackPanelMain.Children.Add(borderMiddle);
            stackPanelMain.Children.Add(borderRight);

            borderMain.Child = stackPanelMain;

            

            Products.Children.Add(borderMain);

        }
    }
}