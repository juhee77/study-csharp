using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PatientApp.Core.Models;
using PatientApp.Core.Persistence;
using PatientApp.Core.Paths;

namespace PatientApp.Gui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly FilePatientRepository _repository;

    public ObservableCollection<Patient> Patients { get; } = new();

    [ObservableProperty]
    private string searchKeyword = string.Empty;

    [ObservableProperty]
    private string newName = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? newBirthDate = DateTimeOffset.Now;

    [ObservableProperty]
    private Gender newGender = Gender.Male;

    [ObservableProperty]
    private string? newPhone;

    [ObservableProperty]
    private string? newNotes;

    public MainWindowViewModel() : this(new FilePatientRepository(StoragePath.ResolveDefault())) { }

    public MainWindowViewModel(FilePatientRepository repository)
    {
        _repository = repository;
        LoadAll();
    }

    [RelayCommand]
    private void LoadAll()
    {
        Patients.Clear();
        var items = _repository.GetAll();
        Console.WriteLine($"[VM] LoadAll -> {items.Count} record(s)");
        foreach (var p in items)
        {
            Patients.Add(p);
        }
    }

    [RelayCommand]
    private void Search()
    {
        var keyword = (SearchKeyword ?? string.Empty).Trim();
        if (keyword.Length == 0)
        {
            LoadAll();
            return;
        }

        Patients.Clear();
        var results = _repository.SearchByName(keyword);
        Console.WriteLine($"[VM] Search('{keyword}') -> {results.Count} record(s)");
        foreach (var p in results)
        {
            Patients.Add(p);
        }
    }

    [RelayCommand]
    private void Add()
    {
        var name = (NewName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name) || NewBirthDate is null)
        {
            Console.WriteLine("[VM] Add -> invalid input");
            return;
        }

        var birth = DateOnly.FromDateTime(NewBirthDate.Value.LocalDateTime);

        var patient = new Patient
        {
            Id = Guid.NewGuid(),
            Name = name,
            BirthDate = birth,
            Gender = NewGender,
            Phone = string.IsNullOrWhiteSpace(NewPhone) ? null : NewPhone,
            Notes = string.IsNullOrWhiteSpace(NewNotes) ? null : NewNotes
        };

        _repository.Add(patient);
        Patients.Add(patient);
        Console.WriteLine($"[VM] Add -> now {Patients.Count} items");
        ClearForm();
    }

    [RelayCommand]
    private void ClearForm()
    {
        NewName = string.Empty;
        NewBirthDate = DateTimeOffset.Now;
        NewGender = Gender.Male;
        NewPhone = null;
        NewNotes = null;
    }

    public Gender[] GenderOptions { get; } = new[] { Gender.Male, Gender.Female };
}
