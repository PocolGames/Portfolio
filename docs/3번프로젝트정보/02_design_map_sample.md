# 맵 디자인 샘플

## 정상적으로 보이는 맵
![Screenshot](/docs/image/02_Image_Work_Log_Documents/01_map_sample.png)

### 오류가 있는 맵
![Screentshot](/docs/image/02_Image_Work_Log_Documents/02_map_sample_error.png)

타일맵에 오브젝트를 배치함에 있어, 2d 공간의 좌표는 3d공간과 다르기 때문에, 마치 3d공간처럼 보이기 위해 아래에 있는 오브젝트가 앞에 있다 가정하는 세팅이 필요하다.
따라서, 그래픽 설정의 Render Pipline 설정을 찾아 Renderer2D에서 Transparency Sort Mode를 커스텀으로 수정하여 Y축에 솔팅 우선순위를 설정하였다.

모든 top view 게임에서 공통적으로 해야할 설정이라고 한다.

![Screenshot](/docs/image/02_Image_Work_Log_Documents/03_Renderer2D_Component_Transparency_Sort_Mode.png)

---

* 작성일: `2025-07-02`
* 작성자: 이은수
* 내용: 전준호