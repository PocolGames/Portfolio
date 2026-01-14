# 임시 로비 UI 테스트 설계

## 테스트 화면

![screenshot](/docs/image/03_Image_Meeting_Notes_Documents/02_lobby_test.png)

## 비고

* AI를 이용해 만든 UI
* 화면에 보이는 모든 UI가 꼭 존재할 필요는 없기 때문에 중요한 요소들만 남겨둘 것
* 아래는 UI를 구현하는 데 사용한 코드들

## Create Lobby Panel
로비 생성 UI
|이름|Type|설명|
|---|---|---|
|LobbyNameInput|TMP_Text|생성하려는 방의 이름|
|PrivateToggle|Toggle|생성하려는 방의 공개/비공개 여부|
|CreateLobbyButton|Button|방 생성 버튼|

## Lobby List Panel
로비 목록 UI
|이름|Type|설명|
|---|---|---|
|RefreshButton|Button|로비 목록 새로고침 버튼|
|LobbyScrollView|ScrollRect|하위 Content에 LobbyDataPrefab 생성|
|LobbyList|Content|ScrollRect 하위에 존재하는 Content|

## LobbyDataPrefab
로비 목록 리스트에 들어가는 UI Prefab

|이름|Type|설명|
|---|---|---|
|LobbyName|TMP_Text|로비 이름|
|PlayerCount|TMP_Text|현재 참가 중인 플레이어 수|
|GameMode|TMP_Text|게임모드에 대한 설명 부분인데 일단 넣어놓음. 기본값 : Standard|
|Join|Button|해당 방에 참가하기 위한 버튼|

## Current Lobby Panel
현재 참가 중인 로비에 대한 정보

|이름|Type|설명|
|---|---|---|
|LobbyName|TMP_Text|로비 이름|
|PlayerCount|TMP_Text|현재 참가 중인 플레이어 수|
|PlayerList|TMP_Text|현재 참가 중인 플레이어에 대한 정보|
|HostStatus|TMP_Text|현재 Host인지 Client인지 구분하는 텍스트|
|LeaveLobby|Button|로비 퇴장 버튼|
|DeleteLobby|Button|로비 삭제 버튼|

## 비고

* 로비 시스템 구현 성공.
* 하지만, 로그인 시스템을 구현하지 않았기 때문에 이 기능을 계속 적용할 수 있을지는 미지수.
* 그렇기떄문에 로그인 시스템을 구현하는 것이 최우선 과제임.

## 작업 계획

* 로그인 시스템
* 랭킹 시스템
* 결제 시스템
* 로비 시스템
* 상점 UI
* 배틀 패스

---

- 작성일: `2025-08-13`
- 작성자: 이은수
- 참여자: 이은수, 최광남