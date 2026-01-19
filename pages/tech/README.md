# 기술 문서 페이지 가이드

## 📁 파일 구조

```
pages/tech/
├── tech_style.css          # 기술 문서 전용 스타일
├── tech_project1.html      # 프로젝트 1: 소울디바이스
├── tech_project2.html      # 프로젝트 2: UI/UX 재정비
├── tech_project3.html      # 프로젝트 3: 체스도쿠
└── tech_project4.html      # 프로젝트 4: 슬롯 꼬치
```

## 🎯 주요 기능

### 1. 프로젝트 간 네비게이션
- 상단 드롭다운에서 다른 프로젝트의 기술 문서로 즉시 이동
- "프로젝트 페이지로 돌아가기" 버튼으로 원래 프로젝트 페이지 복귀

### 2. 사이드바 목차
- 현재 페이지의 섹션 목차 표시
- 클릭 시 해당 섹션으로 부드러운 스크롤
- 스크롤 위치에 따라 현재 섹션 하이라이트

### 3. 이미지 모달
- 이미지 클릭 시 전체 화면으로 확대
- 배경 클릭 또는 X 버튼으로 닫기
- 이미지 캡션 표시

## 📝 문서 작성 가이드

### 기본 구조

각 기술 문서는 다음과 같은 구조로 되어 있습니다:

```html
<section id="section-id" class="tech-section">
    <h2>섹션 제목</h2>
    
    <!-- 설명 텍스트 (3줄 정도) -->
    <p class="tech-description">
        설명 텍스트 1줄
        설명 텍스트 2줄
        설명 텍스트 3줄
    </p>

    <!-- 이미지 갤러리 (3개) -->
    <div class="tech-images">
        <div class="tech-image-item">
            <img src="../../images/project1/screenshot1.gif" alt="설명" onclick="openImageModal(this)">
            <div class="tech-image-caption">이미지 캡션</div>
        </div>
        <!-- 2개 더 반복 -->
    </div>

    <!-- 코드 블록 (10줄 정도) -->
    <div class="code-block">
        <pre><code>// C# 코드 예시
public class ExampleClass
{
    // 코드 내용
}
</code></pre>
        <div class="code-explanation">
            코드 설명 텍스트
        </div>
    </div>
</section>
```

### 새 섹션 추가하기

1. **HTML에 섹션 추가**
```html
<section id="new-section" class="tech-section">
    <h2>새로운 기술 섹션</h2>
    <!-- 내용 -->
</section>
```

2. **사이드바 목차에 추가**
```html
<ul class="toc-list">
    <li><a href="#existing-section">기존 섹션</a></li>
    <li><a href="#new-section">새로운 섹션</a></li>
</ul>
```

### 이미지 경로 주의사항

기술 문서는 `pages/tech/` 폴더에 있으므로, 이미지 경로는 다음과 같이 설정:
```html
<!-- 올바른 경로 -->
<img src="../../images/project1/screenshot1.gif">

<!-- 잘못된 경로 -->
<img src="../images/project1/screenshot1.gif">
```

## 🎨 스타일 커스터마이징

### 주요 CSS 클래스

| 클래스 | 용도 |
|--------|------|
| `.tech-section` | 각 기술 섹션 |
| `.tech-description` | 설명 텍스트 |
| `.tech-images` | 이미지 갤러리 그리드 |
| `.tech-image-item` | 개별 이미지 컨테이너 |
| `.code-block` | 코드 블록 컨테이너 |
| `.code-explanation` | 코드 설명 박스 |

### 색상 커스터마이징

`tech_style.css`에서 다음 색상을 수정할 수 있습니다:

```css
/* 헤더 배경 */
.tech-header {
    background-color: #2c3e50;
}

/* 강조 색상 */
.toc-list a.active {
    background-color: #3498db;
}

/* 코드 블록 배경 */
.code-block pre {
    background-color: #2c3e50;
}
```

## 📱 반응형 디자인

- **1024px 이하**: 사이드바가 상단으로 이동
- **768px 이하**: 모바일 최적화 레이아웃
- 이미지 갤러리는 화면 크기에 따라 자동 조정

## 🔗 링크 구조

```
index.html (메인)
    → pages/project1.html
        → pages/tech/tech_project1.html
            ↔ pages/tech/tech_project2.html (드롭다운)
            ↔ pages/tech/tech_project3.html (드롭다운)
            ↔ pages/tech/tech_project4.html (드롭다운)
```

## ✅ 체크리스트

새 기술 문서 추가 시 확인사항:

- [ ] HTML 파일 생성 (`tech_projectX.html`)
- [ ] 헤더에 올바른 제목과 돌아가기 링크
- [ ] 사이드바 드롭다운에 모든 프로젝트 포함
- [ ] 사이드바 목차에 모든 섹션 링크
- [ ] 이미지 경로 확인 (`../../images/`)
- [ ] 코드 블록에 언어 명시 (주로 C#)
- [ ] 각 섹션에 id 속성 추가
- [ ] 프로젝트 페이지에서 링크 연결

## 🚀 배포 전 확인

1. 모든 링크 작동 테스트
2. 이미지 로딩 확인
3. 모바일 반응형 확인
4. 코드 블록 가독성 확인
5. 목차 자동 하이라이트 작동 확인
