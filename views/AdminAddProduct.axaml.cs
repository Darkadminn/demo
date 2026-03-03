using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace obyv010;

public partial class AdminAddProduct : Window
{
    DB dB = new DB();
    Stream streamImage = null;
    string imagePath = null;
    public AdminAddProduct()
    {
        InitializeComponent();

        Category.ItemsSource = dB.GetCategories();
        Title.ItemsSource = dB.GetTitleProducts();
        ManufactureProduct.ItemsSource = dB.GetManufactures();
        SupplierProduct.ItemsSource = dB.GetSuppliers();

        Category.DisplayMemberBinding = new Binding("name");
        Title.DisplayMemberBinding = new Binding("name");
        ManufactureProduct.DisplayMemberBinding = new Binding("name");
        SupplierProduct.DisplayMemberBinding = new Binding("name");
    }

    private void ButtonClickBack(object? sender, RoutedEventArgs e)
    {
        
        var window = new AdminListProduct();
        window.Show();
        this.Close();
                
    }

    private async void ButtonClickAdd(object? sender, RoutedEventArgs e)
    {
        
        if(string.IsNullOrWhiteSpace(Article.Text) || Category.SelectedItem == null || Title.SelectedItem == null
            || ManufactureProduct.SelectedItem == null || SupplierProduct.SelectedItem == null || string.IsNullOrWhiteSpace(Price.Text)
            || string.IsNullOrWhiteSpace(Unit.Text) || string.IsNullOrWhiteSpace(CountProduct.Text) || string.IsNullOrWhiteSpace(Discount.Text))
        {
            Messages.Text = "Заполните все обязательные поля";
            Messages.IsVisible = true;
            await Task.Delay(1000);
            Messages.IsVisible = false;
        }
        else
        {
            try
            {
                var category = Category.SelectedItem as Category;
                var titleProduct = Title.SelectedItem as TitleProduct;
                var manufacture = ManufactureProduct.SelectedItem as Manufacture;
                var supplier = SupplierProduct.SelectedItem as Supplier;
                int id0 = dB.IdNewProduct();

                string description0 = "";
                string image0 = "";


                if(!string.IsNullOrWhiteSpace(Description.Text)) description0 = Description.Text;

                if(imagePath != null)
                {
                    string imagesDirectory = "images";

                    string extension = Path.GetExtension(imagePath);
                    if (string.IsNullOrEmpty(extension))
                    {
                        extension = ".png";
                    }

                    string newFileName = $"{id0}{extension}";
                    string newFilePath = Path.Combine(imagesDirectory, newFileName);

                    if (File.Exists(newFilePath))
                    {
                        File.Delete(newFilePath);
                        
                    }

                    using(var fileStream = File.Create(newFilePath))
                    {
                        streamImage.Position = 0;
                        await streamImage.CopyToAsync(fileStream);
                    }

                    image0 = newFileName;
                }

                Product product = new Product
                {
                    id = id0,
                    article = Article.Text,
                    titleProductId = titleProduct.id,
                    categoryId = category.id,
                    manufactureId = manufacture.id,
                    supplierId = supplier.id,
                    unit = Unit.Text,
                    price = double.Parse(Price.Text),
                    count = int.Parse(CountProduct.Text),
                    discount = int.Parse(Discount.Text),
                    description = description0,
                    image = image0 
                };

                string result = dB.InsertProduct(product);

                if(result == "-1")
                {
                    Messages.Text = "Этот артикул занят";
                    Messages.IsVisible = true;
                    await Task.Delay(1000);
                    Messages.IsVisible = false;
                }
                else
                {
                    Messages.Text = "Товар успешно добавлен";
                    Messages.IsVisible = true;
                    await Task.Delay(1000);
                    Messages.IsVisible = false;

                    var window = new AdminListProduct();
                    window.Show();
                    this.Close();
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

    private async void ButtonClickImage(object? sender, RoutedEventArgs e)
    {

        try
        {
            var topLevel = TopLevel.GetTopLevel((Control) sender!);

            var files = await topLevel!.StorageProvider.OpenFilePickerAsync(
                
                new FilePickerOpenOptions
                {
                    Title = "Выберите изображение",
                    AllowMultiple = false,
                    FileTypeFilter = new[]
                    {
                        new FilePickerFileType("Изображение")
                        {
                            Patterns = new[]{"*.jpeg", "*.png", "*.jpg"}
                        },
                        new FilePickerFileType("Все файлы")
                        {
                            Patterns = new[]{"*.*"}
                        }
                    }
                }


            );

            if(files.Count > 0 && files[0] is {} selectedFile)
            {
                imagePath = selectedFile.Path.LocalPath;

                streamImage = await selectedFile.OpenReadAsync();

                ImageProduct.Source = new Bitmap(streamImage);

                /*Messages.Text = ImageProduct.Source.ToString();
                Messages.IsVisible = true;
                await Task.Delay(1000);
                Messages.IsVisible = false;*/
               
            }
        }
        catch
        {
            Messages.Text = "Ошибка при выборе изоюражения";
            Messages.IsVisible = true;
            await Task.Delay(1000);
            Messages.IsVisible = false;
        }
                
    }
}