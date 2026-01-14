using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// 메인 메뉴 씬을 관리하는 스크립트
/// 게임 시작, 랭킹 등의 기능을 담당합니다.
/// </summary>
public class MainMenuSceneScript : MonoBehaviour
{
    [Header("UI Buttons")]
    [Tooltip("게임 시작 버튼")]
    [SerializeField] private Button startBtn;
    
    [Tooltip("랭킹 버튼")]
    [SerializeField] private Button rankBtn;

    [Header("High Score Display (Optional)")]
    [Tooltip("최고 점수 텍스트 (없으면 비워두기)")]
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        // 버튼 이벤트 연결
        if (startBtn != null)
        {
            startBtn.onClick.AddListener(OnStartButtonClicked);
        }

        if (rankBtn != null)
        {
            rankBtn.onClick.AddListener(OnRankButtonClicked);
        }

        // 최고 점수 표시
        UpdateHighScoreDisplay();

        // 메인 메뉴 BGM 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuBGM();
        }

#if PLAYSTORE_BUILD
        // 플레이스토어: Google AdMob 배너 광고 표시
        ShowBannerAd();
#elif INAPTOS_BUILD
        // 인앱토스: 인앱토스 IAA 배너 광고 표시
        ShowInAptosBannerAd();
#endif
    }

    private void OnDestroy()
    {
        // 버튼 이벤트 해제
        if (startBtn != null)
        {
            startBtn.onClick.RemoveListener(OnStartButtonClicked);
        }

        if (rankBtn != null)
        {
            rankBtn.onClick.RemoveListener(OnRankButtonClicked);
        }
    }

    /// <summary>
    /// 게임 시작 버튼 클릭
    /// </summary>
    private void OnStartButtonClicked()
    {
        Debug.Log("[MainMenuSceneScript] 게임 시작 버튼 클릭 - GamePlayScene 로드");
        
        // 버튼 클릭 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
#if PLAYSTORE_BUILD
        // 플레이스토어: Google AdMob 배너 광고 숨기기
        HideBannerAd();
#elif INAPTOS_BUILD
        // 인앱토스: 인앱토스 IAA 배너 광고 숨기기
        HideInAptosBannerAd();
#endif
        
        // GamePlayScene으로 전환
        SceneManager.LoadScene("GamePlayScene");
    }

    /// <summary>
    /// 랭킹 버튼 클릭
    /// </summary>
    private void OnRankButtonClicked()
    {
        Debug.Log("[MainMenuSceneScript] 랭킹 버튼 클릭");
        
        // 버튼 클릭 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
#if INAPTOS_BUILD
        // 인앱토스: 리더보드 표시
        ShowLeaderboard();
#else
        // 플레이스토어 또는 개발 모드: 랭킹 기능 없음
        Debug.Log("랭킹 기능은 인앱토스 버전에서만 사용 가능합니다.");
#endif
    }

    /// <summary>
    /// 최고 점수 표시 업데이트
    /// </summary>
    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            int highScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = $"Best: {highScore}";
            Debug.Log($"[MainMenuSceneScript] 최고 점수 표시: {highScore}");
        }
    }

#if PLAYSTORE_BUILD
    // ============================================================
    // 플레이스토어 전용: Google AdMob 배너 광고 메서드
    // ============================================================

    /// <summary>
    /// 배너 광고 표시 (플레이스토어 전용)
    /// </summary>
    private void ShowBannerAd()
    {
        Debug.Log("[MainMenuSceneScript] 배너 광고 표시");
        
        if (AdMobManager.Instance != null)
        {
            AdMobManager.Instance.ShowBannerAd();
        }
        else
        {
            Debug.LogWarning("[MainMenuSceneScript] AdMobManager가 없습니다");
        }
    }

    /// <summary>
    /// 배너 광고 숨기기 (플레이스토어 전용)
    /// </summary>
    private void HideBannerAd()
    {
        Debug.Log("[MainMenuSceneScript] 배너 광고 숨기기");
        
        if (AdMobManager.Instance != null)
        {
            AdMobManager.Instance.HideBannerAd();
        }
        else
        {
            Debug.LogWarning("[MainMenuSceneScript] AdMobManager가 없습니다");
        }
    }
#endif

#if INAPTOS_BUILD
    // ============================================================
    // 인앱토스 전용: 리더보드 및 광고 메서드
    // ============================================================

    /// <summary>
    /// 리더보드 표시 (인앱토스 전용)
    /// </summary>
    private void ShowLeaderboard()
    {
        Debug.Log("[MainMenuSceneScript] 리더보드 표시 (TODO: 인앱토스 SDK 연동)");
        // TODO Phase 5: LeaderboardManager.Instance.ShowLeaderboard();
    }

    /// <summary>
    /// 배너 광고 표시 (인앱토스 전용)
    /// </summary>
    private void ShowInAptosBannerAd()
    {
        Debug.Log("[MainMenuSceneScript] 인앱토스 배너 광고 표시 (TODO: 인앱토스 IAA 연동)");
        // TODO Phase 5: InAptosAdManager.Instance.ShowBannerAd();
    }

    /// <summary>
    /// 배너 광고 숨기기 (인앱토스 전용)
    /// </summary>
    private void HideInAptosBannerAd()
    {
        Debug.Log("[MainMenuSceneScript] 인앱토스 배너 광고 숨기기 (TODO: 인앱토스 IAA 연동)");
        // TODO Phase 5: InAptosAdManager.Instance.HideBannerAd();
    }
#endif
}
