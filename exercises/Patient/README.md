# Patient Management System

환자 관리 시스템입니다. CLI를 제거하고 GUI 중심으로 리팩토링되었습니다.

## 프로젝트 구조

```
Patient/
├── Patient.Core/          # 핵심 비즈니스 로직
│   ├── Models/           # 환자 모델
│   ├── Paths/            # 저장 경로 관리
│   └── Persistence/      # 데이터 저장소
├── Patient.Gui/           # GUI 애플리케이션 (Avalonia)
└── Patient.sln            # 솔루션 파일
```

## 주요 변경사항

- ❌ **CLI 제거**: 콘솔 애플리케이션 제거
- ✅ **GUI 중심**: Avalonia 기반 GUI 애플리케이션 유지
- ✅ **명확한 구조**: Patient 폴더로 통합하여 구조 개선

## 실행 방법

### GUI 애플리케이션 실행
```bash
cd Patient.Gui
dotnet run
```

### 빌드
```bash
dotnet build Patient.sln
```

## 기술 스택

- **.NET 9.0**
- **Avalonia UI** - 크로스 플랫폼 GUI 프레임워크
- **CommunityToolkit.Mvvm** - MVVM 패턴 지원

## 기능

- 환자 추가/수정/삭제
- 환자 검색 (ID, 이름)
- 환자 목록 조회
- 데이터 파일 저장
