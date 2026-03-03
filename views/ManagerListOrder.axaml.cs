using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace obyv010;

public partial class ManagerListOrder : Window
{
    DB dB = new DB();
    public ManagerListOrder()
    {
        InitializeComponent();

        LoadOrders();
    }

    private void ButtonClickBack(object? sender, RoutedEventArgs e)
    {
        
        var window = new ManagerMenu();
        window.Show();
        this.Close();
                
    }

    private void LoadOrders()
    {
        Orders.Children.Clear();

        var orders = dB.GetOrders();

        foreach(var order in orders)
        {
            var stackPanelMain = new StackPanel
            {
                Width = 550,
                Height = 110,
                Margin = new Thickness(0,0,0,10),
                Orientation = Avalonia.Layout.Orientation.Horizontal  
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

            var orderId = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"Артикул заказа: {order.id}",
                FontStyle = Avalonia.Media.FontStyle.Oblique
            };

            var orderStatus = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"Статус заказа: {order.status}"
            };

            var orderPickUpPoint = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"Адрес пункта выдачи: {order.pickUpPoint}"
            };

            var orderDateOrder = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"Дата заказа: {order.dateOrder.Date}"
            };

            var orderDateDelivery = new TextBlock
            {
                FontSize = 11,
                FontFamily = "Times New Roman",
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Text = $"{order.dateDelivery.Date}"
            };


            stackPanelMiddle.Children.Add(orderId);
            stackPanelMiddle.Children.Add(orderStatus);
            stackPanelMiddle.Children.Add(orderPickUpPoint);
            stackPanelMiddle.Children.Add(orderDateOrder);

            stackPanelRight.Children.Add(orderDateDelivery);

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

            borderMiddle.Child = stackPanelMiddle;
            borderRight.Child = stackPanelRight;

            stackPanelMain.Children.Add(borderMiddle);
            stackPanelMain.Children.Add(borderRight);

            borderMain.Child = stackPanelMain;

            Orders.Children.Add(borderMain);

        }
    }
}