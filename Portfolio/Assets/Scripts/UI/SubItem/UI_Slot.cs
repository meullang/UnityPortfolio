using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Slot : UI_Base
{
    enum GameObjects
    {
        Icon,
        CountImage,
        Count,
    }

    public Define.SlotType _type;
    public UI_Inven _Inven;
    public GameObject shop;

    public Item item;
    public UI_SlotToolTip theSlotToolTip;

    public Image item_Icon;
    public Text text_Count;
    public GameObject count_Image;

    public int itemCount;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        item_Icon = Get<GameObject>((int)GameObjects.Icon).GetComponent<Image>();
        text_Count = Get<GameObject>((int)GameObjects.Count).GetComponent<Text>();
        count_Image = Get<GameObject>((int)GameObjects.CountImage);

        BindEvent(this.gameObject, (PointerEventData data) => { PointerClick(data); }, Define.UIEvent.Click);
        BindEvent(this.gameObject, (PointerEventData data) => { BeginDrag(data); }, Define.UIEvent.BeginDrag);
        BindEvent(this.gameObject, (PointerEventData data) => { Drag(data); }, Define.UIEvent.Drag);
        BindEvent(this.gameObject, (PointerEventData data) => { EndDrag(data); }, Define.UIEvent.EndDrag);
        BindEvent(this.gameObject, (PointerEventData data) => { Drop(data); }, Define.UIEvent.Drop);
        BindEvent(this.gameObject, (PointerEventData data) => { PointerEnter(data); }, Define.UIEvent.OnEnter);
        BindEvent(this.gameObject, (PointerEventData data) => { PointerExit(data); }, Define.UIEvent.OnExit);

        ClearSlot();
    }

    private void Start()
    {
        shop = GameObject.Find("@UI_Root").transform.Find("UI_Shop").gameObject;
        theSlotToolTip = GameObject.Find("@UI_Root").transform.Find("UI_SlotToolTip").gameObject.GetComponent<UI_SlotToolTip>();
    }

    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        item_Icon.sprite = item.itemImage;

        if (item.itemType == Item.ItemType.Used || item.itemType == Item.ItemType.Ingredient)
        {
            count_Image.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            count_Image.SetActive(false);
        }

        SetColor(1);
    }

    private void SetColor(float _alpha)
    {
        Color color = item_Icon.color;
        color.a = _alpha;
        item_Icon.color = color;
    }

    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    public void ClearSlot()
    {
        item = null;
        itemCount = 0;
        item_Icon.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        count_Image.SetActive(false);
    }

    public void PointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                if (shop.activeSelf == true)
                {
                    _Inven.Money += item.sellCost;
                    if (item.itemType == Item.ItemType.Used || item.itemType == Item.ItemType.Ingredient)
                        SetSlotCount(-1);
                    else
                        ClearSlot();
                }
                else
                {
                    Managers.Database.UseItem(item);
                    if (item.itemType == Item.ItemType.Used)
                        SetSlotCount(-1);
                }
            }
        }
    }

    public void BeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            UI_DragSlot.instance.dragSlot = this;
            UI_DragSlot.instance.DragSetImage(item_Icon);
            UI_DragSlot.instance.moveItem.position = eventData.position;
        }
    }

    public void Drag(PointerEventData eventData)
    {
        if (item != null)
        {
            UI_DragSlot.instance.moveItem.position = eventData.position;
        }
    }

    public void EndDrag(PointerEventData eventData)
    {
        UI_DragSlot.instance.SetColor(0);
        UI_DragSlot.instance.dragSlot = null;
    }

    public void Drop(PointerEventData eventData)
    {
        if (UI_DragSlot.instance.dragSlot != null && TypeFilter(UI_DragSlot.instance.dragSlot.item))
        {
            ChangeSlot();
        }
    }

    private void ChangeSlot()
    {
        Item _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(UI_DragSlot.instance.dragSlot.item, UI_DragSlot.instance.dragSlot.itemCount);

        if (_tempItem != null)
        {
            UI_DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        }
        else
        {
            UI_DragSlot.instance.dragSlot.ClearSlot();
        }
    }
    
    public void PointerEnter(PointerEventData eventData)
    {
        if (item != null)
            theSlotToolTip.ShowToolTip(item, transform.position);
    }

    public void PointerExit(PointerEventData eventData)
    {
        theSlotToolTip.HideToolTip();
    }

    private bool TypeFilter(Item _item)
    {
        switch (_type)
        {
            case Define.SlotType.EveryThing:
                return true;
            case Define.SlotType.OnlyEquipment:
                if (_item.itemType == Item.ItemType.Weapon || _item.itemType == Item.ItemType.Armor || _item.itemType == Item.ItemType.Accessory)
                    return true;
                break;
            case Define.SlotType.OnlyWeapon:
                if (_item.itemType == Item.ItemType.Weapon)
                    return true;
                break;
            case Define.SlotType.OnlyArmor:
                if (_item.itemType == Item.ItemType.Armor)
                    return true;
                break;
            case Define.SlotType.OnlyAccessory:
                if (_item.itemType == Item.ItemType.Accessory)
                    return true;
                break;
            case Define.SlotType.OnlyUsed:
                if (_item.itemType == Item.ItemType.Used)
                    return true;
                break;
        }

        return false;
    }
}
