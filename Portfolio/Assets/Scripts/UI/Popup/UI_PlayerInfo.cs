using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_PlayerInfo : UI_Popup
{
    enum GameObjects
    {
        Skill,
        SkillText,
        SkillPoint,
        //스킬
        UI_SkillSlot_1,
        UI_SkillSlot_2,
        UI_SkillSlot_3,
        Button_1,
        Button_2,
        Button_3,
        //장비
        UI_Slot_Weapon,
        UI_Slot_Armor,
        UI_Slot_Accessory,
        //스탯
        HPText,
        MPText,
        LevelText,
        AttackText,
        DefenceText,
        EXPText,
    }

    PlayerStat _playerStat;
    PlayerController _playerController;

    Button skill_1;
    Button skill_2;
    Button skill_3;

    Text skillPointText;

    UI_SkillSlot[] skillSlots = new UI_SkillSlot[3];
    UI_Slot[] equipmentSlots = new UI_Slot[3];

    Text HP, MP, Level, Attack, Defence, EXP;

    public UI_SkillSlot[] GetSkillSlots() { return skillSlots; }
    public UI_Slot[] GetSlots() { return equipmentSlots; }

    public void LoadToEquipment(int _arrayNum, int _itemCode)
    {
        equipmentSlots[_arrayNum].AddItem(Managers.Database.ItemDictionary[_itemCode]);
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        _playerStat = GameObject.FindWithTag("PLAYER").GetComponent<PlayerStat>();
        _playerController = GameObject.FindWithTag("PLAYER").GetComponent<PlayerController>();

        //스킬 업과 포인트
        skillPointText = Get<GameObject>((int)GameObjects.SkillPoint).GetComponent<Text>();
        skillSlots[0] = Get<GameObject>((int)GameObjects.UI_SkillSlot_1).GetComponent<UI_SkillSlot>();
        skillSlots[1] = Get<GameObject>((int)GameObjects.UI_SkillSlot_2).GetComponent<UI_SkillSlot>();
        skillSlots[2] = Get<GameObject>((int)GameObjects.UI_SkillSlot_3).GetComponent<UI_SkillSlot>();

        skill_1 = Get<GameObject>((int)GameObjects.Button_1).GetComponent<Button>();
        skill_1.onClick.AddListener(SkillUP_1);
        skill_2 = Get<GameObject>((int)GameObjects.Button_2).GetComponent<Button>();
        skill_2.onClick.AddListener(SkillUP_2);
        skill_3 = Get<GameObject>((int)GameObjects.Button_3).GetComponent<Button>();
        skill_3.onClick.AddListener(SkillUP_3);
        skillPointText = Get<GameObject>((int)GameObjects.SkillPoint).GetComponent<Text>();

        //창 이동
        GameObject movePoint = Get<GameObject>((int)GameObjects.SkillText);
        GameObject moveArea = Get<GameObject>((int)GameObjects.Skill);
        BindEvent(movePoint, (PointerEventData data) => { moveArea.transform.position = data.position; }, Define.UIEvent.Drag);

        //장비
        equipmentSlots[0] = Get<GameObject>((int)GameObjects.UI_Slot_Weapon).GetComponent<UI_Slot>();
        equipmentSlots[1] = Get<GameObject>((int)GameObjects.UI_Slot_Armor).GetComponent<UI_Slot>();
        equipmentSlots[2] = Get<GameObject>((int)GameObjects.UI_Slot_Accessory).GetComponent<UI_Slot>();

        equipmentSlots[0]._type = Define.SlotType.OnlyWeapon;
        equipmentSlots[1]._type = Define.SlotType.OnlyArmor;
        equipmentSlots[2]._type = Define.SlotType.OnlyAccessory;

        //스탯 정보
        HP = Get<GameObject>((int)GameObjects.HPText).GetComponent<Text>();
        MP = Get<GameObject>((int)GameObjects.MPText).GetComponent<Text>();
        Level = Get<GameObject>((int)GameObjects.LevelText).GetComponent<Text>();
        Attack = Get<GameObject>((int)GameObjects.AttackText).GetComponent<Text>();
        Defence = Get<GameObject>((int)GameObjects.DefenceText).GetComponent<Text>();
        EXP = Get<GameObject>((int)GameObjects.EXPText).GetComponent<Text>();

        SetSkillSlot();
        Managers.SaveLoad.theInfo = this;
        Managers.Database._PlayerInfo = this;
    }

    private void Update()
    {
        SetInfo();
    }

    private void SetSkillSlot()
    {
        for (int i = 0; i < skillSlots.Length; ++i)
        {
            skillSlots[i]._skill = Managers.Skill.playerSkillSlots[i];
        }
    }

    private void SkillUP_1()
    {
        if (_playerStat.SkillPoint > 0)
        {
            Managers.Skill.SkillCountUP(0);
            _playerStat.SkillPoint--;
            SetSkillPoint();
        }
        else
        {
            Managers.Notify.SetNotification("스킬 포인트가 부족합니다.");
            Debug.Log("스킬 포인트가 부족합니다.");
        }
    }

    private void SkillUP_2()
    {
        if (_playerStat.SkillPoint > 0)
        {
            Managers.Skill.SkillCountUP(1);
            _playerStat.SkillPoint--;
            SetSkillPoint();
        }
        else
        {
            Managers.Notify.SetNotification("스킬 포인트가 부족합니다.");
            Debug.Log("스킬 포인트가 부족합니다.");
        }
    }

    private void SkillUP_3()
    {
        if (_playerStat.SkillPoint > 0)
        {
            Managers.Skill.SkillCountUP(2);
            _playerStat.SkillPoint--;
            SetSkillPoint();
        }
        else
        {
            Managers.Notify.SetNotification("스킬 포인트가 부족합니다.");
            Debug.Log("스킬 포인트가 부족합니다.");
        }
    }

    private void SetInfo()
    {
        SetSkillPoint();
        SetStatus();
        SetEquipment();
    }

    private void SetSkillPoint()
    {
        skillPointText.text = _playerStat.SkillPoint.ToString();
        for (int i = 0; i < skillSlots.Length; ++i)
        {
            skillSlots[i].skill_Level.text = Managers.Skill.playerSkillLevel[skillSlots[i]._skill.skillCode].ToString();
        }
    }

    private void SetStatus()
    {
        HP.text = $"hp : {_playerStat.Hp} / {_playerStat.MaxHp}";
        MP.text = $"mp : {_playerStat.Mp} / {_playerStat.MaxMp}";
        Level.text = $"Level : {_playerStat.Level}";
        Attack.text = $"attack : {_playerStat.Attack}";
        Defence.text = $"defence : {_playerStat.Defence}";
        EXP.text = $"exp : {_playerStat.Exp} / {_playerStat.TotalExp}";
    }

    int itemAddAttack;
    int itemAddDefence;
    public void SetEquipment()
    {
        if (equipmentSlots[0].item == null)
        {
            if (_playerController.PlayerWeapon != null)
                _playerController.PlayerWeapon.SetActive(false);

            _playerController.PlayerWeapon = null;

            _playerController.anim.runtimeAnimatorController = _playerController.animControllerList[0];
            _playerController.PlayerWeaponCode = 4; //무기가 없다는 코드
        }
        else if (equipmentSlots[0].item.weaponCode != _playerController.PlayerWeaponCode)
        {
            if (_playerController.PlayerWeapon != null)
                _playerController.PlayerWeapon.SetActive(false);

            _playerController.PlayerWeapon = _playerController.PlayerWeaponList[equipmentSlots[0].item.weaponCode];
            _playerController.anim.runtimeAnimatorController = _playerController.animControllerList[equipmentSlots[0].item.weaponCode];

            _playerController.PlayerWeapon.SetActive(true);
            _playerController.P_DisableWeapon();
        }

        itemAddAttack = 0;
        itemAddDefence = 0;
        for (int i = 0; i < equipmentSlots.Length; ++i)
        {
            if (equipmentSlots[i].item != null)
            {
                itemAddAttack += equipmentSlots[i].item.attack;
                itemAddDefence += equipmentSlots[i].item.defence;
            }
        }

        _playerStat.AddAttack = itemAddAttack;
        _playerStat.AddDefence = itemAddDefence;
    }
}
