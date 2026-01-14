# 이미지 추가 가이드

## 📁 폴더 구조

```
Portfolio/
├── images/
│   ├── project1/
│   │   ├── screenshot1.jpg
│   │   ├── screenshot2.jpg
│   │   └── screenshot3.jpg
│   ├── project2/
│   │   ├── screenshot1.jpg
│   │   ├── screenshot2.jpg
│   │   └── screenshot3.jpg
│   ├── project3/
│   │   ├── screenshot1.jpg
│   │   ├── screenshot2.jpg
│   │   └── screenshot3.jpg
│   └── project4/
│       ├── screenshot1.jpg
│       ├── screenshot2.jpg
│       └── screenshot3.jpg
```

## 📸 각 프로젝트별 권장 이미지

### Project 1 (소울디바이스)
- **screenshot1.jpg**: 게임플레이 화면 (전투 장면 또는 메인 화면)
- **screenshot2.jpg**: UI 시스템 화면 (인벤토리, 메뉴 등)
- **screenshot3.jpg**: 다국어 지원 화면 (여러 언어가 보이는 UI)

### Project 2 (UI/UX 재정비)
- **screenshot1.jpg**: UI 리팩토링 전후 비교 (side-by-side)
- **screenshot2.jpg**: 개발 일지 또는 문서화 화면
- **screenshot3.jpg**: UI 애니메이션 개선 (연출 화면)

### Project 3 (체스도쿠)
- **screenshot1.jpg**: 게임플레이 화면
- **screenshot2.jpg**: SNS 마케팅 자료 (Twitter/Discord 포스트)
- **screenshot3.jpg**: 1:1 멀티플레이 화면

### Project 4 (슬롯 꼬치)
- **screenshot1.jpg**: 게임플레이 화면
- **screenshot2.jpg**: AI 생성 그래픽 에셋 모음
- **screenshot3.jpg**: Google AdMob 광고 화면 또는 수익 그래프

## 🎨 이미지 사양 권장

- **파일 형식**: JPG 또는 PNG
- **파일명**: screenshot1.jpg, screenshot2.jpg, screenshot3.jpg
- **권장 크기**: 
  - 가로: 800px ~ 1920px
  - 세로: 450px ~ 1080px
  - 비율: 16:9 권장 (다른 비율도 자동 조정됨)
- **용량**: 각 파일 500KB 이하 권장 (웹 최적화)

## 🔧 이미지 추가 방법

1. 각 프로젝트 폴더에 이미지 파일을 복사합니다
2. 파일명을 `screenshot1.jpg`, `screenshot2.jpg`, `screenshot3.jpg`로 맞춥니다
3. 브라우저를 새로고침하면 이미지가 표시됩니다

## 📝 이미지 캡션 수정

이미지 설명을 변경하고 싶다면 각 프로젝트 HTML 파일에서 수정:

```html
<p class="screenshot-caption">원하는 설명으로 변경</p>
```

## 💡 Placeholder 제거

이미지를 추가하기 전까지는 회색 placeholder가 표시됩니다.
이미지를 추가하면 자동으로 이미지가 표시됩니다.

## ⚠️ 주의사항

- 이미지가 없어도 포트폴리오는 정상 작동합니다 (placeholder 표시)
- Steam이나 Google Play에서 스크린샷을 다운로드하여 사용 가능
- 저작권이 있는 이미지는 사용에 주의하세요
