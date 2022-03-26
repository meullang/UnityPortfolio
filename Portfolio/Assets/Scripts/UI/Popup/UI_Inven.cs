using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven : UI_Popup
{
    enum GameObjects
    {
        GridPanel,
        InventoryText,
        Inventory,
        CurrentMoney
    }

    public UI_Slot[] slots;
    private ActionController _action;

    int _money = 0;
    public int Money { get { return _money; } set { _money = value; SetMoneyText(); } }
    Text _moneyText;

    public GameObject moveArea;
    public GameObject movePoint;

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        _action = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<ActionController>();
        _action._Inven = this;

        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        slots = gridPanel.transform.GetComponentsInChildren<UI_Slot>();
        foreach (UI_Slot slot in slots)
        {
            slot._type = Define.SlotType.EveryThing;
            slot._Inven = this;
        }

        _moneyText = Get<GameObject>((int)GameObjects.CurrentMoney).GetComponent<Text>();

        movePoint = Get<GameObject>((int)GameObjects.InventoryText);
        moveArea = Get<GameObject>((int)GameObjects.Inventory);
        BindEvent(movePoint, (PointerEventData data) => { moveArea.transform.position = data.position; }, Define.UIEvent.Drag);

        SetMoneyText();
        Managers.Database._Inven = this;
        Managers.Quest._inven = this;
        Managers.SaveLoad.theInven = this;
    }

    public UI_Slot[] GetSlots() { return slots; }

    public void LoadToInven(int _arrayNum, int _itemCode, int _itemNum)
    {
        slots[_arrayNum].AddItem(Managers.Database.ItemDictionary[_itemCode], _itemNum);
    }

    public int CheckItemIndex(int _code)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemCode == _code)
                {
                    return i;
                }
            }
        }

        return -1;
    }
    /*
    public bool CheckItemIndex(int _code, bool erase = false, int eraseNum = 1)
    {
        int slotNum = -1;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemCode == _code)
                {
                    slotNum = i;
                    break;
                }
            }
        }

        if(slotNum == -1)
        {
            return false;
        }

        if (erase && slots[slotNum].itemCount >= eraseNum)
        {
            slots[slotNum].SetSlotCount(eraseNum);
            return true;
        }
        else if (!erase)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    */
    public void AcquireItem(Item _item, int _count = 1)
    {
        if (Item.ItemType.Used == _item.itemType || Item.ItemType.Ingredient == _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemCode == _item.itemCode)
                    {
                        slots[i].SetSlotCount(_count);
                        Managers.Notify.SetNotification($"{_item.itemName}을 얻었습니다.");
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                Managers.Notify.SetNotification($"{_item.itemName}을 얻었습니다.");
                return;
            }
        }
    }

    private void SetMoneyText()
    {
        _moneyText.text = _money.ToString();
    }
}
