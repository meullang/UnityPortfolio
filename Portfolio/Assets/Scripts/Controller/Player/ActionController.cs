using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public UI_Inven _Inven;
    public UI_PlayerInfo _Info;
    public UI_Talk _talk;
    public UI_SlotToolTip _toolTip;
    public int talkIndex;

    private bool canInput = true;
    private float talkTime = 0;

    private PlayerController _playerCr;
    private CameraController _cameraCr;

    void Start()
    {
        _playerCr = GetComponent<PlayerController>();
        _cameraCr = Camera.main.GetComponent<CameraController>();

        LockCursorAndMotion();

        StartCoroutine(Delay());
    }

    void Update()
    {
        talkTime = Mathf.Clamp(talkTime -= Time.deltaTime, 0, 10);

        TryOpenInventory();
        TryOpenPlyaerInfo();
        TryOpenSetting();
        ExitPopup();
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.3f);
        canInput = true;
    }

    void OnTriggerStay(Collider coll)
    {
        if (Input.GetKey(KeyCode.E) && canInput)
        {
            if (coll.CompareTag("ITEM"))
            {
                Managers.Sound.Play("OutWorld/pickup");
                _Inven.AcquireItem(coll.GetComponent<ItemPickUp>().item);
                Destroy(coll.gameObject);
            }
            else if (coll.CompareTag("NPC"))
            {
                NPCData npc = coll.GetComponent<NPCData>();

                Talk(npc.npcID, npc.NPCname);
                talkTime = 10;
            }
            else if (coll.CompareTag("OBJECT"))//변경해야됨
            {
                int ItemIndex = _Inven.CheckItemIndex(6);

                if (ItemIndex != -1)
                    _Inven.slots[ItemIndex].SetSlotCount(-1);

                coll.GetComponent<Chest>().OnActive();
            }
            else if (coll.CompareTag("MONEY"))
            {
                _Inven.Money += coll.GetComponent<Money>().thisMoney;
                Destroy(coll.gameObject);
            }

            canInput = false;
            StartCoroutine(Delay());
        }
    }

    //단축키로 UI띄우는 부분
    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Managers.UI.ShowPopupUI<UI_Inven>();
            LockCursorAndMotion();
        }
    }

    private void TryOpenPlyaerInfo()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Managers.UI.ShowPopupUI<UI_PlayerInfo>();
            LockCursorAndMotion();
        }
    }

    private void TryOpenSetting()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Managers.UI.ShowPopupUI<UI_Setting>();
            LockCursorAndMotion();
        }
    }

    private void ExitPopup()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Managers.UI.ClosePopupUI();
            _toolTip.HideToolTip();
            ReleaseCursorAndMotion();
        }
    }

    private void Talk(int id, string name)
    {
        LockCursorAndMotion();

        if (talkIndex == 0)
            Managers.Quest.CheckQuest(id);
        int questTalkIndex = Managers.Quest.GetQuestTalkIndex(id);
        string talkData = Managers.Talk.GetTalk(id + questTalkIndex, talkIndex);

        if (talkData == null)
        {
            if (Managers.Quest.isQuestComplete)
                Managers.Quest.NextQuest();

            Managers.UI.ClosePopupUI();
            talkIndex = 0;

            Managers.Quest.NPCs[id / 1000].GetComponent<NPCData>().OnActive();

            ReleaseCursorAndMotion();

            return;
        }

        _talk.GetText(talkData);
        _talk.GetName(name);
        Managers.UI.ShowPopupUI<UI_Talk>();
        talkIndex++;
    }

    private void LockCursorAndMotion() // 커서 + 화면 움직임 막기
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        _playerCr.isStop = true;
        _cameraCr.isStop = true;
    }

    private void ReleaseCursorAndMotion() // 커서 + 화면 움직임 풀기
    {
        if (Managers.UI.GetStackSize() < 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _playerCr.isStop = false;
            _cameraCr.isStop = false;
        }
    }
}
