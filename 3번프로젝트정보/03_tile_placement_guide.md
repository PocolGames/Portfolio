# 기물 배치 가능 범위 표시 기능 사용 가이드

게임에서 기물을 이동할 때 어디로 이동이 가능한지 시각적으로 표시하는 기능의 사용 방법을 설명합니다.

## 프리팹 설정

지정된 경로에서 `Tiles_Test_1` 프리팹을 찾아 게임 씬의 필드에 배치합니다.

![ScreenShot](/docs/image/02_Image_Work_Log_Documents/04_where_is_isometric_prefab.png)

## 주요 함수

### 배치 가능 범위 표시

```csharp
`ShowPlaceableTiles(int[,] data)`
```

이 함수는 기물이 배치 가능한 타일들을 하이라이트로 표시합니다.

**사용 방법:**
1. `Tiles_Test_1` 프리팹의 `TileSysTest` 오브젝트를 선택
2. `PlaceableTileHighlighter` 컴포넌트에 접근
3. `ShowPlaceableTiles()` 함수를 호출하여 배치 가능한 영역을 표시

![ScreenShot](/docs/image/02_Image_Work_Log_Documents/05_where_is_PlaceableTileHighlighter_Component.png)

### 하이라이트 제거

```csharp
ClearTiles()
```

배치 가능 범위 표시를 모두 제거하고 싶을 때 사용합니다.

## 사용 예시

```csharp
// 배치 가능한 타일 표시
placeableTileHighlighter.ShowPlaceableTiles(movementData);

// 하이라이트 제거
placeableTileHighlighter.ClearTiles();
```

---

* 작성일: `2025-07-10`
* 작성자: 이은수
* 내용: 전준호