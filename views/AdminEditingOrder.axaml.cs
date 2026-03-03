using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace obyv010;

public partial class AdminEditingOrder : Window
{
    DB dB = new DB();
    Order order0;
    List<OrderDetail> orderDetails;
    List<OrderDetail> oldOrderDetails;
    List<Product> listProducts;
    public AdminEditingOrder(Order order)
    {
        InitializeComponent();

        oldOrderDetails = dB.GetOrderDetails(order.id);
        orderDetails = new List<OrderDetail>();
        listProducts = dB.GetProducts();

        var statusOrders = dB.GetStatusOrders();
        var pickUpPoints = dB.GetPickUpPoints();

        DateOrder.SelectedDate = order.dateOrder;
        DateDelivery.SelectedDate = order.dateDelivery;
        Article.Text = order.id.ToString();
        Code.Text = order.code;

        Status.ItemsSource = statusOrders;
        PickUpPoints.ItemsSource = pickUpPoints;

        Status.SelectedItem = statusOrders.FirstOrDefault(s => s.id == order.statusId);
        PickUpPoints.SelectedItem = pickUpPoints.FirstOrDefault(p => p.id == order.pickUpPointId);

        Status.DisplayMemberBinding = new Binding("name");
        PickUpPoints.DisplayMemberBinding = new Binding("info");

        DateOrder.IsEnabled = false;
        Article.IsReadOnly = true;

        order0 = order;

        foreach (var detail in oldOrderDetails)
        {
            OrderDetail orderDetail = new OrderDetail
            {
                id = OrderProducts.Children.Count + 1,
                orderId = detail.orderId,
                productId = detail.productId,
                count = detail.count  
            };

            var product = listProducts.FirstOrDefault(p => p.id == detail.productId);
            product.count = product.count + detail.count;

            var stackPanelMain = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                Margin = new Thickness(0,0,0,20)  
            };

            var comboboxProduct = new ComboBox
            {
                FontFamily = "Times New Roman",
                FontSize = 16,
                Width = 170,
                Background = Brush.Parse("#7FFF00"),
                Margin = new Thickness(0,0,10,0)
            };

            var textBoxCount = new TextBox
            {
                FontFamily = "Times New Roman",
                FontSize = 16,
                Width = 50,
                Background = Brush.Parse("#7FFF00"),
                Margin = new Thickness(0,0,10,0)
            };

            var buttonDelete = new Button
            {
                Content = "-",
                FontSize = 16,
                HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Background = Brush.Parse("#00FA9A")
            };

            comboboxProduct.ItemsSource = listProducts;

            comboboxProduct.SelectedItem = listProducts.FirstOrDefault(p => p.id == orderDetail.productId);

            comboboxProduct.DisplayMemberBinding = new Binding("info");

            comboboxProduct.SelectionChanged += (a, args) =>
            {
                if(comboboxProduct.SelectedItem != null)
                {
                    var selectedProduct = comboboxProduct.SelectedItem as Product;
                    orderDetail.productId = selectedProduct.id;
                }  
            };

            textBoxCount.Text = $"{orderDetail.count}";

            textBoxCount.LostFocus += async (a, args) =>
            {
                try
                {
                    orderDetail.count = int.Parse(textBoxCount.Text);
                }
                catch
                {
                    Messages.Text = "Количество должно быть целым неотрицательным числом";
                    Messages.IsVisible = true;
                    await Task.Delay(1000);
                    Messages.IsVisible = false;

                    textBoxCount.Text = "1";
                }
            };

            buttonDelete.Click += (a,args) =>
            {
                OrderProducts.Children.Remove(stackPanelMain);
                orderDetails.Remove(orderDetail);  
            };


            stackPanelMain.Children.Add(comboboxProduct);
            stackPanelMain.Children.Add(textBoxCount);
            stackPanelMain.Children.Add(buttonDelete);

            OrderProducts.Children.Add(stackPanelMain);

            orderDetails.Add(orderDetail);
        }
    }

    private void ButtonClickBack(object? sender, RoutedEventArgs e)
    {
        
        var window = new AdminListOrder();
        window.Show();
        this.Close();
                
    }

    private async void ButtonClickAddProduct(object? sender, RoutedEventArgs e)
    {
        int id0 = OrderProducts.Children.Count() + 1;

        OrderDetail product = new OrderDetail
        {
            id = id0,
            count = 1  
        };

        orderDetails.Add(product);
        
        var stackPanelMain = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Margin = new Thickness(0,0,0,20)  
        };

        var comboboxProduct = new ComboBox
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            Width = 170,
            Background = Brush.Parse("#7FFF00"),
            Margin = new Thickness(0,0,10,0)
        };

        var textBoxCount = new TextBox
        {
            FontFamily = "Times New Roman",
            FontSize = 16,
            Width = 50,
            Background = Brush.Parse("#7FFF00"),
            Margin = new Thickness(0,0,10,0)
        };

        var buttonDelete = new Button
        {
            Content = "-",
            FontSize = 16,
            HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Background = Brush.Parse("#00FA9A")
        };

        listProducts = dB.GetProducts().Where(p => p.count > 0 
        && orderDetails.Where(o => o.productId == p.id).Count() == 0)
        .ToList();

        comboboxProduct.ItemsSource = listProducts;

        comboboxProduct.DisplayMemberBinding = new Binding("info");

        comboboxProduct.SelectionChanged += (a, args) =>
        {
            if(comboboxProduct.SelectedItem != null)
            {
                var selectedProduct = comboboxProduct.SelectedItem as Product;
                product.productId = selectedProduct.id;
            }  
        };

        textBoxCount.Text = "1";

        textBoxCount.LostFocus += async (a, args) =>
        {
            try
            {
                product.count = int.Parse(textBoxCount.Text);
            }
            catch
            {
                Messages.Text = "Количество должно быть целым неотрицательным числом";
                Messages.IsVisible = true;
                await Task.Delay(1000);
                Messages.IsVisible = false;

                textBoxCount.Text = "1";
            }
        };

        buttonDelete.Click += (a,args) =>
        {
            OrderProducts.Children.Remove(stackPanelMain);
            orderDetails.Remove(product);  
        };


        stackPanelMain.Children.Add(comboboxProduct);
        stackPanelMain.Children.Add(textBoxCount);
        stackPanelMain.Children.Add(buttonDelete);

        OrderProducts.Children.Add(stackPanelMain);
                
    }

    private async void ButtonClickDelete(object? sender, RoutedEventArgs e)
    {

        try
        {
            dB.DeleteOrder(order0.id);
            
            Messages.Text = $"Заказ успешно удален";
            Messages.IsVisible = true;
            await Task.Delay(1000);
            Messages.IsVisible = false;

            var window = new AdminListOrder();
            window.Show();
            this.Close();
            
        }
        catch (Exception ex)
        {
            Messages.Text = $"Ошибка: {ex.Message}";
            Messages.IsVisible = true;
            await Task.Delay(1000);
            Messages.IsVisible = false;
        }
                
    }

    private async void ButtonClickSave(object? sender, RoutedEventArgs e)
    {
        
        if(string.IsNullOrWhiteSpace(Article.Text) || Status.SelectedItem == null || PickUpPoints.SelectedItem == null
            || DateOrder.SelectedDate == null || DateDelivery.SelectedDate == null || string.IsNullOrWhiteSpace(Code.Text))
        {
            Messages.Text = "Заполните все обязательные поля";
            Messages.IsVisible = true;
            await Task.Delay(1000);
            Messages.IsVisible = false;
        }
        else
        {
            if(int.TryParse(Article.Text, out int artic))
            {
                if(artic <= 0)
                {
                    Messages.Text = "Артикул должен быть целым неотрицательным числом";
                    Messages.IsVisible = true;
                    await Task.Delay(1000);
                    Messages.IsVisible = false;
                    return;
                }
            }
            else
            {
                Messages.Text = "Артикул должен быть целым неотрицательным числом";
                Messages.IsVisible = true;
                await Task.Delay(1000);
                Messages.IsVisible = false;
                return;
            }

            if(DateOrder.SelectedDate > DateDelivery.SelectedDate)
            {
                Messages.Text = "Дата заказа не может быть больше даты доставки";
                Messages.IsVisible = true;
                await Task.Delay(1000);
                Messages.IsVisible = false;
                return;
            }

            try
            {
                var status = Status.SelectedItem as StatusOrder;
                var pickUpPoint = PickUpPoints.SelectedItem as PickUpPoint;

                Order order = new Order
                {
                    id = int.Parse(Article.Text),
                    dateOrder = (DateTime)DateOrder.SelectedDate,
                    dateDelivery = (DateTime)DateDelivery.SelectedDate,
                    code = Code.Text,
                    statusId = status.id,
                    pickUpPointId = pickUpPoint.id
                };

                foreach(var detail in orderDetails)
                {
                    Product product = listProducts.FirstOrDefault(p => p.id == detail.productId);

                    if(product == null)
                    {
                        Messages.Text = "Выберите товары";
                        Messages.IsVisible = true;
                        await Task.Delay(1000);
                        Messages.IsVisible = false;

                        return;
                    
                    }
                    else
                    {
                        if(product.count < detail.count)
                        {
                            Messages.Text = $"Товаров {product.info} доступно только {product.count}";
                            Messages.IsVisible = true;
                            await Task.Delay(1000);
                            Messages.IsVisible = false;

                            return;
                        }
                    }

                    detail.orderId = int.Parse(Article.Text);
                }

                string result = dB.UpdateOrder(order, orderDetails, oldOrderDetails);

                if(result == "-1")
                {
                    Messages.Text = "Ошибка при выполнении операции";
                    Messages.IsVisible = true;
                    await Task.Delay(1000);
                    Messages.IsVisible = false;
                }
                else if(result == "-3")
                {
                    Messages.Text = "Недостаточно товаров";
                    Messages.IsVisible = true;
                    await Task.Delay(1000);
                    Messages.IsVisible = false;
                }
                else
                {
                    Messages.Text = "Данные успешно сохранены";
                    Messages.IsVisible = true;
                    await Task.Delay(1000);
                    Messages.IsVisible = false;
                }

                 
                
            }
            catch (Exception ex)
            {
                Messages.Text = $"Ошибка: {ex.Message}";
                Messages.IsVisible = true;
                await Task.Delay(1000);
                Messages.IsVisible = false;
            }
        }
                
    }
}