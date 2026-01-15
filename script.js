// Base path 설정 (로컬: '', GitHub Pages: '/Portfolio')
const BASE_PATH = window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1' 
    ? '' 
    : '/Portfolio';

// 페이지 콘텐츠를 객체로 저장
const pages = {};

// 이미지 경로 수정 함수
function fixImagePaths(html) {
    // 모든 상대 경로를 절대 경로로 변환
    // project1.html에서 ../images/ → /Portfolio/images/ (GitHub) 또는 /images/ (로컬)
    return html.replace(/src="\.\.\/images\//g, `src="${BASE_PATH}/images/`)
               .replace(/src="\.\/images\//g, `src="${BASE_PATH}/images/`)
               .replace(/src="images\//g, `src="${BASE_PATH}/images/`)
               .replace(/href="\.\.\/images\//g, `href="${BASE_PATH}/images/`)
               .replace(/href="\.\/images\//g, `href="${BASE_PATH}/images/`);
}

// 페이지 로드 함수
async function loadPageContent(pageName) {
    if (pages[pageName]) {
        return pages[pageName];
    }
    
    try {
        const response = await fetch(`pages/${pageName}.html`);
        if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }
        const html = await response.text();
        const fixedHtml = fixImagePaths(html);
        pages[pageName] = fixedHtml;
        return fixedHtml;
    } catch (error) {
        console.error(`Error loading ${pageName}:`, error);
        return `
            <div style="text-align: center; padding: 40px; color: #e74c3c;">
                <h3>페이지를 불러올 수 없습니다</h3>
                <p>오류: ${error.message}</p>
                <p>파일: pages/${pageName}.html</p>
                <button onclick="location.reload()" style="margin-top: 20px; padding: 10px 20px; cursor: pointer;">새로고침</button>
            </div>
        `;
    }
}

// 페이지 표시 함수
async function showPage(pageName, shouldScroll = false) {
    const mainContent = document.getElementById('main-content');
    
    // 로딩 표시
    mainContent.innerHTML = '<p style="text-align: center; padding: 40px;">로딩 중...</p>';
    
    // 페이지 로드
    const html = await loadPageContent(pageName);
    mainContent.innerHTML = html;
    
    // 탭 클릭 시에만 스크롤 - 항상 헤더 높이 위치로 (제목이 보이게)
    if (shouldScroll) {
        // requestAnimationFrame으로 렌더링 완료 후 실행
        requestAnimationFrame(() => {
            requestAnimationFrame(() => {
                const header = document.querySelector('header');
                const headerHeight = header ? header.offsetHeight : 0;
                
                window.scrollTo({
                    top: headerHeight,
                    behavior: 'auto'
                });
            });
        });
    }
    
    console.log(`Displayed: ${pageName}`);
}

// 탭 버튼 초기화
function initializeTabs() {
    const tabButtons = document.querySelectorAll('.tab-button');
    
    console.log('Tab buttons found:', tabButtons.length);
    
    // 각 버튼에 이벤트 리스너 추가
    tabButtons.forEach(button => {
        button.addEventListener('click', async function(e) {
            e.preventDefault();
            
            const targetTab = this.getAttribute('data-tab');
            console.log('Tab clicked:', targetTab);
            
            // 모든 버튼에서 active 제거
            tabButtons.forEach(btn => btn.classList.remove('active'));
            
            // 현재 버튼에 active 추가
            this.classList.add('active');
            
            // 페이지 표시 (탭 클릭 시에는 스크롤 조정)
            await showPage(targetTab, true);
            
            // URL 해시 업데이트
            window.location.hash = targetTab;
        });
    });
    
    // URL 해시가 있으면 해당 페이지 로드
    const hash = window.location.hash.substring(1);
    if (hash && document.querySelector(`[data-tab="${hash}"]`)) {
        const button = document.querySelector(`[data-tab="${hash}"]`);
        button.click();
    } else {
        // 기본적으로 intro 페이지 로드 (첫 로드 시에는 스크롤 X)
        showPage('intro', false);
    }
}

// 페이지 로드 완료 시 초기화
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeTabs);
} else {
    initializeTabs();
}

// 뒤로가기/앞으로가기 지원
window.addEventListener('hashchange', function() {
    const hash = window.location.hash.substring(1);
    if (hash) {
        const button = document.querySelector(`[data-tab="${hash}"]`);
        if (button && !button.classList.contains('active')) {
            button.click();
        }
    }
});
