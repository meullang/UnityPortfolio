using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Stat
    [Serializable]
    public class Stat  //json ������ ������ ǥ��
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
    public class StatData : ILoader<int, Stat> // json������ ������ ��ųʸ��� �ű�
    {
        public List<Stat> stats = new List<Stat>();

        public Dictionary<int, Stat> MakeDict()
        {
            Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
            foreach (Stat stat in stats)
                dict.Add(stat.level, stat); //������ Ű�� �޾Ƽ� ������ ���� ������
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