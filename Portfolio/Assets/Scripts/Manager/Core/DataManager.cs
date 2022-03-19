using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value> // ��ӵ� ��鿡�� �̰��� �����ؾ��Ѵٴ� ����
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.Stat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();
    public Dictionary<int, Data.Stat> MonsterStatDict { get; private set; } = new Dictionary<int, Data.Stat>();
    public Dictionary<int, string[]> NPCTalkDict { get; private set; } = new Dictionary<int, string[]>();
    public Dictionary<int, Data.Quest> QuestDict { get; private set ; } = new Dictionary<int, Data.Quest>();

    public void Init()
    {
        StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();
        MonsterStatDict = LoadJson<Data.StatData, int, Data.Stat>("MonsterStat").MakeDict();
        NPCTalkDict = LoadJson<Data.TalkData, int, string[]>("NPCTalkData").MakeDict();
        QuestDict = LoadJson<Data.QuestData, int, Data.Quest>("QuestData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
    }
}