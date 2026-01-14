# 빙고 시스템 사용 방법

![Screenshot](/docs/image/02_Image_Work_Log_Documents/07_Prefabs_Tiles_Test_3.png)

* 새로운 타일 프리팹 추가, 교체 바랍니다.

![Screenshot](/docs/image/02_Image_Work_Log_Documents/08_Component_Tile_VFX_Controller.png)

* `Tile VFX Controller`가 추가되었으며, `PlayBingoVFX(Vector3 position, bool useXorY)` 를 통해 빙고 호출 가능.
* `PlayBingoVFX(Vector3 position, bool useXorY)` 안에는 좌표와 가로선 혹은 세로선을 결정하는 boolean값이 들어감.

---

* 작성일: `2025-07-21`
* 작성자: 이은수
* 내용: 전준호