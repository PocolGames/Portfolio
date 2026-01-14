using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using System.Linq;

/// <summary>
/// 플랫폼별 빌드 설정을 관리하는 에디터 확장
/// Android (플레이스토어)와 WebGL (인앱토스) 간 전환을 자동화합니다.
/// </summary>
public class BuildConfigurationManager : EditorWindow
{
    // 플랫폼별 Scripting Define Symbols
    private const string PLAYSTORE_SYMBOL = "PLAYSTORE_BUILD";
    private const string INAPTOS_SYMBOL = "INAPTOS_BUILD";

    [MenuItem("Tools/Build Configuration/Switch to PlayStore Build (Android)")]
    public static void SwitchToPlayStore()
    {
        Debug.Log("=== 플레이스토어 빌드 설정 전환 시작 ===");
        
        // 1. 빌드 타겟을 Android로 변경
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        
        // 2. Scripting Define Symbols 설정
        SetScriptingDefineSymbols(NamedBuildTarget.Android, PLAYSTORE_SYMBOL);
        
        // 3. PlayerSettings 설정
        ConfigurePlayStoreSettings();
        
        Debug.Log("✅ 플레이스토어 빌드 설정 완료!");
        Debug.Log($"   - Build Target: Android");
        Debug.Log($"   - Define Symbol: {PLAYSTORE_SYMBOL}");
        
        // 사용자에게 알림
        EditorUtility.DisplayDialog(
            "빌드 설정 완료", 
            "플레이스토어 (Android) 빌드 설정이 완료되었습니다.\n\n" +
            "적용된 설정:\n" +
            $"- Build Target: Android\n" +
            $"- Define Symbol: {PLAYSTORE_SYMBOL}\n" +
            "- Google AdMob: 활성화\n" +
            "- 인앱토스 SDK: 비활성화", 
            "확인"
        );
    }

    [MenuItem("Tools/Build Configuration/Switch to InAptos Build (WebGL)")]
    public static void SwitchToInAptos()
    {
        Debug.Log("=== 인앱토스 빌드 설정 전환 시작 ===");
        
        // 1. 빌드 타겟을 WebGL로 변경
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        
        // 2. Scripting Define Symbols 설정
        SetScriptingDefineSymbols(NamedBuildTarget.WebGL, INAPTOS_SYMBOL);
        
        // 3. PlayerSettings 설정
        ConfigureInAptosSettings();
        
        Debug.Log("✅ 인앱토스 빌드 설정 완료!");
        Debug.Log($"   - Build Target: WebGL");
        Debug.Log($"   - Define Symbol: {INAPTOS_SYMBOL}");
        
        // 사용자에게 알림
        EditorUtility.DisplayDialog(
            "빌드 설정 완료", 
            "인앱토스 (WebGL) 빌드 설정이 완료되었습니다.\n\n" +
            "적용된 설정:\n" +
            $"- Build Target: WebGL\n" +
            $"- Define Symbol: {INAPTOS_SYMBOL}\n" +
            "- Google AdMob: 비활성화\n" +
            "- 인앱토스 SDK: 활성화\n" +
            "- Compression: Brotli\n" +
            "- Code Optimization: IL2CPP", 
            "확인"
        );
    }

    /// <summary>
    /// Scripting Define Symbols 설정 (Unity 6 호환)
    /// </summary>
    private static void SetScriptingDefineSymbols(NamedBuildTarget buildTarget, string newSymbol)
    {
        // 현재 설정된 심볼 가져오기 (Unity 6 API)
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbols(buildTarget);
        
        // 세미콜론으로 분리하여 리스트로 변환
        var symbolList = currentSymbols.Split(';').ToList();
        
        // 기존 빌드 심볼 제거
        symbolList.Remove(PLAYSTORE_SYMBOL);
        symbolList.Remove(INAPTOS_SYMBOL);
        
        // 새 심볼 추가
        if (!symbolList.Contains(newSymbol))
        {
            symbolList.Add(newSymbol);
        }
        
        // 빈 문자열 제거
        symbolList.RemoveAll(s => string.IsNullOrWhiteSpace(s));
        
        // 다시 세미콜론으로 결합
        string newSymbols = string.Join(";", symbolList);
        
        // 설정 적용 (Unity 6 API)
        PlayerSettings.SetScriptingDefineSymbols(buildTarget, newSymbols);
        
        Debug.Log($"[BuildConfigurationManager] Scripting Define Symbols 설정 완료: {newSymbols}");
    }

    /// <summary>
    /// 플레이스토어 (Android) 빌드 설정
    /// </summary>
    private static void ConfigurePlayStoreSettings()
    {
        Debug.Log("[BuildConfigurationManager] 플레이스토어 PlayerSettings 적용 중...");
        
        // Android 기본 설정
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25; // Android 7.1 (Unity 6 최소 요구사항)
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel34; // Android 14
        
        // 그래픽 설정
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new UnityEngine.Rendering.GraphicsDeviceType[] 
        {
            UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3,
            UnityEngine.Rendering.GraphicsDeviceType.Vulkan
        });
        
        // IL2CPP 설정 (Unity 6 API)
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        
        Debug.Log("[BuildConfigurationManager] 플레이스토어 설정 완료");
    }

    /// <summary>
    /// 인앱토스 (WebGL) 빌드 설정
    /// </summary>
    private static void ConfigureInAptosSettings()
    {
        Debug.Log("[BuildConfigurationManager] 인앱토스 PlayerSettings 적용 중...");
        
        // WebGL 압축 설정 (Brotli - 최고 압축률)
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        
        // WebGL 메모리 설정 (512MB - 인앱토스 권장사항)
        PlayerSettings.WebGL.memorySize = 512;
        
        // 예외 처리 설정 (성능 최적화)
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
        
        // WebGL 템플릿 (기본 템플릿 사용)
        PlayerSettings.WebGL.template = "APPLICATION:Default";
        
        // 코드 최적화 설정 (Unity 6 API)
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.WebGL, ScriptingImplementation.IL2CPP);
        
        // Unity 6에서는 WebGL 2.0이 기본값이므로 별도 설정 불필요
        
        // 데이터 캐싱 활성화
        PlayerSettings.WebGL.dataCaching = true;
        
        Debug.Log("[BuildConfigurationManager] 인앱토스 설정 완료");
    }

    /// <summary>
    /// 현재 빌드 설정 확인 메뉴
    /// </summary>
    [MenuItem("Tools/Build Configuration/Show Current Configuration")]
    public static void ShowCurrentConfiguration()
    {
        BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(currentBuildTarget));
        
        // Unity 6 API 사용
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
        
        string configInfo = "=== 현재 빌드 설정 ===\n\n";
        configInfo += $"빌드 타겟: {currentBuildTarget}\n";
        configInfo += $"Named 빌드 타겟: {namedBuildTarget}\n";
        configInfo += $"Scripting Define Symbols:\n{currentSymbols}\n\n";
        
        // 어떤 빌드 모드인지 판단
        if (currentSymbols.Contains(PLAYSTORE_SYMBOL))
        {
            configInfo += "✅ 현재 모드: 플레이스토어 빌드 (Android)\n";
            configInfo += "   - Google AdMob: 활성화\n";
            configInfo += "   - 인앱토스 SDK: 비활성화";
        }
        else if (currentSymbols.Contains(INAPTOS_SYMBOL))
        {
            configInfo += "✅ 현재 모드: 인앱토스 빌드 (WebGL)\n";
            configInfo += "   - Google AdMob: 비활성화\n";
            configInfo += "   - 인앱토스 SDK: 활성화";
        }
        else
        {
            configInfo += "⚠️ 빌드 설정이 지정되지 않았습니다.\n";
            configInfo += "   Tools > Build Configuration에서 빌드 모드를 선택하세요.";
        }
        
        Debug.Log(configInfo);
        EditorUtility.DisplayDialog("현재 빌드 설정", configInfo, "확인");
    }

    /// <summary>
    /// 빌드 설정 초기화 (개발 모드)
    /// </summary>
    [MenuItem("Tools/Build Configuration/Reset to Development Mode")]
    public static void ResetToDevelopmentMode()
    {
        Debug.Log("=== 빌드 설정 초기화 (개발 모드) ===");
        
        // 현재 타겟 가져오기
        BuildTarget currentBuildTarget = EditorUserBuildSettings.activeBuildTarget;
        NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(currentBuildTarget));
        
        // Unity 6 API 사용
        string currentSymbols = PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget);
        var symbolList = currentSymbols.Split(';').ToList();
        symbolList.Remove(PLAYSTORE_SYMBOL);
        symbolList.Remove(INAPTOS_SYMBOL);
        symbolList.RemoveAll(s => string.IsNullOrWhiteSpace(s));
        
        string newSymbols = string.Join(";", symbolList);
        PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, newSymbols);
        
        Debug.Log("✅ 빌드 설정 초기화 완료!");
        Debug.Log($"   - 모든 빌드 심볼 제거됨");
        
        EditorUtility.DisplayDialog(
            "초기화 완료", 
            "빌드 설정이 개발 모드로 초기화되었습니다.\n\n" +
            "모든 플랫폼 심볼이 제거되었습니다.\n" +
            "개발 중에는 이 모드를 사용하세요.", 
            "확인"
        );
    }
}
