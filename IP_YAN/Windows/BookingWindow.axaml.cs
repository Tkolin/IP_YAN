using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using IP_YAN.DateBase;
using IP_YAN.Model;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace IP_YAN.Windows;

public partial class BookingWindow : Window
{
    public BookingWindow()
    {
        InitializeComponent();
        DownloadDataGrid();
        UpdateComboBox();
    }
        private List<Booking> _DataBooking { get; set; }
    private List<Booking> _ViewBooking { get; set; }
    
    private List<Room> _RoomList { get; set; }
    private List<Visitor> _VisitorList { get; set; }

    public void DownloadDataGrid()
    {
        _DataBooking = DataBaseManager.GetBookings();
        UpdateDataGrid();
    }
    private void UpdateDataGrid()
    {
        _ViewBooking = _DataBooking;
        
        _ViewBooking = _ViewBooking.Where(c =>
            c.Id.ToString().Contains(SearchBox.Text) ||
            c.RoomID.ToString().Contains(SearchBox.Text) ||
            c.VisitorID.ToString().Contains(SearchBox.Text) ||
            c.FinalPrice.ToString().Contains(SearchBox.Text) ||
            c.DateOfEntry.ToString().Contains(SearchBox.Text) ||
            c.DateOfDeparture.ToString().Contains(SearchBox.Text) ||
            c.VisitorFirstLastName.Contains(SearchBox.Text)
        ).ToList();
        
        DataGrid.ItemsSource = _ViewBooking;
        
    }

    private void UpdateComboBox()
    {
        _RoomList= DataBaseManager.GetRooms();
        _VisitorList = DataBaseManager.GetVisitors();
        
        СBoxRoomID.ItemsSource = _RoomList;
        СBoxVisitorID.ItemsSource = _VisitorList;
    }
    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
        {
            СBoxRoomID.SelectedItem = null;
            СBoxVisitorID.SelectedItem = null;
            DPickerDateOfEntry.SelectedDate = DateTimeOffset.Now;
            DPickerDateOfDeparture.SelectedDate = null;
            NUpDownCost.Value = null;
        }
        else
        {
            Booking selectedBooking = DataGrid.SelectedItem as Booking;

            СBoxRoomID.SelectedItem = _RoomList.
                Where(c => c.Id == selectedBooking.RoomID).FirstOrDefault();
            СBoxVisitorID.SelectedItem = _VisitorList.
                Where(c => c.Id == selectedBooking.VisitorID).FirstOrDefault();
            DPickerDateOfEntry.SelectedDate = selectedBooking.DateOfEntry;
            DPickerDateOfDeparture.SelectedDate = selectedBooking.DateOfDeparture;
            NUpDownCost.Value = selectedBooking.FinalPrice;
        }
    }

    private void ResetBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = "";
    }

    private void BtnDelet_OnClick(object? sender, RoutedEventArgs e)
    {
        if(DataGrid.SelectedItem == null)
            return;
        
        DataBaseManager.DeleteBooking((DataGrid.SelectedItem as Booking).Id);
        
        DownloadDataGrid();
    }

    private void BtnRemoveSelect_OnClick(object? sender, RoutedEventArgs e)
    {
        DataGrid.SelectedItem = null;
    }
    

    private void BtnSave_OnClick(object? sender, RoutedEventArgs e)
    {
        if (СBoxRoomID.SelectedItem == null ||
            СBoxVisitorID.SelectedItem == null || 
            DPickerDateOfEntry.SelectedDate == null || 
            DPickerDateOfDeparture.SelectedDate == null || 
            NUpDownCost.Value == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Данные не заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }

        if (DataGrid.SelectedItem == null)
        {
            DataBaseManager.AddBooking(new Booking(
                0, 
                Convert.ToInt32((СBoxRoomID.SelectedItem as Room).Id), 
                Convert.ToInt32((СBoxVisitorID.SelectedItem as Visitor).Id), 
                Convert.ToDecimal(NUpDownCost.Value), 
                DPickerDateOfEntry.SelectedDate.Value.Date, 
                DPickerDateOfDeparture.SelectedDate.Value.Date
            ));
        }
        else
        {
            DataBaseManager.UpdateBooking(new Booking(
                ((Booking)DataGrid.SelectedItem).Id, 
                Convert.ToInt32((СBoxRoomID.SelectedItem as Room).Id), 
                Convert.ToInt32((СBoxVisitorID.SelectedItem as Visitor).Id), 
                Convert.ToDecimal(NUpDownCost.Value), 
                DPickerDateOfEntry.SelectedDate.Value.Date, 
                DPickerDateOfDeparture.SelectedDate.Value.Date

            ));
        }

        DownloadDataGrid();

    }

    private void BtnCreateService_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void SearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateDataGrid();
    }
}