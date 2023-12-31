﻿using System;
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

public partial class ServiceWindow : Window
{
    private List<Service> _DataService { get; set; }
    private List<Service> _ViewService { get; set; }
    private List<TypeOfService> _TypeOfServiceList { get; set; }
    private List<Staff> _StaffList { get; set; }
    private List<Booking> _BookingList { get; set; }

    public ServiceWindow()
    {
        InitializeComponent();
        DownloadDataGrid();
        UpdateComboBox();
    }
    private void UpdateComboBox()
    {
        _TypeOfServiceList= DataBaseManager.GetTypeOfServices();
        _StaffList = DataBaseManager.GetStaff();
        _BookingList = DataBaseManager.GetBookings();
        
        CBoxTypeOfServiceID.ItemsSource = _TypeOfServiceList;
        CBoxStaffID.ItemsSource = _StaffList;
        CBoxBookingID.ItemsSource = _BookingList;
        CBoxBookingIDFILTER.ItemsSource = _BookingList;
    }
    public void DownloadDataGrid()
    {
        _DataService = DataBaseManager.GetServices();
        UpdateDataGrid();
    }
    private void UpdateDataGrid()
    {
        _ViewService = _DataService;
        
        if (SearchBox.Text.Length > 0)
            _ViewService = _ViewService.Where(c => 
                c.Id.ToString().Contains(SearchBox.Text) ||
                c.TypeOfServiceName.ToLower().Contains(SearchBox.Text.ToLower()) ||
                c.StaffFirstLastName.ToLower().Contains(SearchBox.Text.ToLower()) || 
                c.DateOfService.ToString().Contains(SearchBox.Text)
            ).ToList();
        
        if(CBoxBookingIDFILTER.SelectedItem != null)
            _ViewService = _ViewService.Where(c => 
                c.BookingID == ((Booking)CBoxBookingIDFILTER.SelectedItem).Id).ToList();
        
        if(DPickerStartFILTER.SelectedDate != null)
            _ViewService = _ViewService.Where(c => 
                c.DateOfService >= DPickerStartFILTER.SelectedDate).ToList();
        
        if(DPickerEndFILTER.SelectedDate != null)
            _ViewService = _ViewService.Where(c => 
                c.DateOfService <= DPickerEndFILTER.SelectedDate).ToList();
        DataGrid.ItemsSource = _ViewService;
        
    }
    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
        {
            CBoxTypeOfServiceID.SelectedItem = null;
            DPickerDateOfService.SelectedDate = DateTime.Now;
            NUpDownTimes.Value = 0;
            NUpDownPrice.Value = 0;
            CBoxBookingID.SelectedItem =null;
            CBoxStaffID.SelectedItem = null;
        }
        else
        {
            Service service = DataGrid.SelectedItem as Service;

            CBoxTypeOfServiceID.SelectedItem = _TypeOfServiceList.
                Where(c=>c.Id == service.TypeOfServiceID).FirstOrDefault();
            DPickerDateOfService.SelectedDate = DateTime.Now;
            NUpDownTimes.Value = service.Time;
            NUpDownPrice.Value = service.Price;
            CBoxBookingID.SelectedItem = _BookingList.
                Where(c=>c.Id == service.BookingID).FirstOrDefault();
            CBoxStaffID.SelectedItem = _StaffList.
                Where(c=>c.Id == service.StaffID).FirstOrDefault();
        }

    }

    private void ResetBtn_OnClick(object? sender, RoutedEventArgs e)
    {
        SearchBox.Text = "";
        DPickerEndFILTER.SelectedDate = null;
        DPickerStartFILTER.SelectedDate = null;
        CBoxBookingIDFILTER.SelectedItem = null;

    }

    private void BtnDelet_OnClick(object? sender, RoutedEventArgs e)
    {
        if(DataGrid.SelectedItem == null)
            return;
        
        DataBaseManager.DeleteService((DataGrid.SelectedItem as Service).Id);
        
        DownloadDataGrid();
    }

    private void BtnRemoveSelect_OnClick(object? sender, RoutedEventArgs e)
    {
        DataGrid.SelectedItem = null;
    }

    private void BtnSavet_OnClick(object? sender, RoutedEventArgs e)
    {
        if (CBoxTypeOfServiceID.SelectedItem == null ||
            DPickerDateOfService.SelectedDate == null ||
            NUpDownTimes.Value <= 0 ||
            NUpDownPrice.Value <= 0||
            CBoxBookingID.SelectedItem == null ||
            CBoxStaffID.SelectedItem == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Данные не заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }


        if (DataGrid.SelectedItem == null)
        {
            DataBaseManager.AddService(new Service(
                0,
                (CBoxTypeOfServiceID.SelectedItem as TypeOfService).Id,
                DPickerDateOfService.SelectedDate.Value.Date.Date,
                Convert.ToInt32(NUpDownTimes.Value.Value),
                Convert.ToInt32(NUpDownPrice.Value.Value),
                (CBoxBookingID.SelectedItem as Booking).Id,
            (CBoxStaffID.SelectedItem as Staff).Id
            ));
        }
  
        else
        {
            DataBaseManager.UpdateService(new Service(
                (DataGrid.SelectedItem as Service).Id,
                (CBoxTypeOfServiceID.SelectedItem as TypeOfService).Id,
                DPickerDateOfService.SelectedDate.Value.Date.Date,
                Convert.ToInt32(NUpDownTimes.Value.Value),
                Convert.ToInt32(NUpDownPrice.Value.Value),
                (CBoxBookingID.SelectedItem as Booking).Id,
                (CBoxStaffID.SelectedItem as Staff).Id
            ));
        }

        DownloadDataGrid();
    }

    private void BtnCreateBooking_OnClick(object? sender, RoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void SearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateDataGrid();
    }

    private void DPickerEndFILTER_OnSelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
    {
          UpdateDataGrid();
    }

    private void DPickerStartFILTER_OnSelectedDateChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
    {
        UpdateDataGrid();
    }

    private void CBoxBookingIDFILTER_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        UpdateDataGrid();
    }
}