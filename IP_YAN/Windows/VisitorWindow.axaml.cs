using System;
using System.Collections.Generic;
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

public partial class VisitorWindow : Avalonia.Controls.Window
{
    private List<Visitor> _DataVisitor { get; set; }
    private List<Visitor> _ViewVisitor { get; set; }
    
    private List<Room> _RoomList { get; set; }
    private List<Booking> _BookingList { get; set; }

    public VisitorWindow()
    {
        InitializeComponent();
        DownloadDataGrid();
        UpdateComboBox();
    }
    public void DownloadDataGrid()
    {
        _DataVisitor = DataBaseManager.GetVisitors();
        UpdateDataGrid();
    }

    private void UpdateComboBox()
    {
        _RoomList = DataBaseManager.GetRooms();
        _BookingList = DataBaseManager.GetBookings();

        CBoxRoom.ItemsSource = _RoomList; 
    }
    private void UpdateDataGrid()
    {
        _ViewVisitor = _DataVisitor;
        
        if (SearchBox.Text.Length > 0)
            _ViewVisitor = _ViewVisitor.Where(c => 
               c.Id.ToString().Contains(SearchBox.Text) ||
                c.FirstName.Contains(SearchBox.Text) ||
                c.LastName.Contains(SearchBox.Text) || 
                c.PhoneNumber.Contains(SearchBox.Text)
            ).ToList();
        
        DataGrid.ItemsSource = _ViewVisitor;
        
    }
    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
        {
            TBoxFirstName.Text = "";
            TBoxLastName.Text = "";
            TBoxPhoneNumber.Text = "";
            DPickerDateBirth.SelectedDate = DateTime.Now;
        }
        else
        {
            Visitor visitor = DataGrid.SelectedItem as Visitor;
            // Предполагается, что ваш список _List имеет данные, из которых нужно выбирать
            TBoxFirstName.Text = visitor.FirstName ;
            TBoxLastName.Text = visitor.LastName ;
            TBoxPhoneNumber.Text = visitor.PhoneNumber;
            DPickerDateBirth.SelectedDate = visitor.DateOfBirth;
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
        
        DataBaseManager.DeleteVisitor((DataGrid.SelectedItem as Visitor).Id);
        
        DownloadDataGrid();
    }

    private void BtnRemoveSelect_OnClick(object? sender, RoutedEventArgs e)
    {
        DataGrid.SelectedItem = null;
    }

    private void BtnSavet_OnClick(object? sender, RoutedEventArgs e)
    {
        if (TBoxFirstName.Text.Length <= 0 ||
        TBoxLastName.Text.Length <= 0 ||
        TBoxPhoneNumber.Text.Length <= 0 ||
        DPickerDateBirth.SelectedDate == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Данные не заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }


        if (DataGrid.SelectedItem == null)
        {
            DataBaseManager.AddVisitor(new Visitor(
                0,
                TBoxFirstName.Text,
                TBoxLastName.Text,
                TBoxPhoneNumber.Text,
                DPickerDateBirth.SelectedDate.Value.Date.Date
            ));
        }
  
        else
        {
            DataBaseManager.UpdateVisitor(new Visitor(
                ((Visitor)DataGrid.SelectedItem).Id,              
                TBoxFirstName.Text,
                TBoxLastName.Text,
                TBoxPhoneNumber.Text,
                DPickerDateBirth.SelectedDate.Value.Date.Date
            ));
        }

        DownloadDataGrid();
    }

    private void BtnCreateBooking_OnClick(object? sender, RoutedEventArgs e)
    {
        if (CBoxRoom.SelectedItem == null ||
            DataGrid.SelectedItem == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Данные не заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }
        DataBaseManager.AddBooking(new Booking(
            0, 
            Convert.ToInt32((CBoxRoom.SelectedItem as Room).Id), 
            Convert.ToInt32((DataGrid.SelectedItem as Visitor).Id), 
            Convert.ToDecimal((CBoxRoom.SelectedItem as Room).Cost*NUpDownTime.Value), 
            DPickerDateStart.SelectedDate.Value.Date, 
            DPickerDateStart.SelectedDate.Value.Date.AddDays(Convert.ToInt32(NUpDownTime.Value))
        ));
        CBoxRoom.SelectedItem = null;
        FreeRoomUpdate();
    }

    private void SearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateDataGrid();
    }

    private void FreeRoomUpdate()
    {
        _BookingList = DataBaseManager.GetBookings();
        if (NUpDownTime.Value < 1 || DPickerDateStart.SelectedDate == null)
        {
            CBoxRoom.IsEnabled = false;
            TBoxSupprotMesage.IsVisible = true;
            return;
        }
        else
        {
            CBoxRoom.IsEnabled = true;
            TBoxSupprotMesage.IsVisible = false;
        }

        if (_BookingList.Count > 0)
        {
            int timeFilter = Convert.ToInt32(NUpDownTime.Value.Value);
            DateTime dateFilter = DPickerDateStart.SelectedDate.Value.Date;
            DateTime endDateFilter = dateFilter.AddDays(timeFilter);

            List<Booking> intersectingBookings = _BookingList
                .Where(b =>
                    !(b.DateOfDeparture <= dateFilter || b.DateOfEntry >= endDateFilter)
                ).ToList();

            List<int> occupiedRoomIds = intersectingBookings.Select(b => b.RoomID).Distinct().ToList();

            List<Room> availableRooms = _RoomList
                .Where(room => !occupiedRoomIds.Contains(room.Id))
                .ToList();

            CBoxRoom.ItemsSource = availableRooms;
        }
        else
        {
            CBoxRoom.ItemsSource = _RoomList;
        }

        CBoxRoom.SelectedItem = null;
        CBoxRoom.Clear();


    }

    private void NUpDownTime_OnValueChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        FreeRoomUpdate();
    }

    private void DPickerDateStart_OnSelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
    {
        FreeRoomUpdate();
    }
}