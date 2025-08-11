using PatientApp.Core.Models;
using PatientApp.Core.Paths;
using PatientApp.Core.Persistence;

var dataFilePath = StoragePath.ResolveDefault();
var repository = new FilePatientRepository(dataFilePath);

while (true)
{
    ShowMenu();
    Console.Write("메뉴 선택: ");
    var input = Console.ReadLine()?.Trim();

    switch (input)
    {
        case "1":
            AddPatient(repository);
            break;
        case "2":
            ListPatients(repository);
            break;
        case "3":
            FindById(repository);
            break;
        case "4":
            SearchByName(repository);
            break;
        case "5":
            Console.WriteLine("종료합니다.");
            return;
        default:
            Console.WriteLine("잘못된 선택입니다. 1-5 중에서 선택하세요.\n");
            break;
    }
}

static void ShowMenu()
{
    Console.WriteLine();
    Console.WriteLine("============================");
    Console.WriteLine(" 환자 관리 (저장/조회) 콘솔 ");
    Console.WriteLine("============================");
    Console.WriteLine("1) 환자 추가");
    Console.WriteLine("2) 전체 목록");
    Console.WriteLine("3) ID로 조회");
    Console.WriteLine("4) 이름으로 검색");
    Console.WriteLine("5) 종료");
}

static void AddPatient(FilePatientRepository repository)
{
    Console.WriteLine();
    Console.WriteLine("[환자 추가]");

    var name = PromptNonEmpty("이름: ");

    var birthDate = PromptDate("생년월일(예: 1990-05-21): ");

    var gender = PromptGender("성별(M/F 또는 남/여): ");

    Console.Write("연락처(선택): ");
    var phone = (Console.ReadLine() ?? string.Empty).Trim();

    Console.Write("비고(선택): ");
    var notes = (Console.ReadLine() ?? string.Empty).Trim();

    var newPatient = new Patient
    {
        Id = Guid.NewGuid(),
        Name = name,
        BirthDate = birthDate,
        Gender = gender,
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes
    };

    repository.Add(newPatient);
    Console.WriteLine($"등록 완료: {newPatient.Id}");
}

static void ListPatients(FilePatientRepository repository)
{
    Console.WriteLine();
    Console.WriteLine("[전체 목록]");
    var patients = repository.GetAll();
    if (patients.Count == 0)
    {
        Console.WriteLine("등록된 환자가 없습니다.");
        return;
    }

    foreach (var p in patients)
    {
        Console.WriteLine($"- {p.Id} | {p.Name} | {p.BirthDate:yyyy-MM-dd} | {ToKorean(p.Gender)} | {(p.Phone ?? "-")}");
    }
}

static void FindById(FilePatientRepository repository)
{
    Console.WriteLine();
    Console.WriteLine("[ID로 조회]");
    Console.Write("ID 입력(Guid): ");
    var raw = Console.ReadLine()?.Trim();
    if (!Guid.TryParse(raw, out var id))
    {
        Console.WriteLine("유효한 Guid 형식이 아닙니다.");
        return;
    }

    var patient = repository.GetById(id);
    if (patient is null)
    {
        Console.WriteLine("해당 ID의 환자를 찾을 수 없습니다.");
        return;
    }

    PrintPatientDetail(patient);
}

static void SearchByName(FilePatientRepository repository)
{
    Console.WriteLine();
    Console.WriteLine("[이름으로 검색]");
    Console.Write("이름(부분 가능): ");
    var keyword = (Console.ReadLine() ?? string.Empty).Trim();
    if (keyword.Length == 0)
    {
        Console.WriteLine("검색어가 비어있습니다.");
        return;
    }

    var results = repository.SearchByName(keyword);
    if (results.Count == 0)
    {
        Console.WriteLine("검색 결과가 없습니다.");
        return;
    }

    foreach (var p in results)
    {
        Console.WriteLine($"- {p.Id} | {p.Name} | {p.BirthDate:yyyy-MM-dd} | {ToKorean(p.Gender)} | {(p.Phone ?? "-")}");
    }
}

static void PrintPatientDetail(Patient p)
{
    Console.WriteLine("----------------------------");
    Console.WriteLine($"ID      : {p.Id}");
    Console.WriteLine($"이름     : {p.Name}");
    Console.WriteLine($"생년월일 : {p.BirthDate:yyyy-MM-dd}");
    Console.WriteLine($"성별     : {ToKorean(p.Gender)}");
    Console.WriteLine($"연락처   : {p.Phone ?? "-"}");
    Console.WriteLine($"비고     : {p.Notes ?? "-"}");
    Console.WriteLine("----------------------------");
}

static string PromptNonEmpty(string label)
{
    while (true)
    {
        Console.Write(label);
        var value = (Console.ReadLine() ?? string.Empty).Trim();
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
        Console.WriteLine("값은 비어 있을 수 없습니다. 다시 입력하세요.");
    }
}

static DateOnly PromptDate(string label)
{
    while (true)
    {
        Console.Write(label);
        var raw = (Console.ReadLine() ?? string.Empty).Trim();
        if (DateOnly.TryParseExact(raw, "yyyy-MM-dd", out var date))
        {
            return date;
        }
        Console.WriteLine("형식이 올바르지 않습니다. 예: 1990-05-21");
    }
}

static Gender PromptGender(string label)
{
    while (true)
    {
        Console.Write(label);
        var raw = (Console.ReadLine() ?? string.Empty).Trim().ToLowerInvariant();
        if (raw is "m" or "남") return Gender.Male;
        if (raw is "f" or "여") return Gender.Female;
        Console.WriteLine("M/F 또는 남/여 중 하나로 입력하세요.");
    }
}

static string ToKorean(Gender gender)
{
    return gender == Gender.Male ? "남" : "여";
}
