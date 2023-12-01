using Avalonia.Controls;
using Avalonia.Interactivity;
using IP_YAN.DateBase;

namespace IP_YAN.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    private void BtnClient_OnClick(object? sender, RoutedEventArgs e)
    {
        VisitorWindow sw = new VisitorWindow();
        sw.ShowDialog(this);
    }

    private void BtnBooking_OnClick(object? sender, RoutedEventArgs e)
    {
        BookingWindow sw = new BookingWindow();
        sw.ShowDialog(this);
    }

    private void BtnService_OnClick(object? sender, RoutedEventArgs e)
    {
        ServiceWindow sw = new ServiceWindow();
        sw.ShowDialog(this);
    }

    private void BtnBack_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
}