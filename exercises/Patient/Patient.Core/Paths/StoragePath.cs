namespace PatientApp.Core.Paths;

/// <summary>
/// 애플리케이션의 데이터 저장 경로를 관리하는 정적 유틸리티 클래스
/// 운영체제별로 적절한 앱 데이터 폴더를 찾아 환자 데이터를 저장합니다.
/// </summary>
public static class StoragePath
{
    /// <summary>
    /// 애플리케이션의 기본 데이터 저장 경로를 결정하고 반환합니다.
    /// 
    /// Windows: %APPDATA%\PatientApp\patients.json
    /// macOS: ~/Library/Application Support/PatientApp/patients.json  
    /// Linux: ~/.local/share/PatientApp/patients.json
    /// 
    /// 필요한 디렉토리가 없으면 자동으로 생성합니다.
    /// </summary>
    /// <returns>환자 데이터를 저장할 JSON 파일의 전체 경로</returns>
    public static string ResolveDefault()
    {
        // Environment.GetFolderPath: 운영체제별 표준 폴더 경로를 가져옴
        // SpecialFolder.ApplicationData: 앱 데이터를 저장하는 표준 위치
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        
        // PatientApp이라는 하위 폴더를 생성하여 다른 앱과 구분
        var dir = Path.Combine(appData, "PatientApp");
        
        // 디렉토리가 존재하지 않으면 생성
        if (!Directory.Exists(dir)) 
            Directory.CreateDirectory(dir);
        
        // patients.json 파일의 전체 경로 반환
        return Path.Combine(dir, "patients.json");
    }
}


