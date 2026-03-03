using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace obyv010;

public partial class AdminEditingProduct : Window
{
    DB dB = new DB();
    Product product0;
    Stream streamImage = null;
    string imagePath = null;
    public AdminEditingProduct(Product product)
    {
        InitializeComponent();

        var categories = dB.GetCategories();
        var titles = dB.GetTitleProducts();
        var manufacture = dB.GetManufactures();
        var suppliers = dB.GetSuppliers();

        Category.ItemsSource = categories;
        Title.ItemsSource = titles;
        ManufactureProduct.ItemsSource = manufacture;
        SupplierProduct.ItemsSource = suppliers;

        Category.SelectedItem = categories.FirstOrDefault(c => c.id == product.categoryId);
        Title.SelectedItem = titles.FirstOrDefault(t => t.id == product.titleProductId);
        ManufactureProduct.SelectedItem = manufacture.FirstOrDefault(m => m.id == product.manufactureId);
        SupplierProduct.SelectedItem = suppliers.FirstOrDefault(s => s.id == product.supplierId);

        Category.DisplayMemberBinding = new Binding("name");
        Title.DisplayMemberBinding = new Binding("name");
        ManufactureProduct.DisplayMemberBinding = new Binding("name");
        SupplierProduct.DisplayMemberBinding = new Binding("name");

        Article.Text = product.article;
        Price.Text = product.price.ToString();
        Discount.Text = product.discount.ToString();
        Unit.Text = product.unit;
        CountProduct.Text = product.count.ToString();
        Description.Text = product.description;

        Article.IsReadOnly = true;

        product0 = product;

        try
        {
            var bitmap = new Bitmap($"images/{product.image}");
            ImageProduct.Source = bitmap;
        }
        catch
        {
            try
            {
                var bitmap = new Bitmap($"{product.image}");
                ImageProduct.Source = bitmap;
            }
            catch
            {
                var bitmap = new Bitmap($"images/picture.png");
                ImageProduct.Source = bitmap;
            }
        }

    }

    private void ButtonClickBack(object? sender, RoutedEventArgs e)
    {
        
        var window = new AdminListProduct();
        window.Show();
        this.Close();
                
    }

    private async void ButtonClickDelete(object? sender, RoutedEventArgs e)
    {

        try
        {
            string result = dB.DeleteProduct(product0.id);

            if(result == "-1")
            {
                Messages.Text = $"Товар используется в заказах";
                Messages.IsVisible = true;
                await Task.Delay(1000);
                Messages.IsVisible = false;
            }
            else
            {
                Messages.Text = $"Товар успешно удален";
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

    private async void ButtonClickSave(object? sender, RoutedEventArgs e)
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

                    string newFileName = $"{product0.id}{extension}";
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

                dB.UpdateProduct(product);


                Messages.Text = "Данные успешно сохранены";
                Messages.IsVisible = true;
                await Task.Delay(1000);
                Messages.IsVisible = false;

                
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