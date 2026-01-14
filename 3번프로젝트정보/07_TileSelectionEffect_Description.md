# 선택 이펙트 설명 문서

![screenshot](/docs/image/02_Image_Work_Log_Documents/09_Tile_VFX_Controller_Component.png)

## 개요

타일 선택 시 플레이어가 시각적으로 구분하기 쉽게 VFX 색상과 선택 이펙트를 개선합니다.
기존 이동 범위와 선택 범위가 혼동되는 문제를 해결하며, 오브젝트가 밀집된 상태에서도 어떤 기물이 선택되었는지 명확히 표시합니다.

---

## 변경 사항

* **이동 범위 VFX 색상 변경**
  기물 이동 범위를 다른 색상으로 표시하여 선택 영역과 혼동을 줄임.
* **선택 기물 표시 개선**
  오브젝트가 뭉쳐 있는 상황에서도 선택된 기물을 직관적으로 확인 가능.
* **TileVFXController**에 새로운 함수 2개 추가:

  1. `ShowSelectedEffect(Vector3 position)`
     지정한 위치에 선택 이펙트를 표시.
  2. `HideSelectedEffect()`
     현재 표시 중인 선택 이펙트를 숨김.

---

## API

### `void ShowSelectedEffect(Vector3 position)`

**설명:**
지정된 월드 좌표에 선택 이펙트를 표시합니다.

**매개변수:**

* `position` (`Vector3`)
  선택 이펙트를 표시할 타일의 월드 좌표.

---

### `void HideSelectedEffect()`

**설명:**
현재 활성화된 선택 이펙트를 비활성화합니다.

---


* 작성일: `2025-08-05`
* 작성자: 이은수
* 내용: 전준호