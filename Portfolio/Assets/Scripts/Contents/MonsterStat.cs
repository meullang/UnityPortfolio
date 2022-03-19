using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : Stat
{
    [SerializeField]
    protected int _exp;

    public int Gold { get; }
    public int Exp { get; }

    private void Start()
    {
        _addAttack = 0;
        _addDefnece = 0;
    }

    public void SetStat(int level) // 몬스터 타입에 따른 딕셔러리 갱신
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.MonsterStatDict;
        Data.Stat stat = dict[level];

        _level = stat.level;
        _hp = stat.maxHp;
        _maxHp = stat.maxHp;
        _mp = stat.maxMp;
        _maxMp = stat.maxMp;
        _attack = stat.attack;
        _defence = stat.defence;
        _exp = stat.totalExp;
    }

    protected override void OnDead(Stat attacker)
    {
        PlayerStat playerStat = attacker as PlayerStat;
        if (playerStat != null)
        {
            playerStat.Exp += _exp;
        }

        IsDead = true;
    }
}
