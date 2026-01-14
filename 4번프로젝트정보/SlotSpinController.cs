using UnityEngine;
using System;

/// <summary>
/// 3개의 슬롯 컬럼을 통합 관리하는 컨트롤러
/// 스핀 시작, 순차 정지, 매칭 판정을 담당합니다.
/// </summary>
public class SlotSpinController : MonoBehaviour
{
    [Header("Slot Columns")]
    [Tooltip("3개의 슬롯 컬럼 (왼쪽부터 0, 1, 2)")]
    [SerializeField] private SlotColumn[] slotColumns = new SlotColumn[3];

    [Header("Spin Settings")]
    [Tooltip("초기 스핀 속도 (한 칸 이동 시간)")]
    [SerializeField] private float initialSpinDuration = 0.5f;

    // 현재 스핀 속도
    private float currentSpinDuration;
    
    // 정지된 슬롯 수 (0~3)
    private int stoppedSlotCount = 0;
    
    // 각 슬롯의 최종 결과 (0~4)
    private int[] slotResults = new int[3];
    
    // 스핀 상태
    private bool isSpinning = false;
    
    // 스핀 완료 처리 중 플래그 (중복 호출 방지)
    private bool isCompletingspin = false;

    /// <summary>
    /// 스핀 중인지 여부
    /// </summary>
    public bool IsSpinning => isSpinning;

    /// <summary>
    /// 정지된 슬롯 수
    /// </summary>
    public int StoppedSlotCount => stoppedSlotCount;

    // 이벤트 콜백
    public event Action OnSpinStarted;
    public event Action<int> OnSlotStopped; // 정지된 슬롯 인덱스 전달
    public event Action<bool, int[]> OnSpinCompleted; // (매칭 성공 여부, 슬롯 결과)

    private void Awake()
    {
        currentSpinDuration = initialSpinDuration;
    }

    /// <summary>
    /// 게임 시작 시 슬롯 초기화
    /// </summary>
    public void Initialize()
    {
        foreach (var column in slotColumns)
        {
            column.Initialize();
        }
    }

    /// <summary>
    /// 모든 슬롯 스핀 시작
    /// </summary>
    public void StartSpin()
    {
        if (isSpinning) return;

        isSpinning = true;
        isCompletingspin = false;
        stoppedSlotCount = 0;
        slotResults = new int[3];

        // Debug.Log("[SlotSpinController] 스핀 시작");

        // 3개의 슬롯 모두 스핀 시작
        foreach (var column in slotColumns)
        {
            column.StartSpin(currentSpinDuration);
        }

        OnSpinStarted?.Invoke();
    }

    /// <summary>
    /// 슬롯 순차 정지 (터치 입력 시 호출)
    /// </summary>
    public void StopNextSlot()
    {
        if (!isSpinning) return;
        if (stoppedSlotCount >= 3) return;

        int slotIndex = stoppedSlotCount;

        // 현재 슬롯 정지
        slotColumns[slotIndex].StopSpin(() =>
        {
            // 정지 완료 후 결과 저장
            slotResults[slotIndex] = slotColumns[slotIndex].CurrentCenterFoodIndex;
            stoppedSlotCount++;

            // Debug.Log($"[SlotSpinController] 슬롯 {slotIndex} 정지 완료 - 결과: {slotResults[slotIndex]}, 정지된 슬롯: {stoppedSlotCount}/3");

            OnSlotStopped?.Invoke(slotIndex);

            // 매칭 체크
            bool shouldContinue = CheckMatching(slotIndex);

            if (!shouldContinue)
            {
                // 매칭 실패 - 남은 슬롯이 있으면 즉시 정지
                if (stoppedSlotCount < 3)
                {
                    StopAllSlotsImmediately();
                }
                else
                {
                    // 이미 3개 모두 정지됨 - 바로 결과 처리
                    Debug.Log("[SlotSpinController] 모든 슬롯 이미 정지됨 - 즉시 결과 처리");
                    CompleteSpin(false);
                }
            }
            else if (stoppedSlotCount >= 3)
            {
                // 3개 모두 정지 완료 - 최종 결과 처리
                CompleteSpin(true);
            }
        });
    }

    /// <summary>
    /// 매칭 체크 (조기 종료 판정)
    /// </summary>
    /// <param name="stoppedIndex">방금 정지된 슬롯 인덱스</param>
    /// <returns>계속 진행 여부</returns>
    private bool CheckMatching(int stoppedIndex)
    {
        // 첫 번째 슬롯은 항상 통과
        if (stoppedIndex == 0)
            return true;

        // 두 번째 슬롯: 첫 번째와 비교
        if (stoppedIndex == 1)
        {
            if (slotResults[0] != slotResults[1])
            {
                Debug.Log($"[SlotSpinController] 매칭 실패: 슬롯0({slotResults[0]}) != 슬롯1({slotResults[1]})");
                return false;
            }
            return true;
        }

        // 세 번째 슬롯: 첫 번째, 두 번째와 비교
        if (stoppedIndex == 2)
        {
            if (slotResults[0] != slotResults[2])
            {
                Debug.Log($"[SlotSpinController] 매칭 실패: 슬롯0({slotResults[0]}) != 슬롯2({slotResults[2]})");
                return false;
            }
            return true;
        }

        return true;
    }

    /// <summary>
    /// 모든 슬롯 즉시 정지 (매칭 실패 시)
    /// </summary>
    private void StopAllSlotsImmediately()
    {
        // Debug.Log($"[SlotSpinController] 즉시 모든 슬롯 정지 시작 - 현재 정지된 슬롯: {stoppedSlotCount}");
        
        // 남은 슬롯들을 정지시킴
        int remainingSlotsToStop = slotColumns.Length - stoppedSlotCount;
        int expectedFinalCount = slotColumns.Length;
        
        for (int i = stoppedSlotCount; i < slotColumns.Length; i++)
        {
            int finalIndex = i;
            slotColumns[i].StopSpin(() =>
            {
                slotResults[finalIndex] = slotColumns[finalIndex].CurrentCenterFoodIndex;
                stoppedSlotCount++;

                // Debug.Log($"[SlotSpinController] 슬롯 {finalIndex} 정지 완료 - 현재: {stoppedSlotCount}/{expectedFinalCount}");
                OnSlotStopped?.Invoke(finalIndex);

                // 마지막 슬롯까지 정지되면 결과 처리 (한 번만 호출)
                if (stoppedSlotCount >= expectedFinalCount)
                {
                    // Debug.Log($"[SlotSpinController] 모든 슬롯 정지 완료 - 결과 처리 시작");
                    CompleteSpin(false);
                }
            });
        }
    }

    /// <summary>
    /// 스핀 완료 처리
    /// </summary>
    /// <param name="isMatched">3개 매칭 성공 여부</param>
    private void CompleteSpin(bool isMatched)
    {
        // 중복 호출 방지
        if (isCompletingspin)
        {
            // Debug.LogWarning("[SlotSpinController] CompleteSpin 중복 호출 방지!");
            return;
        }

        isCompletingspin = true;
        isSpinning = false;

        // Debug.Log($"[SlotSpinController] 스핀 완료 - 매칭: {isMatched}, 결과: [{slotResults[0]}, {slotResults[1]}, {slotResults[2]}]");

        OnSpinCompleted?.Invoke(isMatched, slotResults);
    }

    /// <summary>
    /// 스핀 속도 업데이트 (난이도 조절)
    /// </summary>
    /// <param name="totalScore">현재 총 점수</param>
    public void UpdateSpinSpeed(int totalScore)
    {
        // 난이도 증가 공식: spinDuration = Mathf.Clamp(0.5f - totalScore * 0.02f, 0.1f, 0.5f)
        // 점수 1점당 0.02초 감소 (20점에 최고 난이도 도달)
        currentSpinDuration = Mathf.Clamp(0.5f - totalScore * 0.02f, 0.1f, 0.5f);
        // Debug.Log($"[SlotSpinController] 스핀 속도 업데이트: {currentSpinDuration}초 (점수: {totalScore})");
    }

    /// <summary>
    /// 현재 스핀 속도 가져오기
    /// </summary>
    public float GetCurrentSpinDuration()
    {
        return currentSpinDuration;
    }

    /// <summary>
    /// 특정 슬롯의 결과 가져오기
    /// </summary>
    public int GetSlotResult(int index)
    {
        if (index >= 0 && index < slotResults.Length)
        {
            return slotResults[index];
        }
        return 0;
    }
}
