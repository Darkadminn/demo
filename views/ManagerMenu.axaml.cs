using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace obyv010;

public partial class ManagerMenu : Window
{
    public ManagerMenu()
    {
        InitializeComponent();
    }

    private void ButtonClickExit(object? sender, RoutedEventArgs e)
    {
        UserAuthorization.id = -1;
        UserAuthorization.role = "";

        var window = new MainWindow();
        window.Show();
        this.Close();
    }

    private void ButtonClickProduct(object? sender, RoutedEventArgs e)
    {
        var window = new ManagerListProduct();
        window.Show();
        this.Close();
    }

    private void ButtonClickOrder(object? sender, RoutedEventArgs e)
    {
        var window = new ManagerListOrder();
        window.Show();
        this.Close();
    }
}