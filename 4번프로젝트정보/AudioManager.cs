using UnityEngine;

/// <summary>
/// 게임 전체의 사운드를 관리하는 매니저 (싱글톤)
/// BGM과 SFX를 재생하고 볼륨을 조절합니다.
/// </summary>
public class AudioManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [Tooltip("배경 음악용 AudioSource")]
    [SerializeField] private AudioSource bgmSource;

    [Tooltip("효과음용 AudioSource")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Background Music")]
    [Tooltip("메인 메뉴 BGM")]
    [SerializeField] private AudioClip mainMenuBGM;

    [Tooltip("게임 플레이 BGM")]
    [SerializeField] private AudioClip gamePlayBGM;

    [Header("Sound Effects")]
    [Tooltip("터치 사운드 (슬롯 정지)")]
    [SerializeField] private AudioClip touchSound;

    [Tooltip("매칭 성공 사운드")]
    [SerializeField] private AudioClip matchSuccessSound;

    [Tooltip("매칭 실패 사운드")]
    [SerializeField] private AudioClip matchFailSound;

    [Tooltip("꼬치 완성 사운드")]
    [SerializeField] private AudioClip skewerCompleteSound;

    [Tooltip("게임 오버 사운드")]
    [SerializeField] private AudioClip gameOverSound;

    [Tooltip("버튼 클릭 사운드")]
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Volume Settings")]
    [Tooltip("BGM 볼륨 (0~1)")]
    [SerializeField] private float bgmVolume = 0.5f;

    [Tooltip("SFX 볼륨 (0~1)")]
    [SerializeField] private float sfxVolume = 0.7f;

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
            Debug.Log("[AudioManager] AudioManager 싱글톤 생성");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("[AudioManager] 중복된 AudioManager 제거");
            return;
        }

        // AudioSource 컴포넌트 확인 및 생성
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true; // BGM은 반복 재생
            Debug.Log("[AudioManager] BGM AudioSource 자동 생성");
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false; // SFX는 한 번만 재생
            Debug.Log("[AudioManager] SFX AudioSource 자동 생성");
        }

        // 초기 볼륨 설정
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    #region BGM Control

    /// <summary>
    /// 메인 메뉴 BGM 재생
    /// </summary>
    public void PlayMainMenuBGM()
    {
        PlayBGM(mainMenuBGM);
    }

    /// <summary>
    /// 게임 플레이 BGM 재생
    /// </summary>
    public void PlayGamePlayBGM()
    {
        PlayBGM(gamePlayBGM);
    }

    /// <summary>
    /// BGM 재생
    /// </summary>
    private void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] BGM 클립이 null입니다!");
            return;
        }

        // 이미 같은 클립이 재생 중이면 무시
        if (bgmSource.isPlaying && bgmSource.clip == clip)
        {
            return;
        }

        bgmSource.clip = clip;
        bgmSource.Play();
        Debug.Log($"[AudioManager] BGM 재생: {clip.name}");
    }

    /// <summary>
    /// BGM 정지
    /// </summary>
    public void StopBGM()
    {
        bgmSource.Stop();
        Debug.Log("[AudioManager] BGM 정지");
    }

    /// <summary>
    /// BGM 볼륨 설정
    /// </summary>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
        Debug.Log($"[AudioManager] BGM 볼륨 설정: {bgmVolume}");
    }

    #endregion

    #region SFX Control

    /// <summary>
    /// 터치 사운드 재생 (슬롯 정지)
    /// </summary>
    public void PlayTouchSound()
    {
        PlaySFX(touchSound);
    }

    /// <summary>
    /// 매칭 성공 사운드 재생
    /// </summary>
    public void PlayMatchSuccessSound()
    {
        PlaySFX(matchSuccessSound);
    }

    /// <summary>
    /// 매칭 실패 사운드 재생
    /// </summary>
    public void PlayMatchFailSound()
    {
        PlaySFX(matchFailSound);
    }

    /// <summary>
    /// 꼬치 완성 사운드 재생
    /// </summary>
    public void PlaySkewerCompleteSound()
    {
        PlaySFX(skewerCompleteSound);
    }

    /// <summary>
    /// 게임 오버 사운드 재생
    /// </summary>
    public void PlayGameOverSound()
    {
        PlaySFX(gameOverSound);
    }

    /// <summary>
    /// 버튼 클릭 사운드 재생
    /// </summary>
    public void PlayButtonClickSound()
    {
        PlaySFX(buttonClickSound);
    }

    /// <summary>
    /// SFX 재생
    /// </summary>
    private void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] SFX 클립이 null입니다!");
            return;
        }

        sfxSource.PlayOneShot(clip);
        // Debug.Log($"[AudioManager] SFX 재생: {clip.name}");
    }

    /// <summary>
    /// SFX 볼륨 설정
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume;
        Debug.Log($"[AudioManager] SFX 볼륨 설정: {sfxVolume}");
    }

    #endregion

    #region Volume Control

    /// <summary>
    /// 현재 BGM 볼륨 가져오기
    /// </summary>
    public float GetBGMVolume()
    {
        return bgmVolume;
    }

    /// <summary>
    /// 현재 SFX 볼륨 가져오기
    /// </summary>
    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    #endregion
}
