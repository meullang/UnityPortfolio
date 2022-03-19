using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public UI_Inven _Inven;
    public UI_Shop _Shop;
    public UI_PlayerInfo _Info;
    public UI_Talk _talk;
    public UI_SlotToolTip _toolTip;
    public int talkIndex;

    private bool canInput = true;
    private float talkTime = 0;

    private PlayerController playerCr;
    private CameraController cameraCr;

    void Start()
    {
        playerCr = GetComponent<PlayerController>();
        cameraCr = Camera.main.GetComponent<CameraController>();

        LockCursorAndMotion();
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
                NPCData _npc = coll.GetComponent<NPCData>();

                Talk(_npc.npcID, _npc.NPCname);
                talkTime = 10;
            }
            else if (coll.CompareTag("OBJECT"))
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

    private void Talk(int id, string _name)
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
        _talk.GetName(_name);
        Managers.UI.ShowPopupUI<UI_Talk>();
        talkIndex++;
    }

    private void LockCursorAndMotion()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        playerCr.isStop = true;
        cameraCr.isStop = true;
    }

    private void ReleaseCursorAndMotion()
    {
        if (Managers.UI.GetStackSize() < 1)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            playerCr.isStop = false;
            cameraCr.isStop = false;
        }
    }
}
