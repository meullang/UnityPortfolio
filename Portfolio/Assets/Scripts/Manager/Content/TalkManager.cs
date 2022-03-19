using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkManager
{
    public Dictionary<int, string[]> talkData;

    public void Init()
    {
        talkData = Managers.Data.NPCTalkDict;
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (!talkData.ContainsKey(id))
        {
            if(!talkData.ContainsKey(id - id % 10))
            {
                return GetTalk(id - id % 100, talkIndex);
            }
            else
            {
                return GetTalk(id - id % 10, talkIndex);
            }
        }

        if (talkIndex == talkData[id].Length)
            return null;
        else
            return talkData[id][talkIndex];
    }
}
