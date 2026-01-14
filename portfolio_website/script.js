// 탭 전환 기능
document.addEventListener('DOMContentLoaded', function() {
    const tabButtons = document.querySelectorAll('.tab-button');
    const tabContents = document.querySelectorAll('.tab-content');

    tabButtons.forEach(button => {
        button.addEventListener('click', function() {
            const targetTab = this.getAttribute('data-tab');

            // 모든 탭 버튼과 콘텐츠에서 active 클래스 제거
            tabButtons.forEach(btn => btn.classList.remove('active'));
            tabContents.forEach(content => content.classList.remove('active'));

            // 클릭한 탭 버튼과 해당 콘텐츠에 active 클래스 추가
            this.classList.add('active');
            document.getElementById(targetTab).classList.add('active');

            // 페이지 상단으로 부드럽게 스크롤
            window.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    });

    // URL 해시로 직접 접근 지원 (예: #project1)
    const hash = window.location.hash.substring(1);
    if (hash) {
        const targetButton = document.querySelector(`[data-tab="${hash}"]`);
        if (targetButton) {
            targetButton.click();
        }
    }
});
