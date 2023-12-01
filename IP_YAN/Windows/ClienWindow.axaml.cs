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

public partial class ProcedureWindow : Avalonia.Controls.Window
{
    private List<Visitor> _DataVisitor { get; set; }
    private List<Visitor> _ViewVisitor { get; set; }
    

    public ProcedureWindow()
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
    private void UpdateDataGrid()
    {
        _ViewVisitor = _DataVisitor;
        
        if (SearchBox.Text.Length > 0)
            _ViewVisitor = _ViewVisitor.Where(c => 
                c.Id.ToString().Contains(SearchBox.Text) ||
                c.DoctorID.ToString().Contains(SearchBox.Text) ||
                c.DiseaseRecordID.ToString().Contains(SearchBox.Text) ||
                c.Description.ToString().Contains(SearchBox.Text) ||
                c.DateStart.ToString().Contains(SearchBox.Text) ||
                c.Duration.ToString().Contains(SearchBox.Text) ||
                c.Cost.ToString().Contains(SearchBox.Text) ||
                c.StatusID.ToString().Contains(SearchBox.Text)
            ).ToList();
        
        DataGrid.ItemsSource = _ViewVisitor;
        
    }
    private void DataGrid_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataGrid.SelectedItem == null)
        {
            CBoxAttendingDoctor.SelectedItem = null;
            CBoxDisiaseRecord.SelectedItem = null;
            TBoxDescription.Text = "";
            DPickerDateStart.SelectedDate = DateTime.Now;
            NUpDownDuration.Value = 0;
            NUpDownCost.Value = 0;
            CBoxStatus.SelectedItem = null;
        }
        else
        {
            Procedure value = DataGrid.SelectedItem as Procedure; 
            CBoxAttendingDoctor.SelectedItem = _DoctorList.Where(c => c.Id == value.DoctorID);
            CBoxDisiaseRecord.SelectedItem = _PatientDisesList.Where(c => c.Id == value.DiseaseRecordID);
            TBoxDescription.Text = value.Description;
            DPickerDateStart.SelectedDate = value.DateStart;
            NUpDownDuration.Value = value.Duration;
            NUpDownCost.Value = value.Cost;
            CBoxStatus.SelectedItem = _StatusList.Where(c => c.Id == value.StatusID);

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
        
        DataBaseManager.DeleteVisitor(DataGrid.SelectedItem as Visitor);
        
        DownloadDataGrid();
    }

    private void BtnRemoveSelect_OnClick(object? sender, RoutedEventArgs e)
    {
        DataGrid.SelectedItem = null;
    }

    private void BtnSavet_OnClick(object? sender, RoutedEventArgs e)
    {
        if (CBoxAttendingDoctor.SelectedItem == null ||
            CBoxDisiaseRecord.SelectedItem == null || 
            TBoxDescription.Text.Length >= 1 ||
            DPickerDateStart.SelectedDate == null || 
            NUpDownDuration.Value == null || 
            NUpDownCost.Value == null || 
            CBoxStatus.SelectedItem == null)
        {
            MessageBoxManager.GetMessageBoxStandard("Ошибка", "Данные не заполнены", ButtonEnum.Ok).ShowAsync();
            return;
        }


        if (DataGrid.SelectedItem == null)
        {
            DataBaseManager.AddVisitor(new Procedure(
                0,
                (CBoxDisiaseRecord.SelectedItem as Doctor).Id,
                (CBoxDisiaseRecord.SelectedItem as DiseaseRecord).Id,
                TBoxDescription.Text,
            DPickerDateStart.SelectedDate.Value.Date,
                Convert.ToInt32(NUpDownDuration.Value.Value),
                NUpDownCost.Value.Value,
               (CBoxStatus.SelectedItem as Status).Id
            ));
        }
  
        else
        {
            DataBaseManager.AddVisitor(new Procedure(
                ((Procedure)DataGrid.SelectedItem).Id,              
                (CBoxDisiaseRecord.SelectedItem as Doctor).Id,
                (CBoxDisiaseRecord.SelectedItem as DiseaseRecord).Id,
                TBoxDescription.Text,
                DPickerDateStart.SelectedDate.Value.Date,
                Convert.ToInt32(NUpDownDuration.Value.Value),
                NUpDownCost.Value.Value,
                (CBoxStatus.SelectedItem as Status).Id
            ));
        }

        DownloadDataGrid();
    }
    }