using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// 매칭 성공 시 음식이 꼬치로 날아가는 애니메이션 관리
/// 런타임에 음식 이미지를 동적으로 생성하여 이동시킵니다.
/// </summary>
public class FoodAnimationController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Canvas Transform")]
    [SerializeField] private Canvas canvas;

    [Tooltip("3개의 슬롯 컬럼")]
    [SerializeField] private SlotColumn[] slotColumns = new SlotColumn[3];

    [Tooltip("하단 꼬치 슬롯 (3개)")]
    [SerializeField] private Image[] bottomSlots = new Image[3];

    [Tooltip("5가지 음식 스프라이트")]
    [SerializeField] private Sprite[] foodSprites = new Sprite[5];

    [Header("Animation Settings")]
    [Tooltip("이동 시간")]
    [SerializeField] private float moveDuration = 0.7f;

    [Tooltip("포물선 높이 최소값")]
    [SerializeField] private float arcHeightMin = 100f;

    [Tooltip("포물선 높이 최대값")]
    [SerializeField] private float arcHeightMax = 200f;

    [Tooltip("좌우 흔들림 범위")]
    [SerializeField] private float horizontalOffsetRange = 50f;

    [Header("Completion Animation Settings")]
    [Tooltip("꼬치 완성 시 크기 배율")]
    [SerializeField] private float completionScaleMultiplier = 1.2f;

    [Tooltip("꼬치 완성 시 커지는 시간")]
    [SerializeField] private float scaleUpDuration = 0.3f;

    [Tooltip("꼬치 완성 시 원래대로 돌아오는 시간")]
    [SerializeField] private float scaleDownDuration = 0.2f;

    // 꼬치 스택 카운트 (0~2)
    private int skewerStackCount = 0;

    /// <summary>
    /// 매칭 성공 애니메이션 재생
    /// </summary>
    /// <param name="foodIndices">매칭된 음식 인덱스 배열 (3개)</param>
    /// <param name="onComplete">애니메이션 완료 콜백</param>
    public void PlayMatchSuccessAnimation(int[] foodIndices, Action onComplete)
    {
        if (foodIndices == null || foodIndices.Length != 3)
        {
            Debug.LogError("[FoodAnimationController] foodIndices는 3개여야 합니다!");
            onComplete?.Invoke();
            return;
        }

        Debug.Log($"[FoodAnimationController] 매칭 성공 애니메이션 시작 - 음식: [{foodIndices[0]}, {foodIndices[1]}, {foodIndices[2]}]");

        // 3개 음식을 동시에 이동 (마지막 하나만 콜백 연결)
        for (int i = 0; i < 2; i++)
        {
            MoveFoodToSkewer(i, foodIndices[i], null);
        }

        // 마지막 음식에만 완료 콜백 연결
        MoveFoodToSkewer(2, foodIndices[2], () =>
        {
            Debug.Log($"[FoodAnimationController] 모든 음식 이동 완료 - 현재 스택: {skewerStackCount}/3");

            // 3개 완성 체크
            if (skewerStackCount >= 3)
            {
                Debug.Log("[FoodAnimationController] 꼬치 3개 완성! 크기 애니메이션 시작");
                
                // 꼬치 완성 애니메이션 재생 후 콜백 실행
                PlayCompletionAnimation(() =>
                {
                    // 3개 이후부터는 순환 모드 (0, 1, 2, 0, 1, 2, ...)
                    skewerStackCount = 0;
                    onComplete?.Invoke();
                });
            }
            else
            {
                // 아직 3개가 안 채워졌으면 바로 콜백 실행
                onComplete?.Invoke();
            }
        });
    }

    /// <summary>
    /// 꼬치 완성 시 크기 애니메이션 (3개 모두 동시에 커졌다 작아지기)
    /// </summary>
    /// <param name="onComplete">애니메이션 완료 콜백</param>
    private void PlayCompletionAnimation(Action onComplete)
    {
        Debug.Log("[FoodAnimationController] 꼬치 완성 애니메이션 재생");

        // 꼬치 완성 사운드 재생
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySkewerCompleteSound();
        }

        // Sequence 생성
        Sequence completionSequence = DOTween.Sequence();

        // 3개의 bottomSlot을 동시에 크기 애니메이션
        for (int i = 0; i < bottomSlots.Length; i++)
        {
            if (bottomSlots[i] != null)
            {
                RectTransform rect = bottomSlots[i].GetComponent<RectTransform>();
                Vector3 originalScale = Vector3.one; // 원래 크기
                Vector3 targetScale = Vector3.one * completionScaleMultiplier; // 1.2배

                // 커지기 (OutElastic)
                completionSequence.Join(rect.DOScale(targetScale, scaleUpDuration).SetEase(Ease.OutElastic));
            }
        }

        // 작아지기 (OutElastic) - 커지기가 끝난 후 동시에 실행
        // 첫 번째는 Append로 시퀀스에 추가, 나머지는 Join으로 동시 실행
        bool isFirst = true;
        for (int i = 0; i < bottomSlots.Length; i++)
        {
            if (bottomSlots[i] != null)
            {
                RectTransform rect = bottomSlots[i].GetComponent<RectTransform>();
                Vector3 originalScale = Vector3.one;

                // 첫 번째는 Append, 나머지는 Join (동시 실행)
                if (isFirst)
                {
                    completionSequence.Append(rect.DOScale(originalScale, scaleDownDuration).SetEase(Ease.Linear));
                    isFirst = false;
                }
                else
                {
                    completionSequence.Join(rect.DOScale(originalScale, scaleDownDuration).SetEase(Ease.Linear));
                }
            }
        }

        // 애니메이션 완료 후 콜백 실행
        completionSequence.OnComplete(() =>
        {
            Debug.Log("[FoodAnimationController] 꼬치 완성 애니메이션 완료");
            onComplete?.Invoke();
        });
    }

    /// <summary>
    /// 음식 하나를 꼬치로 이동
    /// </summary>
    private void MoveFoodToSkewer(int slotIndex, int foodIndex, Action onComplete)
    {
        // 1. 런타임에 GameObject 생성
        GameObject foodClone = new GameObject($"FoodClone_{slotIndex}");
        foodClone.transform.SetParent(canvas.transform, false);

        // 2. Image 컴포넌트 추가
        Image cloneImage = foodClone.AddComponent<Image>();
        cloneImage.sprite = foodSprites[foodIndex];
        cloneImage.raycastTarget = false; // 성능 최적화

        // 3. RectTransform 설정
        RectTransform cloneRect = foodClone.GetComponent<RectTransform>();
        cloneRect.sizeDelta = new Vector2(100, 100); // 크기 설정

        // 4. 시작 위치 (슬롯 중앙 이미지 위치)
        Vector3 startPos = slotColumns[slotIndex].GetCenterImagePosition();
        cloneRect.position = startPos;

        // 5. 목표 위치 (현재 꼬치 스택 위치)
        Vector3 endPos = bottomSlots[skewerStackCount].transform.position;

        // 6. 랜덤 포물선 경로 생성
        Vector3[] path = CreateRandomArcPath(startPos, endPos);

        // 7. DOTween 경로 이동
        cloneRect.DOPath(path, moveDuration, PathType.CatmullRom)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // 8. 도착 후 처리
                Destroy(foodClone); // 복제 이미지 삭제
                ActivateBottomSlot(foodIndex); // 꼬치 슬롯 활성화
                onComplete?.Invoke();
            });
    }

    /// <summary>
    /// 랜덤 포물선 경로 생성
    /// </summary>
    private Vector3[] CreateRandomArcPath(Vector3 start, Vector3 end)
    {
        Vector3 middle = (start + end) / 2f;

        // Y축 높이 랜덤 (위로 튀어오르는 높이)
        float arcHeight = UnityEngine.Random.Range(arcHeightMin, arcHeightMax);
        middle.y += arcHeight;

        // X축 랜덤 편차 (좌우 흔들림)
        float xOffset = UnityEngine.Random.Range(-horizontalOffsetRange, horizontalOffsetRange);
        middle.x += xOffset;

        // 3점 경로: 시작 → 중간(포물선 정점) → 끝
        return new Vector3[] { start, middle, end };
    }

    /// <summary>
    /// 꼬치 슬롯 활성화
    /// </summary>
    private void ActivateBottomSlot(int foodIndex)
    {
        Debug.Log($"[FoodAnimationController] ActivateBottomSlot 호출 - foodIndex: {foodIndex}, skewerStackCount: {skewerStackCount}");

        // 3개 초과 시 더 이상 활성화하지 않음
        if (skewerStackCount >= bottomSlots.Length)
        {
            Debug.LogWarning("[FoodAnimationController] 꼬치 슬롯이 가득 찼습니다! (더 이상 활성화하지 않음)");
            return;
        }

        // Null 체크
        if (bottomSlots[skewerStackCount] == null)
        {
            Debug.LogError($"[FoodAnimationController] bottomSlots[{skewerStackCount}]이 null입니다!");
            return;
        }

        if (foodSprites[foodIndex] == null)
        {
            Debug.LogError($"[FoodAnimationController] foodSprites[{foodIndex}]가 null입니다!");
            return;
        }

        // bottomSlot에 음식 스프라이트 설정 및 활성화
        bottomSlots[skewerStackCount].sprite = foodSprites[foodIndex];
        bottomSlots[skewerStackCount].gameObject.SetActive(true);

        Debug.Log($"[FoodAnimationController] BottomSlot[{skewerStackCount}] 활성화 완료 - 음식: {foodIndex}, Active: {bottomSlots[skewerStackCount].gameObject.activeSelf}");

        skewerStackCount++;
    }

    /// <summary>
    /// 꼬치 초기화 (3개 완성 시)
    /// </summary>
    private void ResetSkewer()
    {
        Debug.Log("[FoodAnimationController] 꼬치 초기화");

        // TODO: 나중에 축하 애니메이션 추가

        // 모든 bottomSlot 비활성화
        foreach (var slot in bottomSlots)
        {
            slot.gameObject.SetActive(false);
        }

        skewerStackCount = 0;
    }

    /// <summary>
    /// 게임 시작 시 초기화
    /// </summary>
    public void Initialize()
    {
        ResetSkewer();
    }
}
