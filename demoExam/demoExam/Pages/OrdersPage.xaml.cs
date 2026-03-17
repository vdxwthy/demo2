using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.EntityFrameworkCore;
using demoExam.Model;

namespace demoExam.Pages;

public partial class OrdersPage : Page
{
    private readonly User? _currentUser;

    public OrdersPage(User? currentUser)
    {
        InitializeComponent();
        _currentUser = currentUser;
        LoadOrders();
    }

    private void LoadOrders()
    {
        using var db = new DbshopbootsContext();
        
        var orders = db.Orders
            .Include(o => o.PickupPoint)
            .OrderByDescending(o => o.DateOrder)
            .Select(o => new OrderItem
            {
                Id = o.Id,
                Code = o.Code,
                StatusOrder = o.StatusOrder,
                DateOrder = o.DateOrder,
                DateDelivery = o.DateDelivery,
                PickupAddress = $"{o.PickupPoint.AddressIndex}, {o.PickupPoint.AddressCity}, {o.PickupPoint.AddressStreet}, {o.PickupPoint.AddressNumberHouse}"
            })
            .ToList();

        OrdersList.ItemsSource = orders;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (NavigationService.CanGoBack)
        {
            NavigationService.GoBack();
        }
        else
        {
            NavigationService.Navigate(new ProductsPage(_currentUser));
        }
    }

    private class OrderItem
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string StatusOrder { get; set; } = "";
        public DateOnly DateOrder { get; set; }
        public DateOnly DateDelivery { get; set; }
        public string PickupAddress { get; set; } = "";
    }
}
