using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_SlotToolTip : UI_Base
{
    enum GameObjects
    {
        ShowToolTipPosition,
        ItemName,
        ItemDesc,
        Cost,
    }

    private ActionController _action;

    private GameObject moveBase;
    private Text txt_ItemName;
    private Text txt_ItemDesc;
    private Text txt_ItemCost;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        moveBase = Get<GameObject>((int)GameObjects.ShowToolTipPosition);
        txt_ItemName = Get<GameObject>((int)GameObjects.ItemName).GetOrAddComponent<Text>();
        txt_ItemDesc = Get<GameObject>((int)GameObjects.ItemDesc).GetOrAddComponent<Text>();
        txt_ItemCost = Get<GameObject>((int)GameObjects.Cost).GetOrAddComponent<Text>();

        moveBase.SetActive(false);

        _action = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<ActionController>();
        _action._toolTip = this;
    }

    public void ShowToolTip(Item _item, Vector3 _pos, bool isShop = false)
    {
        moveBase.SetActive(true);
        _pos += new Vector3(moveBase.GetComponent<RectTransform>().rect.width, -moveBase.GetComponent<RectTransform>().rect.height + 30, 0f);
        moveBase.transform.position = _pos;

        txt_ItemName.text = _item.itemName;
        txt_ItemDesc.text = _item.itemDesc;
        if (isShop)
            txt_ItemCost.text = $"buying price {_item.buyCost.ToString()}";
        else
            txt_ItemCost.text = $"selling price {_item.sellCost.ToString()}";
    }

    public void ShowSkillToolTip(SkillInfo _skill, Vector3 _pos)
    {
        moveBase.SetActive(true);
        _pos += new Vector3(moveBase.GetComponent<RectTransform>().rect.width * 0.5f, -moveBase.GetComponent<RectTransform>().rect.height, 0f);
        moveBase.transform.position = _pos;

        string descEffect;
        if(_skill.skillCode == 2)
        {
            descEffect = "의 체력을 회복합니다.";
        }
        else
        {
            descEffect = "의 피해를 줍니다.";
        }

        txt_ItemName.text = _skill.skillName;
        txt_ItemDesc.text = _skill.skillDesc + $"{_skill.damage + _skill.damageIncreaseRate * Managers.Skill.playerSkillLevel[_skill.skillCode]}" + descEffect;
        txt_ItemCost.text = $"Mana {_skill.ManaCost}, cool {_skill.coolingTime - _skill.coolingTimeDecrease * Managers.Skill.playerSkillLevel[_skill.skillCode]}" ;
    }

    public void HideToolTip()
    {
        moveBase.SetActive(false);
    }
}
