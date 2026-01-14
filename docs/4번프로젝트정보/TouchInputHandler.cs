using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 터치 입력을 처리하는 핸들러
/// TouchPanel 버튼을 통해 슬롯 순차 정지를 제어합니다.
/// </summary>
public class TouchInputHandler : MonoBehaviour
{
    [Header("References")]
    [Tooltip("터치 패널 버튼")]
    [SerializeField] private Button touchPanel;

    [Tooltip("슬롯 스핀 컨트롤러")]
    [SerializeField] private SlotSpinController slotSpinController;

    // 터치 활성화 여부
    private bool isTouchEnabled = false;

    private void Awake()
    {
        if (touchPanel != null)
        {
            touchPanel.onClick.AddListener(OnTouchPanelClicked);
        }
    }

    private void OnDestroy()
    {
        if (touchPanel != null)
        {
            touchPanel.onClick.RemoveListener(OnTouchPanelClicked);
        }
    }

    /// <summary>
    /// 터치 입력 활성화
    /// </summary>
    public void EnableTouch()
    {
        isTouchEnabled = true;
        // Debug.Log("[TouchInputHandler] 터치 입력 활성화");
    }

    /// <summary>
    /// 터치 입력 비활성화
    /// </summary>
    public void DisableTouch()
    {
        isTouchEnabled = false;
        // Debug.Log("[TouchInputHandler] 터치 입력 비활성화");
    }

    /// <summary>
    /// 터치 패널 클릭 이벤트
    /// </summary>
    private void OnTouchPanelClicked()
    {
        if (!isTouchEnabled) return;
        if (slotSpinController == null) return;
        if (!slotSpinController.IsSpinning) return;

        // 터치 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTouchSound();
        }

        // 다음 슬롯 정지 요청
        slotSpinController.StopNextSlot();

        // Debug.Log($"[TouchInputHandler] 터치 감지 - 슬롯 정지 요청 (정지된 슬롯: {slotSpinController.StoppedSlotCount})");

        // 3개 모두 정지되면 터치 비활성화
        if (slotSpinController.StoppedSlotCount >= 3)
        {
            DisableTouch();
        }
    }
}
