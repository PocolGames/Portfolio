using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 플레이 전체 흐름을 관리하는 메인 매니저
/// 점수, 체력, 게임 상태를 관리하고 UI를 업데이트합니다.
/// </summary>
public class GamePlayManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("슬롯 스핀 컨트롤러")]
    [SerializeField] private SlotSpinController slotSpinController;

    [Tooltip("터치 입력 핸들러")]
    [SerializeField] private TouchInputHandler touchInputHandler;

    [Tooltip("음식 애니메이션 컨트롤러")]
    [SerializeField] private FoodAnimationController foodAnimationController;

    [Header("UI - Score")]
    [Tooltip("점수 텍스트")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("UI - Health")]
    [Tooltip("체력 이미지 (3개)")]
    [SerializeField] private Image[] healthSlots = new Image[3];

    [Tooltip("체력 스프라이트 (0: 꽉찬 하트, 1: 빈 하트)")]
    [SerializeField] private Sprite[] heartSprites = new Sprite[2];

    [Header("UI - Game Over")]
    [Tooltip("게임 오버 패널")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("최종 점수 텍스트")]
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Tooltip("Again 버튼 (보상형 광고)")]
    [SerializeField] private Button againButton;

    [Tooltip("Replay 버튼")]
    [SerializeField] private Button replayButton;

    [Tooltip("Exit 버튼")]
    [SerializeField] private Button exitButton;

    // 게임 상태
    private int currentScore = 0;
    private int currentHealth = 3;
    private bool isGameOver = false;
    private bool isProcessingRound = false; // 라운드 처리 중 플래그

    private void Awake()
    {
        // 이벤트 구독
        if (slotSpinController != null)
        {
            slotSpinController.OnSpinStarted += OnSpinStarted;
            slotSpinController.OnSlotStopped += OnSlotStopped;
            slotSpinController.OnSpinCompleted += OnSpinCompleted;
        }

        // 버튼 이벤트 연결
        if (replayButton != null)
        {
            replayButton.onClick.AddListener(OnReplayButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        if (againButton != null)
        {
            againButton.onClick.AddListener(OnAgainButtonClicked);
        }
    }

    private void Start()
    {
        InitializeGame();

        // 게임 플레이 BGM 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGamePlayBGM();
        }
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        if (slotSpinController != null)
        {
            slotSpinController.OnSpinStarted -= OnSpinStarted;
            slotSpinController.OnSlotStopped -= OnSlotStopped;
            slotSpinController.OnSpinCompleted -= OnSpinCompleted;
        }

        // 버튼 이벤트 해제
        if (replayButton != null)
        {
            replayButton.onClick.RemoveListener(OnReplayButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }

        if (againButton != null)
        {
            againButton.onClick.RemoveListener(OnAgainButtonClicked);
        }
    }

    /// <summary>
    /// 게임 초기화
    /// </summary>
    private void InitializeGame()
    {
        currentScore = 0;
        currentHealth = 3;
        isGameOver = false;

        // UI 초기화
        UpdateScoreUI();
        UpdateHealthUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 슬롯 초기화
        if (slotSpinController != null)
        {
            slotSpinController.Initialize();
        }

        // 음식 애니메이션 초기화
        if (foodAnimationController != null)
        {
            foodAnimationController.Initialize();
        }

        // 자동으로 첫 라운드 시작
        StartNewRound();
    }

    /// <summary>
    /// 새 라운드 시작
    /// </summary>
    private void StartNewRound()
    {
        if (isGameOver)
        {
            // Debug.Log("[GamePlayManager] 게임 오버 상태 - 라운드 시작 불가");
            return;
        }

        // 대기 중인 Invoke 취소
        CancelInvoke(nameof(StartNewRound));
        
        // 플래그 초기화
        isProcessingRound = false;

        // Debug.Log($"[GamePlayManager] 새 라운드 시작 - 점수: {currentScore}, 체력: {currentHealth}");

        // 스핀 속도 업데이트 (난이도 조절)
        if (slotSpinController != null)
        {
            slotSpinController.UpdateSpinSpeed(currentScore);
        }

        // 스핀 시작
        if (slotSpinController != null)
        {
            slotSpinController.StartSpin();
        }
    }

    /// <summary>
    /// 스핀 시작 이벤트
    /// </summary>
    private void OnSpinStarted()
    {
        // Debug.Log("[GamePlayManager] 스핀 시작");

        // 터치 입력 활성화
        if (touchInputHandler != null)
        {
            touchInputHandler.EnableTouch();
        }
    }

    /// <summary>
    /// 슬롯 정지 이벤트
    /// </summary>
    private void OnSlotStopped(int slotIndex)
    {
        // Debug.Log($"[GamePlayManager] 슬롯 {slotIndex} 정지");
    }

    /// <summary>
    /// 스핀 완료 이벤트
    /// </summary>
    /// <param name="isMatched">매칭 성공 여부</param>
    /// <param name="results">슬롯 결과</param>
    private void OnSpinCompleted(bool isMatched, int[] results)
    {
        // 중복 처리 방지
        if (isProcessingRound)
        {
            Debug.LogWarning("[GamePlayManager] 라운드 처리 중 중복 호출 방지!");
            return;
        }

        isProcessingRound = true;

        // Debug.Log($"[GamePlayManager] 스핀 완료 - 매칭: {isMatched}, 결과: [{results[0]}, {results[1]}, {results[2]}]");

        // 터치 입력 비활성화
        if (touchInputHandler != null)
        {
            touchInputHandler.DisableTouch();
        }

        if (isMatched)
        {
            // 매칭 성공 - 점수 증가
            OnMatchSuccess();
        }
        else
        {
            // 매칭 실패 - 체력 감소
            OnMatchFailed();
        }
    }

    /// <summary>
    /// 매칭 성공 처리
    /// </summary>
    private void OnMatchSuccess()
    {
        currentScore++;
        UpdateScoreUI();

        // 매칭 성공 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMatchSuccessSound();
        }

        // Debug.Log($"[GamePlayManager] 매칭 성공! 현재 점수: {currentScore}");

        // 음식이 꼬치로 날아가는 애니메이션
        if (foodAnimationController != null && slotSpinController != null)
        {
            // 매칭된 음식 인덱스 가져오기
            int[] matchedFoods = new int[] 
            { 
                slotSpinController.GetSlotResult(0),
                slotSpinController.GetSlotResult(1),
                slotSpinController.GetSlotResult(2)
            };

            foodAnimationController.PlayMatchSuccessAnimation(matchedFoods, () =>
            {
                // 애니메이션 완료 후 다음 라운드 시작
                StartNewRound();
            });
        }
        else
        {
            // 애니메이션 컨트롤러가 없으면 바로 다음 라운드
            Invoke(nameof(StartNewRound), 1f);
        }
    }

    /// <summary>
    /// 매칭 실패 처리
    /// </summary>
    private void OnMatchFailed()
    {
        currentHealth--;
        UpdateHealthUI();

        // 매칭 실패 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMatchFailSound();
        }

        // Debug.Log($"[GamePlayManager] 매칭 실패! 현재 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            // 게임 오버
            OnGameOver();
        }
        else
        {
            // 다음 라운드 시작 (임시로 1초 후)
            Invoke(nameof(StartNewRound), 1f);
        }
    }

    /// <summary>
    /// 게임 오버 처리
    /// </summary>
    private void OnGameOver()
    {
        isGameOver = true;

        Debug.Log($"[GamePlayManager] 게임 오버! 최종 점수: {currentScore}");

        // 게임 오버 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGameOverSound();
        }

        // 최고 점수 갱신
        UpdateHighScore();

#if INAPTOS_BUILD
        // 인앱토스: 리더보드에 점수 제출
        SubmitScoreToLeaderboard(currentScore);
#endif

        // 게임 오버 UI 표시
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = currentScore.ToString();
        }
    }

    /// <summary>
    /// 최고 점수 갱신
    /// </summary>
    private void UpdateHighScore()
    {
        int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
        
        if (currentScore > currentHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
            Debug.Log($"[GamePlayManager] 새 최고 점수! {currentScore}");
        }
    }

    /// <summary>
    /// 점수 UI 업데이트
    /// </summary>
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    /// <summary>
    /// 체력 UI 업데이트
    /// </summary>
    private void UpdateHealthUI()
    {
        if (healthSlots == null || heartSprites == null || heartSprites.Length < 2)
            return;

        for (int i = 0; i < healthSlots.Length; i++)
        {
            if (healthSlots[i] != null)
            {
                // i번째 하트가 체력 범위 내면 꽉찬 하트, 아니면 빈 하트
                healthSlots[i].sprite = (i < currentHealth) ? heartSprites[0] : heartSprites[1];
            }
        }
    }

    /// <summary>
    /// Again 버튼 클릭 (보상형 광고)
    /// </summary>
    private void OnAgainButtonClicked()
    {
        Debug.Log("[GamePlayManager] Again 버튼 클릭 - 체력 회복 후 게임 재개");
        
        // 버튼 클릭 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
#if PLAYSTORE_BUILD
        // 플레이스토어: Google AdMob 보상형 광고
        ShowRewardedAd(() => {
            RecoverHealthAndContinue();
        });
#elif INAPTOS_BUILD
        // 인앱토스: 인앱토스 IAA 보상형 광고
        ShowInAptosRewardedAd(() => {
            RecoverHealthAndContinue();
        });
#else
        // 개발 모드: 광고 없이 바로 체력 회복
        RecoverHealthAndContinue();
#endif
    }

    /// <summary>
    /// 체력 회복 후 게임 재개
    /// </summary>
    private void RecoverHealthAndContinue()
    {
        // 체력 1 회복 (최대 3)
        currentHealth = Mathf.Min(currentHealth + 1, 3);
        UpdateHealthUI();

        Debug.Log($"[GamePlayManager] 체력 회복 - 현재 체력: {currentHealth}");

        // 게임 오버 상태 해제
        isGameOver = false;

        // 게임 오버 패널 닫기
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // 게임 재개 (점수는 유지)
        Debug.Log($"[GamePlayManager] 게임 재개 - 현재 점수: {currentScore}, 체력: {currentHealth}");
        StartNewRound();
    }

    /// <summary>
    /// Replay 버튼 클릭 (전면 광고)
    /// </summary>
    private void OnReplayButtonClicked()
    {
        Debug.Log("[GamePlayManager] Replay 버튼 클릭 - 게임 재시작");
        
        // 버튼 클릭 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
#if PLAYSTORE_BUILD
        // 플레이스토어: Google AdMob 전면 광고
        ShowInterstitialAd(() => {
            InitializeGame();
        });
#elif INAPTOS_BUILD
        // 인앱토스: 인앱토스 IAA 전면 광고
        ShowInAptosInterstitialAd(() => {
            InitializeGame();
        });
#else
        // 개발 모드: 광고 없이 바로 재시작
        InitializeGame();
#endif
    }

    /// <summary>
    /// Exit 버튼 클릭
    /// </summary>
    private void OnExitButtonClicked()
    {
        Debug.Log("[GamePlayManager] Exit 버튼 클릭 - 메인 메뉴로 이동");
        
        // 버튼 클릭 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClickSound();
        }
        
        // 메인 메뉴로 씬 전환
        SceneManager.LoadScene("MainMenuScene");
    }

#if PLAYSTORE_BUILD
    // ============================================================
    // 플레이스토어 전용: Google AdMob 광고 메서드
    // ============================================================

    /// <summary>
    /// 보상형 광고 표시 (플레이스토어 전용)
    /// </summary>
    private void ShowRewardedAd(System.Action onAdCompleted)
    {
        Debug.Log("[GamePlayManager] 보상형 광고 표시");
        
        if (AdMobManager.Instance != null)
        {
            AdMobManager.Instance.ShowRewardedAd(onAdCompleted);
        }
        else
        {
            Debug.LogWarning("[GamePlayManager] AdMobManager가 없습니다 - 바로 콜백 실행");
            onAdCompleted?.Invoke();
        }
    }

    /// <summary>
    /// 전면 광고 표시 (플레이스토어 전용)
    /// </summary>
    private void ShowInterstitialAd(System.Action onAdCompleted)
    {
        Debug.Log("[GamePlayManager] 전면 광고 표시");
        
        if (AdMobManager.Instance != null)
        {
            AdMobManager.Instance.ShowInterstitialAd(onAdCompleted);
        }
        else
        {
            Debug.LogWarning("[GamePlayManager] AdMobManager가 없습니다 - 바로 콜백 실행");
            onAdCompleted?.Invoke();
        }
    }
#endif

#if INAPTOS_BUILD
    // ============================================================
    // 인앱토스 전용: 리더보드 및 광고 메서드
    // ============================================================

    /// <summary>
    /// 리더보드에 점수 제출 (인앱토스 전용)
    /// </summary>
    private void SubmitScoreToLeaderboard(int score)
    {
        Debug.Log($"[GamePlayManager] 리더보드에 점수 제출: {score} (TODO: 인앱토스 SDK 연동)");
        // TODO Phase 5: LeaderboardManager.Instance.SubmitScore(score);
    }

    /// <summary>
    /// 보상형 광고 표시 (인앱토스 전용)
    /// </summary>
    private void ShowInAptosRewardedAd(System.Action onAdCompleted)
    {
        Debug.Log("[GamePlayManager] 인앱토스 보상형 광고 표시 (TODO: 인앱토스 IAA 연동)");
        // TODO Phase 5: InAptosAdManager.Instance.ShowRewardedAd(onAdCompleted);
        
        // 임시: 바로 콜백 실행
        onAdCompleted?.Invoke();
    }

    /// <summary>
    /// 전면 광고 표시 (인앱토스 전용)
    /// </summary>
    private void ShowInAptosInterstitialAd(System.Action onAdCompleted)
    {
        Debug.Log("[GamePlayManager] 인앱토스 전면 광고 표시 (TODO: 인앱토스 IAA 연동)");
        // TODO Phase 5: InAptosAdManager.Instance.ShowInterstitialAd(onAdCompleted);
        
        // 임시: 바로 콜백 실행
        onAdCompleted?.Invoke();
    }
#endif
}
