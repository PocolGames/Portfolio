using System;
using UnityEngine;
using GoogleMobileAds.Api;

/// <summary>
/// Google AdMob 광고를 관리하는 싱글톤 매니저 (Android 전용)
/// 배너, 전면, 보상형 광고를 제공합니다.
/// </summary>
public class AdMobManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static AdMobManager Instance { get; private set; }

    [Header("AdMob 광고 단위 ID (실제)")]
    [SerializeField] private string bannerAdUnitId = "ca-app-pub-1776903415677887/9367825683";
    [SerializeField] private string interstitialAdUnitId = "ca-app-pub-1776903415677887/5509645185";
    [SerializeField] private string rewardedAdUnitId = "ca-app-pub-1776903415677887/4306893083";

    // 광고 객체
    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;

    // 광고 로드 상태
    private bool isInterstitialAdReady = false;
    private bool isRewardedAdReady = false;

    // 보상형 광고 콜백
    private Action onRewardedAdCompleted;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // AdMob 초기화
            InitializeAdMob();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// AdMob 초기화
    /// </summary>
    private void InitializeAdMob()
    {
        Debug.Log("[AdMobManager] AdMob 초기화 시작...");
        
        // Mobile Ads SDK 초기화
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("[AdMobManager] AdMob 초기화 완료!");
            
            // 전면 광고 미리 로드
            LoadInterstitialAd();
            
            // 보상형 광고 미리 로드
            LoadRewardedAd();
        });
    }

    // ============================================================
    // 배너 광고
    // ============================================================

    /// <summary>
    /// 배너 광고 표시
    /// </summary>
    public void ShowBannerAd()
    {
        Debug.Log("[AdMobManager] 배너 광고 표시 요청");

        // 이미 배너가 있으면 제거
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        // 배너 광고 생성 (화면 하단)
        bannerView = new BannerView(bannerAdUnitId, AdSize.Banner, AdPosition.Bottom);

        // 이벤트 등록
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("[AdMobManager] 배너 광고 로드 완료");
        };

        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError($"[AdMobManager] 배너 광고 로드 실패: {error.GetMessage()}");
        };

        // 광고 요청
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    /// <summary>
    /// 배너 광고 숨기기
    /// </summary>
    public void HideBannerAd()
    {
        Debug.Log("[AdMobManager] 배너 광고 숨기기");

        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }

    // ============================================================
    // 전면 광고 (Interstitial Ad)
    // ============================================================

    /// <summary>
    /// 전면 광고 로드
    /// </summary>
    private void LoadInterstitialAd()
    {
        Debug.Log("[AdMobManager] 전면 광고 로드 시작...");

        // 기존 광고 제거
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        // 광고 요청
        AdRequest request = new AdRequest();

        InterstitialAd.Load(interstitialAdUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"[AdMobManager] 전면 광고 로드 실패: {error?.GetMessage()}");
                isInterstitialAdReady = false;
                return;
            }

            Debug.Log("[AdMobManager] 전면 광고 로드 완료");
            interstitialAd = ad;
            isInterstitialAdReady = true;

            // 이벤트 등록
            RegisterInterstitialAdEvents(ad);
        });
    }

    /// <summary>
    /// 전면 광고 이벤트 등록
    /// </summary>
    private void RegisterInterstitialAdEvents(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdMobManager] 전면 광고 닫힘");
            
            // 광고 닫힌 후 다음 광고 미리 로드
            LoadInterstitialAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"[AdMobManager] 전면 광고 표시 실패: {error.GetMessage()}");
            
            // 실패 시 다음 광고 미리 로드
            LoadInterstitialAd();
        };
    }

    /// <summary>
    /// 전면 광고 표시
    /// </summary>
    public void ShowInterstitialAd(Action onAdCompleted)
    {
        Debug.Log("[AdMobManager] 전면 광고 표시 요청");

        if (interstitialAd != null && isInterstitialAdReady)
        {
            interstitialAd.Show();
            
            // 광고 닫힌 후 콜백 실행
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                onAdCompleted?.Invoke();
            };
        }
        else
        {
            Debug.LogWarning("[AdMobManager] 전면 광고가 준비되지 않음 - 바로 콜백 실행");
            onAdCompleted?.Invoke();
            
            // 다시 로드 시도
            LoadInterstitialAd();
        }
    }

    // ============================================================
    // 보상형 광고 (Rewarded Ad)
    // ============================================================

    /// <summary>
    /// 보상형 광고 로드
    /// </summary>
    private void LoadRewardedAd()
    {
        Debug.Log("[AdMobManager] 보상형 광고 로드 시작...");

        // 기존 광고 제거
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        // 광고 요청
        AdRequest request = new AdRequest();

        RewardedAd.Load(rewardedAdUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"[AdMobManager] 보상형 광고 로드 실패: {error?.GetMessage()}");
                isRewardedAdReady = false;
                return;
            }

            Debug.Log("[AdMobManager] 보상형 광고 로드 완료");
            rewardedAd = ad;
            isRewardedAdReady = true;

            // 이벤트 등록
            RegisterRewardedAdEvents(ad);
        });
    }

    /// <summary>
    /// 보상형 광고 이벤트 등록
    /// </summary>
    private void RegisterRewardedAdEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[AdMobManager] 보상형 광고 닫힘");
            
            // 광고 닫힌 후 다음 광고 미리 로드
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"[AdMobManager] 보상형 광고 표시 실패: {error.GetMessage()}");
            
            // 실패 시 다음 광고 미리 로드
            LoadRewardedAd();
        };
    }

    /// <summary>
    /// 보상형 광고 표시
    /// </summary>
    public void ShowRewardedAd(Action onAdCompleted)
    {
        Debug.Log("[AdMobManager] 보상형 광고 표시 요청");

        // 콜백 저장
        onRewardedAdCompleted = onAdCompleted;

        if (rewardedAd != null && isRewardedAdReady)
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"[AdMobManager] 보상 지급: {reward.Type}, {reward.Amount}");
                
                // 보상 지급 (콜백 실행)
                onRewardedAdCompleted?.Invoke();
                onRewardedAdCompleted = null;
            });
        }
        else
        {
            Debug.LogWarning("[AdMobManager] 보상형 광고가 준비되지 않음 - 바로 콜백 실행");
            onRewardedAdCompleted?.Invoke();
            onRewardedAdCompleted = null;
            
            // 다시 로드 시도
            LoadRewardedAd();
        }
    }

    // ============================================================
    // 유틸리티
    // ============================================================

    /// <summary>
    /// 광고 준비 상태 확인
    /// </summary>
    public bool IsInterstitialAdReady()
    {
        return isInterstitialAdReady;
    }

    /// <summary>
    /// 보상형 광고 준비 상태 확인
    /// </summary>
    public bool IsRewardedAdReady()
    {
        return isRewardedAdReady;
    }

    private void OnDestroy()
    {
        // 모든 광고 정리
        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
    }
}
