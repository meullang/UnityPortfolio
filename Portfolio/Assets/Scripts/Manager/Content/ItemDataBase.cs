using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase
{
    public List<Item> ItemList = new List<Item>();
    public Dictionary<int, Item> ItemDictionary = new Dictionary<int, Item>();
    public GameObject money;

    public UI_Inven _Inven;
    public UI_PlayerInfo _PlayerInfo;
    public PlayerStat _playerStat;

    private const string HP = "HP", MP = "MP", SP = "SP";

    public void Init()
    {
        Generate();
    }

    private void Generate()
    {
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Potion/HealthPotion"));      //0 
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Potion/ManaPotion"));        //1
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Potion/StaminaPotion"));     //2
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Ingredient/SkeletonSkul"));  //3
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Ingredient/OrcMeat"));       //4
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Ingredient/InsectEgg"));     //5
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/ETC/Key"));                  //6
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Wear/LeatherArmor"));        //7
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Wear/IronArmor"));           //8
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Wear/Emerald"));             //9
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Wear/Ruby"));                //10
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Wear/Sapphire"));            //11
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/ETC/Letter"));               //12
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Weapon/Sword"));             //13
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Weapon/GreatSword"));        //14
        ItemList.Add(Managers.Resource.Load<Item>("Data/Item/Weapon/Polearm"));           //15

        money = Managers.Resource.Load<GameObject>("Prefabs/Item_Prefab/Crystal");

        foreach (Item items in ItemList)
            ItemDictionary.Add(items.itemCode, items);
    }

    public void UseItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.Used)
        {
            _playerStat.Hp += _item.giveHp;
            _playerStat.Mp += _item.giveMp;
            _playerStat.Sp += _item.giveSp;
            Managers.Sound.Play("OutWorld/potiondrink");
        }
    }

    public void GetItem(Item _item)
    {
        _Inven.AcquireItem(_item);
    }
}
