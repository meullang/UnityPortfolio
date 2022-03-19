using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public Vector3 playerRot;

    public int playerLevel;
    public int playerExp;
    public int playerHp;
    public int playerMp;

    public int playerSkillPoint;

    public List<int> skillLevel = new List<int>();
    public List<int> equipmentNumber = new List<int>();
    public List<int> equipmentItemCode = new List<int>();

    public List<int> invenArrayNumber = new List<int>();
    public List<int> invenItemCode = new List<int>();
    public List<int> invenItemNumber = new List<int>();
    public int money;

    public List<int> quickArrayNumber = new List<int>();
    public List<int> quickItemCode = new List<int>();
    public List<int> quickItemNumber = new List<int>();

    public int nowQuestID;
    public int nowQuestIndex;
}

public class SaveLoadManager
{
    public bool Load = false;

    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    public PlayerStat thePlayer;
    public UI_Inven theInven;
    public UI_PlayerInfo theInfo;
    public GamePlayUI theGameUI;

    void Init()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        if (!Directory.Exists(SAVE_DATA_DIRECTORY))
        {
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
        }
    }

    public void SaveData()
    {
        saveData.playerLevel = thePlayer.Level;
        saveData.playerExp = thePlayer.Exp;
        saveData.playerHp = thePlayer.Hp;
        saveData.playerMp = thePlayer.Mp;

        saveData.playerSkillPoint = thePlayer.SkillPoint;
        for (int i = 0; i < Managers.Skill.playerSkillLevel.Length; ++i)
        {
            saveData.skillLevel.Add(Managers.Skill.playerSkillLevel[i]);
        }

        UI_Slot[] equipmentSlots = theInfo.GetSlots();
        for (int i = 0; i < equipmentSlots.Length; ++i)
        {
            if (equipmentSlots[i].item != null)
            {
                saveData.equipmentNumber.Add(i);
                saveData.equipmentItemCode.Add(equipmentSlots[i].item.itemCode);
            }
        }

        UI_Slot[] invenSlots = theInven.GetSlots();
        for (int i = 0; i < invenSlots.Length; ++i)
        {
            if (invenSlots[i].item != null)
            {
                saveData.invenArrayNumber.Add(i);
                saveData.invenItemCode.Add(invenSlots[i].item.itemCode);
                saveData.invenItemNumber.Add(invenSlots[i].itemCount);
            }
        }

        UI_Slot[] quickSlots = theGameUI.GetSlots();
        for (int i = 0; i < quickSlots.Length; ++i)
        {
            if (quickSlots[i].item != null)
            {
                saveData.quickArrayNumber.Add(i);
                saveData.quickItemCode.Add(quickSlots[i].item.itemCode);
                saveData.quickItemNumber.Add(quickSlots[i].itemCount);
            }
        }

        saveData.money = theInven.Money;

        saveData.nowQuestID = Managers.Quest.nowQuestID;
        saveData.nowQuestIndex = Managers.Quest.questActionIndex;

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료");
    }

    public void LoadData()
    {
        if (File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            thePlayer.Level = saveData.playerLevel;
            thePlayer.GetComponent<PlayerStat>().SetStat(saveData.playerLevel);
            thePlayer.Exp = saveData.playerExp;
            thePlayer.Hp = saveData.playerHp;
            thePlayer.Mp = saveData.playerMp;

            for (int i = 0; i < Managers.Skill.playerSkillLevel.Length; ++i)
            {
                Managers.Skill.playerSkillLevel[i] = saveData.skillLevel[i];
            }

            for (int i = 0; i < saveData.invenItemCode.Count; ++i)
            {
                theInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemCode[i], saveData.invenItemNumber[i]);
            }

            for (int i = 0; i < saveData.equipmentItemCode.Count; ++i)
            {
                theInfo.LoadToEquipment(saveData.equipmentNumber[i], saveData.equipmentItemCode[i]);
            }

            for(int i=0;i<saveData.quickItemCode.Count; ++i)
            {
                theGameUI.LoadToQuick(saveData.quickItemNumber[i], saveData.quickItemCode[i], saveData.quickItemNumber[i]);
            }

            theInven.Money = saveData.money;

            Managers.Quest.nowQuestID = saveData.nowQuestID;
            Managers.Quest.questActionIndex = saveData.nowQuestIndex;
            Managers.Notify.SetNotification("저장 기록이 로드되었습니다.");
        }
        else
        {
            Managers.Notify.SetNotification("저장된 기록이 없습니다.");
        }
    }
}
