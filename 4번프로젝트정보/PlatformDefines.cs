/// <summary>
/// 플랫폼별 빌드 설정 상수 정의
/// BuildConfigurationManager와 함께 사용됩니다.
/// </summary>
public static class PlatformDefines
{
    // Scripting Define Symbols
    public const string PLAYSTORE_BUILD = "PLAYSTORE_BUILD";
    public const string INAPTOS_BUILD = "INAPTOS_BUILD";

    /// <summary>
    /// 현재 플레이스토어 빌드인지 확인
    /// </summary>
    public static bool IsPlayStoreBuild()
    {
#if PLAYSTORE_BUILD
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// 현재 인앱토스 빌드인지 확인
    /// </summary>
    public static bool IsInAptosBuild()
    {
#if INAPTOS_BUILD
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// 현재 개발 모드인지 확인 (빌드 설정 없음)
    /// </summary>
    public static bool IsDevelopmentMode()
    {
#if !PLAYSTORE_BUILD && !INAPTOS_BUILD
        return true;
#else
        return false;
#endif
    }

    /// <summary>
    /// 현재 빌드 타입을 문자열로 반환
    /// </summary>
    public static string GetCurrentBuildType()
    {
#if PLAYSTORE_BUILD
        return "PlayStore (Android)";
#elif INAPTOS_BUILD
        return "InAptos (WebGL)";
#else
        return "Development Mode";
#endif
    }
}
