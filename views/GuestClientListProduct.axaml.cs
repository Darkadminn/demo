using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace obyv010;

public partial class GuestClientListProduct : Window
{
    DB dB = new DB();
    public GuestClientListProduct()
    {
        InitializeComponent();

        LoadProducs();
    }

    private void ButtonClickBack(object? sender, RoutedEventArgs e)
    {
        if(UserAuthorization.role == "Авторизированный клиент")
        {
            var window = new ClientMenu();
            window.Show();
            this.Close();
        }
        else
        {
            var window = new MainWindow();
            window.Show();
            this.Close();
        }
        
    }

    private void LoadProducs()
    {
        Products.Children.Clear();

        var products = dB.GetProducts();

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