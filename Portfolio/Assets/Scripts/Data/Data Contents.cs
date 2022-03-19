using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Stat
    [Serializable]
    public class Stat  //json 파일의 내용을 표시
    {
        public int level;
        public int maxHp;
        public int maxMp;
        public int attack;
        public int defence;
        public int totalExp;
    }

    [Serializable]
    public class Talk
    {
        public int index;
        public string[] talks;
    }

    [Serializable]
    public class Quest
    {
        public string[] questName;
        public int questID;
        public int[] npcID;
        public int giveExp;
    }

    [Serializable]
    public class StatData : ILoader<int, Stat> // json파일의 내용을 딕셔너리에 옮김
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)
                dict.Add(stat.level, stat); //레벨을 키로 받아서 나머지 값을 저장함
            return dict;
        }
    }

    [Serializable]
    public class TalkData : ILoader<int, string[]>
    {
        public List<Talk> talks = new List<Talk>();

        public Dictionary<int, string[]> MakeDict()
        {
            Dictionary<int, string[]> dict = new Dictionary<int, string[]>();

            foreach (Talk talk in talks)
                dict.Add(talk.index, talk.talks);
            return dict;
        }
    }

    [Serializable]
    public class QuestData : ILoader<int, Quest>
    {
        public List<Quest> quests = new List<Quest>();

        public Dictionary<int, Quest> MakeDict()
        {
            Dictionary<int, Quest> dict = new Dictionary<int, Quest>();

            foreach (Quest quest in quests)
                dict.Add(quest.questID, quest);
            return dict;
        }
    }

    #endregion
}