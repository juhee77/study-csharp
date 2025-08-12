using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PatientApp.Core.Models;
using PatientApp.Core.Persistence;
using PatientApp.Core.Paths;

namespace PatientApp.Gui.ViewModels;

/// <summary>
/// 메인 윈도우의 데이터와 사용자 상호작용을 관리하는 ViewModel 클래스
/// MVVM 패턴에서 Model과 View 사이의 중재자 역할을 합니다.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// 환자 데이터를 저장하고 불러오는 저장소 객체
    /// </summary>
    private readonly FilePatientRepository _repository;

    /// <summary>
    /// UI에 표시할 환자 목록을 담는 ObservableCollection
    /// 이 컬렉션이 변경되면 UI가 자동으로 업데이트됩니다.
    /// </summary>
    public ObservableCollection<Patient> Patients { get; } = new();

    /// <summary>
    /// 검색 키워드를 저장하는 속성
    /// [ObservableProperty] 어트리뷰트로 자동으로 INotifyPropertyChanged 구현
    /// </summary>
    [ObservableProperty]
    private string searchKeyword = string.Empty;

    /// <summary>
    /// 새 환자 추가 시 이름 입력 필드
    /// </summary>
    [ObservableProperty]
    private string newName = string.Empty;

    /// <summary>
    /// 새 환자 추가 시 생년월일 선택 필드
    /// DateTimeOffset? : null 가능한 날짜+시간 타입
    /// </summary>
    [ObservableProperty]
    private DateTimeOffset? newBirthDate = DateTimeOffset.Now;

    /// <summary>
    /// 새 환자 추가 시 성별 선택 필드
    /// 기본값은 Male(남성)
    /// </summary>
    [ObservableProperty]
    private Gender newGender = Gender.Male;

    /// <summary>
    /// 새 환자 추가 시 연락처 입력 필드 (선택사항)
    /// string? : null 가능한 문자열 타입
    /// </summary>
    [ObservableProperty]
    private string? newPhone;

    /// <summary>
    /// 새 환자 추가 시 비고 입력 필드 (선택사항)
    /// </summary>
    [ObservableProperty]
    private string? newNotes;

    /// <summary>
    /// 기본 생성자: 기본 저장소를 사용하여 ViewModel을 초기화합니다.
    /// </summary>
    public MainWindowViewModel() : this(new FilePatientRepository(StoragePath.ResolveDefault())) { }

    /// <summary>
    /// 의존성 주입을 위한 생성자: 테스트나 다른 저장소를 사용할 때 활용
    /// </summary>
    /// <param name="repository">사용할 환자 저장소</param>
    public MainWindowViewModel(FilePatientRepository repository)
    {
        _repository = repository;
        LoadAll(); // 초기 데이터 로드
    }

    /// <summary>
    /// 모든 환자 데이터를 로드하여 UI에 표시합니다.
    /// [RelayCommand] 어트리뷰트로 자동으로 ICommand 구현
    /// </summary>
    [RelayCommand]
    private void LoadAll()
    {
        Patients.Clear(); // 기존 목록 비우기
        var items = _repository.GetAll(); // 저장소에서 모든 데이터 가져오기
        Console.WriteLine($"[VM] LoadAll -> {items.Count} record(s)");
        
        // 새로 가져온 데이터를 UI 컬렉션에 추가
        foreach (var p in items)
        {
            Patients.Add(p);
        }
    }

    /// <summary>
    /// 이름으로 환자를 검색하여 결과를 UI에 표시합니다.
    /// 검색어가 비어있으면 전체 목록을 로드합니다.
    /// </summary>
    [RelayCommand]
    private void Search()
    {
        var keyword = (SearchKeyword ?? string.Empty).Trim(); // null 체크 후 공백 제거
        
        if (keyword.Length == 0)
        {
            Console.WriteLine("[VM] Search -> empty keyword, call LoadAll");
            LoadAll(); // 빈 검색어면 전체 목록 표시
            return;
        }

        Patients.Clear(); // 기존 목록 비우기
        var results = _repository.SearchByName(keyword); // 저장소에서 검색
        Console.WriteLine($"[VM] Search('{keyword}') -> {results.Count} record(s)");
        
        // 검색 결과를 UI 컬렉션에 추가
        foreach (var p in results)
        {
            Patients.Add(p);
        }
    }

    /// <summary>
    /// 새로운 환자를 추가합니다.
    /// 이름과 생년월일이 필수 입력 항목입니다.
    /// </summary>
    [RelayCommand]
    private void Add()
    {
        var name = (NewName ?? string.Empty).Trim(); // 이름에서 공백 제거
        
        // 필수 입력 항목 검증
        if (string.IsNullOrWhiteSpace(name) || NewBirthDate is null)
        {
            Console.WriteLine("[VM] Add -> invalid input");
            return; // 유효하지 않은 입력이면 추가하지 않음
        }

        // DateTimeOffset을 DateOnly로 변환 (시간 정보 제거)
        var birth = DateOnly.FromDateTime(NewBirthDate.Value.LocalDateTime);

        // 새로운 Patient 객체 생성
        var patient = new Patient
        {
            Id = Guid.NewGuid(),                    // 새로운 고유 ID 생성
            Name = name,                            // 입력된 이름
            BirthDate = birth,                      // 입력된 생년월일
            Gender = NewGender,                     // 선택된 성별
            Phone = string.IsNullOrWhiteSpace(NewPhone) ? null : NewPhone,     // 연락처 (빈 값이면 null)
            Notes = string.IsNullOrWhiteSpace(NewNotes) ? null : NewNotes      // 비고 (빈 값이면 null)
        };

        _repository.Add(patient);    // 저장소에 환자 추가
        Patients.Add(patient);       // UI 목록에도 추가
        Console.WriteLine($"[VM] Add -> now {Patients.Count} items");
        ClearForm();                 // 입력 폼 초기화
    }

    /// <summary>
    /// 환자 추가 폼의 모든 입력 필드를 초기값으로 리셋합니다.
    /// </summary>
    [RelayCommand]
    private void ClearForm()
    {
        NewName = string.Empty;              // 이름 초기화
        NewBirthDate = DateTimeOffset.Now;   // 생년월일을 현재 날짜로 설정
        NewGender = Gender.Male;             // 성별을 남성으로 설정
        NewPhone = null;                     // 연락처 초기화
        NewNotes = null;                     // 비고 초기화
    }

    /// <summary>
    /// 성별 선택 콤보박스에 표시할 옵션들
    /// UI에서 이 배열을 바인딩하여 사용
    /// </summary>
    public Gender[] GenderOptions { get; } = new[] { Gender.Male, Gender.Female };
}
