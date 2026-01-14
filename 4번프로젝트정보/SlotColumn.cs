using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// 개별 슬롯 컬럼을 관리하는 클래스
/// 5개의 이미지 슬롯(0~4)을 관리하며, 위에서 아래로 스핀 애니메이션을 제어합니다.
/// </summary>
public class SlotColumn : MonoBehaviour
{
    [Header("Slot Images")]
    [Tooltip("5개의 슬롯 이미지 (0~4), 인덱스 2번이 중앙")]
    [SerializeField] private Image[] slotImages = new Image[5];

    [Header("Food Sprites")]
    [Tooltip("5가지 음식 스프라이트")]
    [SerializeField] private Sprite[] foodSprites = new Sprite[5];

    [Header("Animation Settings")]
    [Tooltip("한 칸 이동하는데 걸리는 거리 (슬롯 간 간격과 동일해야 함)")]
    [SerializeField] private float moveDistance = 256f; // 한 칸 이동 거리

    // 현재 중앙(인덱스 2)에 표시되는 음식 인덱스 (0~4)
    private int currentCenterFoodIndex;
    
    // 각 슬롯 위치의 원래 Y 좌표
    private float[] originalYPositions = new float[5];
    
    // 스핀 중인지 여부
    private bool isSpinning = false;
    
    // 정지 요청 플래그 (OnComplete에서 다음 사이클 실행 방지)
    private bool stopRequested = false;
    
    // 정지 완료 콜백 저장
    private Action pendingStopCallback = null;
    
    // 스핀 애니메이션 Sequence
    private Sequence spinSequence;
    
    // 현재 스핀 속도
    private float currentSpinDuration = 0.5f;

    /// <summary>
    /// 현재 중앙에 표시된 음식 인덱스 (0~4)
    /// </summary>
    public int CurrentCenterFoodIndex => currentCenterFoodIndex;

    /// <summary>
    /// 스핀 중인지 여부
    /// </summary>
    public bool IsSpinning => isSpinning;

    /// <summary>
    /// 중앙 이미지 위치 가져오기 (애니메이션용)
    /// </summary>
    public Vector3 GetCenterImagePosition()
    {
        if (slotImages[2] != null)
        {
            return slotImages[2].transform.position;
        }
        return Vector3.zero;
    }

    private void Awake()
    {
        // 각 슬롯의 원래 Y 위치 저장
        for (int i = 0; i < slotImages.Length; i++)
        {
            originalYPositions[i] = slotImages[i].rectTransform.anchoredPosition.y;
        }
    }

    /// <summary>
    /// 슬롯 초기화 - 중복 없이 5개 음식을 섞어서 할당
    /// </summary>
    public void Initialize()
    {
        // 5개의 음식 인덱스를 담은 리스트 생성 (0, 1, 2, 3, 4)
        System.Collections.Generic.List<int> foodIndices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < foodSprites.Length; i++)
        {
            foodIndices.Add(i);
        }

        // Fisher-Yates 셔플 알고리즘으로 섞기
        for (int i = foodIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            int temp = foodIndices[i];
            foodIndices[i] = foodIndices[randomIndex];
            foodIndices[randomIndex] = temp;
        }

        // 섞인 순서대로 슬롯에 할당
        for (int i = 0; i < slotImages.Length; i++)
        {
            slotImages[i].sprite = foodSprites[foodIndices[i]];
        }

        // 중앙(인덱스 2) 슬롯의 음식 인덱스를 현재 인덱스로 설정
        currentCenterFoodIndex = GetFoodIndexFromSprite(slotImages[2].sprite);
        
        // Debug.Log($"[SlotColumn] 초기화 완료 - 슬롯 순서: [{foodIndices[0]}, {foodIndices[1]}, {foodIndices[2]}, {foodIndices[3]}, {foodIndices[4]}]");
    }

    /// <summary>
    /// 스핀 시작
    /// </summary>
    /// <param name="spinDuration">한 칸 이동하는데 걸리는 시간</param>
    public void StartSpin(float spinDuration)
    {
        if (isSpinning) return;

        currentSpinDuration = spinDuration;
        isSpinning = true;
        stopRequested = false;
        pendingStopCallback = null;
        
        SpinCycle();
    }

    /// <summary>
    /// 스핀 정지 (현재 사이클 완료 후)
    /// </summary>
    /// <param name="onStopComplete">정지 완료 시 콜백</param>
    public void StopSpin(Action onStopComplete = null)
    {
        if (!isSpinning)
        {
            Debug.LogWarning("[SlotColumn] 이미 정지된 상태입니다.");
            onStopComplete?.Invoke();
            return;
        }

        // Debug.Log($"[SlotColumn] 정지 요청 - 현재 중앙 음식: {currentCenterFoodIndex}");
        
        stopRequested = true;
        pendingStopCallback = onStopComplete;
        
        // isSpinning은 OnComplete에서 false로 설정됨
    }

    /// <summary>
    /// 스핀 1사이클 실행
    /// - 위치: 0→1→2→3→4 (위에서 아래로 이동)
    /// - 이미지: 4→0, 3→4, 2→3, 1→2, 0→1 (아래에서 위로 순환)
    /// - 원위치 복귀
    /// - 무한 반복 (isSpinning이 true인 경우)
    /// </summary>
    private void SpinCycle()
    {
        if (spinSequence != null && spinSequence.IsActive())
        {
            spinSequence.Kill();
        }

        spinSequence = DOTween.Sequence();

        // 1. 모든 슬롯을 아래로 한 칸씩 이동 (0→1, 1→2, 2→3, 3→4)
        for (int i = 0; i < slotImages.Length; i++)
        {
            RectTransform rect = slotImages[i].rectTransform;
            Vector2 currentPos = rect.anchoredPosition;
            Vector2 targetPos = new Vector2(currentPos.x, currentPos.y - moveDistance);
            
            spinSequence.Join(rect.DOAnchorPos(targetPos, currentSpinDuration).SetEase(Ease.Linear));
        }

        // 2. 이동 완료 후 이미지 교체 및 원위치 (OnComplete 사용)
        spinSequence.OnComplete(() =>
        {
            // 이미지 순환 (아래에서 위로: 4→0, 3→4, 2→3, 1→2, 0→1)
            // 4번(맨 아래) 이미지가 0번(맨 위)으로 올라감
            Sprite temp = slotImages[4].sprite;
            for (int i = 4; i > 0; i--)
            {
                slotImages[i].sprite = slotImages[i - 1].sprite;
            }
            slotImages[0].sprite = temp;

            // 모든 슬롯을 원위치로 즉시 복귀
            for (int i = 0; i < slotImages.Length; i++)
            {
                slotImages[i].rectTransform.anchoredPosition = new Vector2(
                    slotImages[i].rectTransform.anchoredPosition.x,
                    originalYPositions[i]
                );
            }

            // 중앙 슬롯의 현재 음식 인덱스 업데이트
            int previousIndex = currentCenterFoodIndex;
            currentCenterFoodIndex = GetFoodIndexFromSprite(slotImages[2].sprite);
            
            // 디버깅: 중앙 슬롯 변화 로그
            // Debug.Log($"[SlotColumn] 사이클 완료 - 중앙: {previousIndex} → {currentCenterFoodIndex}");

            // 정지 요청이 있었는지 확인
            if (stopRequested)
            {
                isSpinning = false;
                stopRequested = false;
                
                // Debug.Log($"[SlotColumn] 정지 완료 - 최종 중앙 음식: {currentCenterFoodIndex}");
                
                // 저장된 콜백 실행
                Action callback = pendingStopCallback;
                pendingStopCallback = null;
                callback?.Invoke();
            }
            // 스핀 중이면 다음 사이클 실행
            else if (isSpinning)
            {
                SpinCycle();
            }
        });
    }

    /// <summary>
    /// Sprite로부터 음식 인덱스 찾기
    /// </summary>
    private int GetFoodIndexFromSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogError("[SlotColumn] Sprite가 null입니다!");
            return 0;
        }

        for (int i = 0; i < foodSprites.Length; i++)
        {
            if (foodSprites[i] == sprite)
            {
                return i;
            }
        }
        
        Debug.LogError($"[SlotColumn] 일치하는 음식 Sprite를 찾을 수 없습니다: {sprite.name}");
        return 0; // 기본값
    }

    /// <summary>
    /// 정리 작업
    /// </summary>
    private void OnDestroy()
    {
        if (spinSequence != null && spinSequence.IsActive())
        {
            spinSequence.Kill();
        }
    }
}
