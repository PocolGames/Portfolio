# 멀티플레이 환경 테스트 중 호스트와 클라이언트 통신 과정에서 발생하는 동기화 문제 해결 과정

1. 일반 이동 방식에서 동기화 시 호스트에서만 작동하고 클라이언트에서는 작동을 하지 않음
2. 권한을 클라이언트에도 부여하여 이동방식을 제작했지만 추후 보안 문제 발생가능성이 높아서 기각
3. 클라이언트에서 ServerRpc를 통해 통신하는 방법으로 호스트에서 작동하는 방식으로 수정하여 보안성 높임

![Screenshot](/docs/image/02_Image_Work_Log_Documents/00_Multi-play_test_view.png)

``` csharp
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : NetworkBehaviour
{
    public float moveSpeed = 5f;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    private void Awake()
    {
        Application.runInBackground = true;
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false; // 로컬 플레이어가 아닌 경우 입력 비활성화
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        // New Input System을 사용하여 이동 입력 처리
        moveInput = GetMoveInput();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        Debug.Log(moveInput * moveSpeed);
        // 서버와 위치 동기화
        UpdatePositionServerRpc(moveInput * moveSpeed);
    }

    private Vector2 GetMoveInput()
    {
        // New Input System에서 Move 액션 값 가져오기
        return new Vector2(
            Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0,
            Keyboard.current.sKey.isPressed ? -1 : Keyboard.current.wKey.isPressed ? 1 : 0
        );
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector2 power)
    {
        rb.linearVelocity = power;
    }
}
```

* 작성일: `2025-07-02`
* 작성자: 이은수
* 내용: 최광남