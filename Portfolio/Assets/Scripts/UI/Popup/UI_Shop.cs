using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Shop : UI_Popup
{
    enum GameObjects
    {
        GridPanel,
        ShopText,
        Shop
    }

    protected UI_ShopSlot[] slots;

    private ActionController _action;

    public GameObject moveArea;

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        _action = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<ActionController>();

        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        slots = gridPanel.transform.GetComponentsInChildren<UI_ShopSlot>();

        GameObject movePoint = Get<GameObject>((int)GameObjects.ShopText);
        moveArea = Get<GameObject>((int)GameObjects.Shop);
        BindEvent(movePoint, (PointerEventData data) => { moveArea.transform.position = data.position; }, Define.UIEvent.Drag);

        Managers.Game._shop = this;
    }

    private void Start()
    {
        AddItemInShop();
    }

    private void AddItemInShop()
    {
        int n = 0;
        for (int i = 0; i < Managers.Database.ItemDictionary.Count; ++i) 
        {
            if(Managers.Database.ItemDictionary[i].itemType != Item.ItemType.ETC)
            {
                slots[n].AddItem(Managers.Database.ItemDictionary[i]);
                n++;
            }
        }
    }
}
