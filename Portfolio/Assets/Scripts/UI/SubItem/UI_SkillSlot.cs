using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillSlot : UI_Base
{
    enum GameObjects
    {
        Icon,
        CoolingFiller,
        SkillLevel,
        SkillLevelImage,
    }

    public Image skill_icon;
    public Image coolingTime;
    public Text skill_Level;
    public GameObject LevelImage;

    public SkillInfo _skill;
    public UI_SlotToolTip toolTip;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        skill_icon = Get<GameObject>((int)GameObjects.Icon).GetComponent<Image>();
        coolingTime = Get<GameObject>((int)GameObjects.CoolingFiller).GetComponent<Image>();
        skill_Level = Get<GameObject>((int)GameObjects.SkillLevel).GetComponent<Text>();
        LevelImage = Get<GameObject>((int)GameObjects.SkillLevelImage);


        BindEvent(this.gameObject, (PointerEventData data) => { SkillPointerEnter(data); }, Define.UIEvent.OnEnter);
        BindEvent(this.gameObject, (PointerEventData data) => { SkillPointerExit(data); }, Define.UIEvent.OnExit);
    }

    private void Start()
    {
        toolTip = GameObject.Find("@UI_Root").transform.Find("UI_SlotToolTip").gameObject.GetComponent<UI_SlotToolTip>();
        SetInfo();
    }

    private void SetInfo()
    {
        skill_icon.sprite = _skill.skillImage;
        skill_Level.text = Managers.Skill.playerSkillLevel[_skill.skillCode].ToString();
    }

    public void SetCoolingTimeFiller(int n)
    {
        coolingTime.fillAmount = Managers.Skill.leftCoolTingTime(n);
    }

    public void SkillPointerEnter(PointerEventData eventData)
    {
        toolTip.ShowSkillToolTip(_skill, transform.position);
    }

    public void SkillPointerExit(PointerEventData eventData)
    {
        toolTip.HideToolTip();
    }
}
