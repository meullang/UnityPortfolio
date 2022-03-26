using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : UI_Scene
{
    enum GameObjects
    {
        HP_Bar,
        SP_Bar,
        MP_Circle,
        EXP_Bar,

        SkillShortCut_1,
        SkillShortCut_2,
        SkillShortCut_3,

        ItemShortCut_1,
        ItemShortCut_2,
        ItemShortCut_3,

        NotificationText,

        QuestName,
        QuestTitle,
    }

    PlayerStat _playerStat;
    UI_SkillSlot[] skillSlots = new UI_SkillSlot[3];
    UI_Slot[] ItemSlots = new UI_Slot[3];

    public Text notificationText;
    public Text questName;
    public Text questStiuaton;

    private float fadeTime = 4;

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        _playerStat = GameObject.FindWithTag("PLAYER").GetComponent<PlayerStat>();
        Managers.Notify._game = this;
        Managers.SaveLoad.theGameUI = this;

        //½ºÅ³ Äü ½º·Ô
        skillSlots[0] = Get<GameObject>((int)GameObjects.SkillShortCut_1).GetComponent<UI_SkillSlot>();
        skillSlots[1] = Get<GameObject>((int)GameObjects.SkillShortCut_2).GetComponent<UI_SkillSlot>();
        skillSlots[2] = Get<GameObject>((int)GameObjects.SkillShortCut_3).GetComponent<UI_SkillSlot>();
        

        //¾ÆÀÌÅÛ Äü ½½·Ô
        ItemSlots[0] = Get<GameObject>((int)GameObjects.ItemShortCut_1).GetComponent<UI_Slot>();
        ItemSlots[1] = Get<GameObject>((int)GameObjects.ItemShortCut_2).GetComponent<UI_Slot>();
        ItemSlots[2] = Get<GameObject>((int)GameObjects.ItemShortCut_3).GetComponent<UI_Slot>();
        for(int i = 0; i < ItemSlots.Length; ++i)
        {
            ItemSlots[i]._type = Define.SlotType.OnlyUsed;
        }

        notificationText = Get<GameObject>((int)GameObjects.NotificationText).GetComponent<Text>();

        questName = Get<GameObject>((int)GameObjects.QuestName).GetComponent<Text>();
        questStiuaton = Get<GameObject>((int)GameObjects.QuestTitle).GetComponent<Text>();
    }

    public UI_Slot[] GetSlots() { return ItemSlots; }

    public void LoadToQuick(int _arrayNum, int _itemCode, int _itemNum)
    {
        ItemSlots[_arrayNum].AddItem(Managers.Database.ItemDictionary[_itemCode], _itemNum);
    }

    public void Start()
    {
        SetSkillSlot();
        for(int i = 0; i < skillSlots.Length; ++i)
        {
            skillSlots[i].LevelImage.SetActive(false);
        }
    }

    private void Update()
    {
        for(int i = 0; i < skillSlots.Length; ++i)
        {
            skillSlots[i].SetCoolingTimeFiller(i);
        }
        
        float HPratio = _playerStat.Hp / (float)_playerStat.MaxHp;
        float SPratio = _playerStat.Sp / (float)_playerStat.MaxSp;
        float MPratio = _playerStat.Mp / (float)_playerStat.MaxMp;
        float EXPratio = (_playerStat.Exp - _playerStat.PreTatalExp) / (float)(_playerStat.TotalExp - _playerStat.PreTatalExp);

        TryUseItem();

        SetStatusRatio(HPratio, SPratio, MPratio, EXPratio);
    }

    public void SetStatusRatio(float hpRatio, float spRatio, float mpRatio, float expRatio)
    {
        GetObject((int)GameObjects.HP_Bar).GetComponent<Image>().fillAmount = hpRatio;
        GetObject((int)GameObjects.SP_Bar).GetComponent<Image>().fillAmount = spRatio;
        GetObject((int)GameObjects.MP_Circle).GetComponent<Image>().fillAmount = mpRatio;
        GetObject((int)GameObjects.EXP_Bar).GetComponent<Image>().fillAmount = expRatio;
    }

    private void TryUseItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseItem(ItemSlots[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UseItem(ItemSlots[1]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            UseItem(ItemSlots[2]);
        }
    }

    private void UseItem(UI_Slot _slot)
    {
        if (_slot.item != null && _slot.item.itemType == Item.ItemType.Used)
        {
            Managers.Database.UseItem(_slot.item);
            _slot.SetSlotCount(-1); ;
        }
        else
        {
            Debug.Log("Äü½½·Ô¿¡¼­ »ç¿ëÇÒ ¼ö ¾ø´Â ¾ÆÀÌÅÛÀÔ´Ï´Ù.");
        }
    }

    private void SetSkillSlot()
    {
        for(int i = 0; i < skillSlots.Length; ++i)
        {
            skillSlots[i]._skill = Managers.Skill.playerSkillSlots[i];
        }
    }

    private IEnumerator notificationCoroutine;

    public void SetNewNotification(string message)
    {
        if(notificationCoroutine != null)
        {
            StopCoroutine(notificationCoroutine);
        }
        notificationCoroutine = FadeOutNotification(message);
        StartCoroutine(notificationCoroutine);
    }

    private IEnumerator FadeOutNotification(string message)
    {
        notificationText.text = message;
        float t = 0;
        while(t<fadeTime)
        {
            t += Time.deltaTime; //unscaledDeltaTime
            notificationText.color = new Color(
                notificationText.color.r,
                notificationText.color.g,
                notificationText.color.b,
                Mathf.Lerp(1f, 0f, t / fadeTime));
            yield return null;
        }
    }

    public void SetQuestSit(string _desc)
    {
        questStiuaton.text = _desc;
    }


    public void SetQuestName(string _name)
    {
        questName.text = _name;
    }
}
