using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager
{
    public int nowQuestID = 10;
    public int questActionIndex;

    public Dictionary<int, Data.Quest> questList;

    public PlayerStat _stat;
    public UI_Inven _Inven;
    public NPCData[] NPCs = new NPCData[10];
    public GameObject QuestNPC;
    public bool isDoneCheck = false;
    public bool isQuestComplete = false;

    public GamePlayUI _questNote;

    public void Init()
    {
        QuestNPC = Managers.Resource.Load<GameObject>("Prefabs/NPC/npcE Quest");
        questList = Managers.Data.QuestDict;
    }

    public void SetFirstQuest()
    {
        NPCs[questList[nowQuestID].npcID[1] / 1000].QuestMarkAppear();
        _questNote.SetQuestName(questList[nowQuestID].questName[questActionIndex]);
        if(questActionIndex == 0)
        {
            _questNote.SetQuestSit("다음 퀘스트");
        }
        else
        {
            _questNote.SetQuestSit("현재 퀘스트");
        }
    }

    public int GetQuestTalkIndex(int id)
    {
        return nowQuestID + questActionIndex;
    }

    public void CheckQuest(int id)
    {
        if (questList[nowQuestID].npcID[1] == 0000)
            return;

        if (id == questList[nowQuestID].npcID[questActionIndex + 1])
        {
            if (RequireCondition())
            {
                if (questActionIndex < questList[nowQuestID].npcID.Length)
                {
                    questActionIndex++;

                    _questNote.SetQuestName(questList[nowQuestID].questName[questActionIndex]);
                    _questNote.SetQuestSit("현재 퀘스트");
                }

                NPCs[questList[nowQuestID].npcID[questActionIndex] / 1000].QuestMarkDisappear();

                if (questActionIndex == questList[nowQuestID].npcID.Length - 1)
                {
                    if(questList[nowQuestID + 10].npcID[1] != 0000)
                    {
                        NPCs[questList[nowQuestID + 10].npcID[1] / 1000].QuestMarkAppear();
                    }
                }
                else
                {
                    NPCs[questList[nowQuestID].npcID[questActionIndex + 1] / 1000].QuestMarkAppear();
                }
            }
        }

        if (questActionIndex == questList[nowQuestID].npcID.Length - 1)
        {
            isQuestComplete = true;
        }
    }

    public void NextQuest()
    {
        _stat.Exp += questList[nowQuestID].giveExp;
        Managers.Notify.SetNotification($"{questList[nowQuestID].giveExp}의 경험치를 얻었습니다.");

        nowQuestID += 10;
        questActionIndex = 0;
        isQuestComplete = false;
        Managers.Sound.Play("quest-done");

        _questNote.SetQuestName(questList[nowQuestID].questName[questActionIndex]);
        _questNote.SetQuestSit("다음 퀘스트");
    }

    bool RequireCondition()
    {
        switch (nowQuestID)
        {
            case 10:
                return Quest_10();
            case 20:
                return Quest_20();
            case 30:
                return Quest_30();
            case 40:
                return Quest_40();
            default:
                return false;
        }
    }

    bool Quest_10()
    {
        Item questItem = Managers.Database.ItemDictionary[12];
        if (questActionIndex == 0)
        {
            _Inven.AcquireItem(questItem);
            return true;
        }
        else if (questActionIndex == 1 && _Inven.CheckItemIndex(questItem.itemCode) != -1)
        {
            _Inven.slots[_Inven.CheckItemIndex(questItem.itemCode)].ClearSlot();
            _Inven.AcquireItem(Managers.Database.ItemDictionary[6], 2);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool Quest_20()
    {
        Item questItem = Managers.Database.ItemDictionary[4];
        if (questActionIndex == 0)
        {
            return true;
        }
        else if (questActionIndex == 1 && _Inven.CheckItemIndex(questItem.itemCode) != -1)
        {
            if (_Inven.slots[_Inven.CheckItemIndex(questItem.itemCode)].itemCount >= 5)
            {
                _Inven.slots[_Inven.CheckItemIndex(questItem.itemCode)].SetSlotCount(-5);
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    GameObject QuestObject;
    bool Quest_30()
    {
        if (questActionIndex == 0)
        {
            NPCs[5].gameObject.SetActive(false);
            QuestObject = Managers.Instantiate(QuestNPC);
            return true;
        }
        else if (questActionIndex == 1)
        {
            return true;
        }
        else if (questActionIndex == 2 && isDoneCheck)
        {
            NPCs[5].gameObject.SetActive(true);
            Managers.Destroy(QuestObject.gameObject);

            _Inven.AcquireItem(Managers.Database.ItemDictionary[15]);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool Quest_40()
    {
        if (questActionIndex == 0)
        {
            return true;
        }
        else if (questActionIndex == 1 && Managers.Game.bossAlive == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Clear()
    {
        nowQuestID = 10;
        questActionIndex = 0;

        for(int i = 0; i < NPCs.Length; i++)
        {
            NPCs[i] = null;
        }
    }
}
