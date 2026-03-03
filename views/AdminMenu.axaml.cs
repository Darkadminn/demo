using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace obyv010;

public partial class AdminMenu : Window
{
    public AdminMenu()
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
        var window = new AdminListProduct();
        window.Show();
        this.Close();
    }

    private void ButtonClickOrder(object? sender, RoutedEventArgs e)
    {
        var window = new AdminListOrder();
        window.Show();
        this.Close();
    }
}