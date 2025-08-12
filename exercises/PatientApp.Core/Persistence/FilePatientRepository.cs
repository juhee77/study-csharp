using System.Text.Json;
using System.Text.Json.Serialization;
using PatientApp.Core.Models;

namespace PatientApp.Core.Persistence;

/// <summary>
/// JSON 파일을 사용하여 환자 데이터를 저장하고 불러오는 저장소 클래스
/// 파일 기반 데이터베이스 역할을 합니다.
/// </summary>
public sealed class FilePatientRepository
{
    /// <summary>
    /// 환자 데이터가 저장될 JSON 파일의 경로
    /// </summary>
    private readonly string _filePath;

    /// <summary>
    /// JSON 직렬화/역직렬화를 위한 설정 옵션
    /// WriteIndented: true로 설정하여 JSON을 읽기 쉽게 포맷팅
    /// Converters: DateOnly 타입을 JSON으로 변환하기 위한 커스텀 컨버터
    /// </summary>
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new DateOnlyJsonConverter() }
    };

    /// <summary>
    /// 생성자: 저장소를 초기화하고 필요한 파일/폴더를 생성합니다.
    /// </summary>
    /// <param name="filePath">데이터를 저장할 JSON 파일 경로</param>
    public FilePatientRepository(string filePath)
    {
        _filePath = filePath;
        EnsureStorage(); // 저장소 초기화
        Console.WriteLine($"[PatientRepo] Storage file: {_filePath}");
    }

    /// <summary>
    /// 모든 환자 데이터를 이름순, 생년월일순으로 정렬하여 반환합니다.
    /// </summary>
    /// <returns>정렬된 환자 목록</returns>
    public List<Patient> GetAll()
    {
        var list = Load(); // 파일에서 데이터 로드
        Console.WriteLine($"[PatientRepo] GetAll -> {list.Count} record(s) before sort");
        
        // LINQ를 사용하여 정렬: 이름순 -> 생년월일순
        return list
            .OrderBy(p => p.Name)           // 이름으로 먼저 정렬
            .ThenBy(p => p.BirthDate)       // 이름이 같으면 생년월일로 정렬
            .ToList();                       // List<Patient>로 변환하여 반환
    }

    /// <summary>
    /// 특정 ID를 가진 환자를 찾아 반환합니다.
    /// </summary>
    /// <param name="id">찾을 환자의 GUID</param>
    /// <returns>찾은 환자 또는 null (찾지 못한 경우)</returns>
    public Patient? GetById(Guid id)
    {
        // LINQ의 FirstOrDefault: 조건에 맞는 첫 번째 항목을 찾거나 기본값(null) 반환
        return Load().FirstOrDefault(p => p.Id == id);
    }

    /// <summary>
    /// 이름에 특정 키워드가 포함된 환자들을 검색합니다.
    /// 대소문자를 구분하지 않습니다.
    /// </summary>
    /// <param name="keyword">검색할 키워드</param>
    /// <returns>검색 결과 환자 목록 (이름순 정렬)</returns>
    public List<Patient> SearchByName(string keyword)
    {
        keyword = keyword.Trim(); // 앞뒤 공백 제거
        
        if (keyword.Length == 0) 
            return new List<Patient>(); // 빈 키워드면 빈 목록 반환

        var result = Load()
            .Where(p => p.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase)) // 대소문자 구분 없이 검색
            .OrderBy(p => p.Name) // 이름순 정렬
            .ToList();

        Console.WriteLine($"[PatientRepo] SearchByName('{keyword}') -> {result.Count} record(s)");
        return result;
    }

    /// <summary>
    /// 새로운 환자를 추가합니다.
    /// </summary>
    /// <param name="patient">추가할 환자 정보</param>
    public void Add(Patient patient)
    {
        var list = Load();        // 기존 데이터 로드
        list.Add(patient);        // 새 환자 추가
        Save(list);               // 파일에 저장
        Console.WriteLine($"[PatientRepo] Added: {patient.Id} | {patient.Name} | {patient.BirthDate:yyyy-MM-dd} | {patient.Gender}");
    }

    /// <summary>
    /// 저장소가 사용할 수 있도록 필요한 폴더와 파일을 생성합니다.
    /// </summary>
    private void EnsureStorage()
    {
        // 파일이 있는 디렉토리 경로 추출
        var directory = Path.GetDirectoryName(_filePath);
        
        // 디렉토리가 존재하지 않으면 생성
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // JSON 파일이 존재하지 않으면 빈 환자 목록으로 초기화
        if (!File.Exists(_filePath))
        {
            Save(new List<Patient>());
            Console.WriteLine($"[PatientRepo] Created new storage at {_filePath}");
        }
    }

    /// <summary>
    /// JSON 파일에서 환자 데이터를 로드합니다.
    /// </summary>
    /// <returns>로드된 환자 목록 (파일이 없거나 오류 시 빈 목록)</returns>
    private List<Patient> Load()
    {
        try
        {
            using var stream = File.OpenRead(_filePath); // 파일을 읽기 모드로 열기
            var list = JsonSerializer.Deserialize<List<Patient>>(stream, _jsonOptions); // JSON을 객체로 변환
            return list ?? new List<Patient>(); // null이면 빈 목록 반환
        }
        catch
        {
            // 파일 읽기 실패 시 빈 목록 반환 (예: 파일이 손상된 경우)
            return new List<Patient>();
        }
    }

    /// <summary>
    /// 환자 데이터를 JSON 파일에 저장합니다.
    /// </summary>
    /// <param name="list">저장할 환자 목록</param>
    private void Save(List<Patient> list)
    {
        using var stream = File.Create(_filePath); // 파일을 쓰기 모드로 생성 (기존 파일 덮어쓰기)
        JsonSerializer.Serialize(stream, list, _jsonOptions); // 객체를 JSON으로 변환하여 파일에 쓰기
        Console.WriteLine($"[PatientRepo] Saved {list.Count} record(s) to {_filePath}");
    }
}

/// <summary>
/// DateOnly 타입을 JSON 문자열과 상호 변환하기 위한 커스텀 컨버터
/// .NET의 기본 JSON 직렬화는 DateOnly를 지원하지 않아서 필요합니다.
/// </summary>
internal sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    /// <summary>
    /// 날짜를 문자열로 변환할 때 사용할 형식 (예: 1990-05-21)
    /// </summary>
    private const string Format = "yyyy-MM-dd";

    /// <summary>
    /// JSON 문자열을 DateOnly로 변환 (역직렬화)
    /// </summary>
    /// <param name="reader">JSON 읽기 도구</param>
    /// <param name="typeToConvert">변환할 타입</param>
    /// <param name="options">직렬화 옵션</param>
    /// <returns>변환된 DateOnly 값</returns>
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var str = reader.GetString(); // JSON에서 문자열 읽기
        
        if (str is null) 
            return default; // null이면 기본값 반환
        
        // 문자열을 DateOnly로 파싱 시도
        if (DateOnly.TryParseExact(str, Format, out var date))
        {
            return date; // 성공하면 파싱된 날짜 반환
        }
        
        return default; // 실패하면 기본값 반환
    }

    /// <summary>
    /// DateOnly를 JSON 문자열로 변환 (직렬화)
    /// </summary>
    /// <param name="writer">JSON 쓰기 도구</param>
    /// <param name="value">변환할 DateOnly 값</param>
    /// <param name="options">직렬화 옵션</param>
    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        // DateOnly를 지정된 형식의 문자열로 변환하여 JSON에 쓰기
        writer.WriteStringValue(value.ToString(Format));
    }
}


