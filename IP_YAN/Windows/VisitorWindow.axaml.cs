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
    

    public VisitorWindow()
    {
        InitializeComponent();
        DownloadDataGrid();
    }
    public void DownloadDataGrid()
    {
        _DataVisitor = DataBaseManager.GetVisitors();
        UpdateDataGrid();
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
        throw new NotImplementedException();
    }

    private void SearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateDataGrid();
    }
}