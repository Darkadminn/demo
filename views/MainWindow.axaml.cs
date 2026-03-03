using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace obyv010;

public partial class MainWindow : Window
{
    DB dB = new DB();
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void ButtonClickSignUp(object? sender, RoutedEventArgs e)
    {
        if(string.IsNullOrWhiteSpace(Login.Text) || string.IsNullOrWhiteSpace(Password.Text))
        {
            Messages.Text = "Заполните все поля";
            Messages.IsVisible = true;
            await Task.Delay(1000);
            Messages.IsVisible = false;
        }
        else
        {
            try
            {
                dB.Authorization(Login.Text, Password.Text);

                if(UserAuthorization.id <= 0)
                {
                    Messages.Text = "Неверный логин или пароль";
                    Messages.IsVisible = true;
                    await Task.Delay(1000);
                    Messages.IsVisible = false;
                }
                else
                {
                    if(UserAuthorization.role == "Авторизированный клиент")
                    {
                        var window = new ClientMenu();
                        window.Show();
                        this.Close();
                    }
                    else if(UserAuthorization.role == "Менеджер")
                    {
                        var window = new ManagerMenu();
                        window.Show();
                        this.Close();
                    }
                    else
                    {
                        var window = new AdminMenu();
                        window.Show();
                        this.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                Messages.Text = $"Ошибка: {ex.Message}";
                Messages.IsVisible = true;
                await Task.Delay(1000);
                Messages.IsVisible = false; 
            }
        }
    }

    private void ButtonClickSignUpGuest(object? sender, RoutedEventArgs e)
    {
        var window = new GuestClientListProduct();
        window.Show();
        this.Close();
    }


    private void ButtonClickExit(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

}