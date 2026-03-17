using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using demoExam.Model;

namespace demoExam.Pages;

public partial class ProductsPage : Page
    {
        private readonly User? _currentUser;

        public bool IsAdminOrManager => _currentUser != null && (_currentUser.Role == "менеджер" || _currentUser.Role == "администратр");

        public ProductsPage(User? currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            DataContext = this;

            if (IsAdminOrManager)
            {
                OrdersButton.Visibility = System.Windows.Visibility.Visible;
                AddProductButton.Visibility = System.Windows.Visibility.Visible;
            }

            LoadProducts();
        }

        private void LoadProducts()
        {
            using var db = new DbshopbootsContext();

            var products = db.Products.Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Price,
                    p.Discount,
                    Description = p.Description,
                    CategoryName = p.Category.Name,
                    ManufacturerName = p.Manufacture.Name,
                    SupplierName = p.Supplier.Name,
                    Unit = db.Storages.Where(s => s.ProductId == p.Id).Select(s => s.Unit).FirstOrDefault(),
                    StockCountNullable = db.Storages.Where(s => s.ProductId == p.Id).Sum(s => (int?)s.Count),
                    ImageName = db.Images.Where(i => i.ProductId == p.Id).Select(i => i.Image1).FirstOrDefault()
                })
                .ToList();

            var list = new List<ProductItem>();
            foreach (var b in products)
            {
                list.Add(new ProductItem
                {
                    Id = b.Id,
                    Name = b.Name,
                    Price = b.Price,
                    Discount = b.Discount,
                    Description = b.Description ?? "",
                    CategoryName = b.CategoryName,
                    ManufacturerName = b.ManufacturerName,
                    SupplierName = b.SupplierName,
                    Unit = string.IsNullOrWhiteSpace(b.Unit) ? "шт." : b.Unit!,
                    StockCount = b.StockCountNullable ?? 0,
                    ImageName = b.ImageName,
                    FinalPrice = Math.Round(b.Price * (100 - b.Discount) / 100.0, 2),
                    PhotoUri = string.IsNullOrWhiteSpace(b.ImageName)
                        ? "pack://application:,,,/Resource/Icons/picture.png"
                        : (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "Products", b.ImageName))
                            ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "Products", b.ImageName)
                            : $"pack://application:,,,/Resource/Products/{b.ImageName}"),
                    CategoryAndName = $"{b.CategoryName} | {b.Name}",
                    IsBigDiscount = b.Discount > 15,
                    IsDiscounted = b.Discount > 0
                });
            }

            ProductsList.ItemsSource = list;
            RecordsCountText.Text = $"{list.Count} записей";
        }

        private void AddProductButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var editWindow = new Windows.ProductEditWindow(null, _currentUser!);
            editWindow.ShowDialog();
            LoadProducts();
        }

        private void EditProductButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int productId)
            {
                using var db = new DbshopbootsContext();
                var product = db.Products.FirstOrDefault(p => p.Id == productId);
                if (product != null)
                {
                     var editWindow = new Windows.ProductEditWindow(product, _currentUser!);
                     editWindow.ShowDialog();
                     LoadProducts();
                }
            }
        }

        private void OrdersButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage(_currentUser));
        }

        private class ProductItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double Price { get; set; }
        public int Discount { get; set; }
        public string Description { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string ManufacturerName { get; set; } = "";
        public string SupplierName { get; set; } = "";
        public string Unit { get; set; } = "шт.";
        public int StockCount { get; set; }
        public string? ImageName { get; set; }
        public string PhotoUri { get; set; } = "";
        public double FinalPrice { get; set; }
        public string CategoryAndName { get; set; } = "";
        public bool IsBigDiscount { get; set; }
        public bool IsDiscounted { get; set; }
    }
}
