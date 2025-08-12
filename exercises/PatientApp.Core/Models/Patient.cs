namespace PatientApp.Core.Models;

/// <summary>
/// 환자의 성별을 나타내는 열거형(Enum)
/// </summary>
public enum Gender
{
    /// <summary>남성</summary>
    Male,
    /// <summary>여성</summary>
    Female
}

/// <summary>
/// 환자 정보를 담는 데이터 클래스
/// </summary>
public sealed class Patient
{
    /// <summary>
    /// 환자의 고유 식별자 (GUID: Globally Unique Identifier)
    /// 데이터베이스의 Primary Key 역할
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 환자 이름 (필수 입력 항목)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 환자 생년월일 (DateOnly 타입으로 시간 정보 제외)
    /// </summary>
    public DateOnly BirthDate { get; set; }

    /// <summary>
    /// 환자 성별 (Male 또는 Female)
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// 환자 연락처 (선택 입력 항목, null 가능)
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 환자에 대한 추가 메모 (선택 입력 항목, null 가능)
    /// </summary>
    public string? Notes { get; set; }
}


