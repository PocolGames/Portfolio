// Base path 설정 (로컬: '', GitHub Pages: '/Portfolio')
const BASE_PATH = window.location.hostname === 'localhost' || window.location.hostname === '127.0.0.1' 
    ? '' 
    : '/Portfolio';

// 페이지 콘텐츠를 객체로 저장
const pages = {};

// 이미지 경로 수정 함수
function fixImagePaths(html) {
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
            </div>
        `;
    }
}

// 페이지 표시 함수
async function showPage(pageName, shouldScroll = false) {
    const mainContent = document.getElementById('main-content');
    mainContent.innerHTML = '<p style="text-align: center; padding: 40px;">로딩 중...</p>';
    
    const html = await loadPageContent(pageName);
    mainContent.innerHTML = html;
    
    initializeImageModal();
    
    if (shouldScroll) {
        requestAnimationFrame(() => {
            const header = document.querySelector('header');
            const headerHeight = header ? header.offsetHeight : 0;
            window.scrollTo({ top: headerHeight, behavior: 'auto' });
        });
    }
}

// 탭 버튼 초기화
function initializeTabs() {
    const tabButtons = document.querySelectorAll('.tab-button');
    
    tabButtons.forEach(button => {
        button.addEventListener('click', async function(e) {
            e.preventDefault();
            const targetTab = this.getAttribute('data-tab');
            
            // 버튼 활성화
            tabButtons.forEach(btn => btn.classList.remove('active'));
            this.classList.add('active');
            
            // 페이지 표시
            await showPage(targetTab, true);
            
            // URL 변경
            window.location.hash = targetTab;
        });
    });
    
    // 초기 페이지 로드
    const hash = window.location.hash.substring(1) || 'intro';
    const button = document.querySelector(`[data-tab="${hash}"]`);
    if (button) {
        // 모든 탭 버튼에서 active 제거 후 현재 탭만 활성화
        tabButtons.forEach(btn => btn.classList.remove('active'));
        button.classList.add('active');
        showPage(hash, false);
    }
    
    // 페이지 로드 시에도 hash를 체크하여 올바른 탭 활성화
    window.addEventListener('load', function() {
        const currentHash = window.location.hash.substring(1) || 'intro';
        const currentButton = document.querySelector(`[data-tab="${currentHash}"]`);
        if (currentButton && !currentButton.classList.contains('active')) {
            tabButtons.forEach(btn => btn.classList.remove('active'));
            currentButton.classList.add('active');
        }
    });
}

// showTab 함수 - intro 링크용
window.showTab = function(tabName) {
    const button = document.querySelector(`[data-tab="${tabName}"]`);
    if (button) {
        button.click();
    }
};

// 뒤로가기/앞으로가기 처리 (기술문서에서 돌아올 때도 처리)
window.addEventListener('hashchange', function() {
    const hash = window.location.hash.substring(1) || 'intro';
    const button = document.querySelector(`[data-tab="${hash}"]`);
    
    if (button) {
        const tabButtons = document.querySelectorAll('.tab-button');
        const wasActive = button.classList.contains('active');
        
        // 모든 탭에서 active 제거
        tabButtons.forEach(btn => btn.classList.remove('active'));
        // 현재 탭에 active 추가
        button.classList.add('active');
        
        // 이전에 active가 아니었다면 페이지 로드
        if (!wasActive) {
            showPage(hash, false);
        }
    }
});

// 페이지 로드
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initializeTabs);
} else {
    initializeTabs();
}

// 이미지 모달 기능
function initializeImageModal() {
    let modal = document.getElementById('image-modal');
    if (!modal) {
        modal = document.createElement('div');
        modal.id = 'image-modal';
        modal.className = 'image-modal';
        modal.innerHTML = `
            <button class="image-modal-close" aria-label="닫기">&times;</button>
            <img src="" alt="">
            <div class="image-modal-caption"></div>
        `;
        document.body.appendChild(modal);
    }

    const modalImg = modal.querySelector('img');
    const modalCaption = modal.querySelector('.image-modal-caption');
    const closeBtn = modal.querySelector('.image-modal-close');

    const images = document.querySelectorAll('.screenshot-img');
    images.forEach(img => {
        img.addEventListener('click', function() {
            modal.classList.add('active');
            modalImg.src = this.src;
            modalImg.alt = this.alt;
            
            const captionElement = this.closest('.screenshot-item')?.querySelector('.screenshot-caption');
            if (captionElement) {
                modalCaption.textContent = captionElement.textContent;
            } else {
                modalCaption.textContent = this.alt || '';
            }
            
            document.body.style.overflow = 'hidden';
        });
    });

    const closeModal = () => {
        modal.classList.remove('active');
        document.body.style.overflow = '';
    };

    closeBtn.addEventListener('click', (e) => {
        e.stopPropagation();
        closeModal();
    });

    modal.addEventListener('click', closeModal);

    modalImg.addEventListener('click', (e) => {
        e.stopPropagation();
        closeModal();
    });

    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && modal.classList.contains('active')) {
            closeModal();
        }
    });
}
