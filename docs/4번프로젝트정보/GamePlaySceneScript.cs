using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlaySceneScript : MonoBehaviour
{
    // 터치 패널 버튼입니다. 이 게임에서 모든 플레이는 이 버튼 하나로 진행합니다.
    [SerializeField] private Button touchPanel;
    // 이 게임에서 사용하려고 하는 이미지 스프라이트 5개 입니다.
    [SerializeField] private Sprite[] foodImages = new Sprite[5];

    [Header("Top Panel")]
    // 첫번째 슬롯입니다. 0, 1, 2, 3, 4 중에서 배열 2번이 중앙입니다.
    [SerializeField] private Image[] top_slot_0 = new Image[5];
    // 두번째 슬롯입니다. 0, 1, 2, 3, 4 중에서 배열 2번이 중앙입니다.
    [SerializeField] private Image[] top_slot_1 = new Image[5];
    // 세번째 슬롯입니다. 0, 1, 2, 3, 4 중에서 배열 2번이 중앙입니다.
    [SerializeField] private Image[] top_slot_2 = new Image[5];
    // 현재 슬롯에 어떤 음식인지 알 수 있는 상태 변수입니다.
    [SerializeField] private int[] top_slot_index = new int[3];
    // 스코어 UI입니다. 슬롯이 완성되면 여기에 +1 됩니다.
    [SerializeField] private TextMeshProUGUI score;

    [Header("Bottom Panel")]
    // 이 Image 오브젝트는 top 슬롯들이 3개가 똑같을 때, 이 bottom 슬롯으로 음식 이미지가 이동하는 애니메이션을 보여주고 여기에 이미지가 들어갑니다. 이 게임 오브젝트는 기본적으로 SetActive(false) 상태로 있습니다.
    [SerializeField] private Image[] bottomSlot = new Image[3];
    // 체력을 표시하는 이미지입니다. 0번은 꽉찬 하트, 1번은 빈하트입니다. 0번이 체력이 있음을 보여주는 이미지고 1번이 체력이 없음을 보여주는 이미지입니다.
    [SerializeField] private Sprite[] heartImage = new Sprite[2];
    // 체력 UI입니다. 
    [SerializeField] private Image[] heartSlot = new Image[3];


    [Header("Result UI")]
    // 결과창 UI 입니다. 게임이 종료되면 이 UI가 켜집니다.
    [SerializeField] private GameObject resultPanel;
    // 최종 점수입니다. score라는 변수로 존재하고 있지만 결과창 UI에서 한번 더 보여줘야하기 때문에 존재합니다.
    [SerializeField] private TextMeshProUGUI final_score;
    // Again 버튼입니다. 게임이 종료되었을 때, 보상형 광고를 시청하고 체력을 +1 해주고 스코어를 유지한 상태로 계속해서 진행할 수 있습니다. 1번만 사용가능하며, 게임을 재시작하거나 메인메뉴로 이동하면 다시 사용할 수 있습니다.
    [SerializeField] private Button againBtn;
    // Replay 버튼입니다. 이 버튼을 누르면 모든 상태를 초기화하고 게임을 재시작합니다.
    [SerializeField] private Button replayBtn;
    // Exit 버튼입니다. 메인메뉴로 이동을 할 수 있는 버튼입니다.
    [SerializeField] private Button exitBtn;
}
