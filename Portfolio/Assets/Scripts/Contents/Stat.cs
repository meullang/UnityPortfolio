using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stat : MonoBehaviour
{
    [SerializeField]
    protected int _level;
    [SerializeField]
    protected int _hp;
    [SerializeField]
    protected int _maxHp;
    [SerializeField]
    protected int _mp;
    [SerializeField]
    protected int _maxMp;
    [SerializeField]
    protected int _attack;
    [SerializeField]
    protected int _defence;
    [SerializeField]
    protected bool _isDead;

    [SerializeField]
    protected int _addAttack;
    [SerializeField]
    protected int _addDefnece;

    public int Level { get { return _level; } set { _level = value; } }
    public int Hp { get { return _hp; } set { _hp = value; } }
    public int MaxHp { get { return _maxHp; } set { _maxHp = value; } }
    public int Mp { get { return _mp; } set { _mp = value; } }
    public int MaxMp { get { return _maxMp; } set { _maxMp = value; } }
    public int Attack { get { return _attack + _addAttack; } set { _attack = value; } }
    public int AddAttack { get { return _addAttack; } set { _addAttack = value; } }
    public int Defence { get { return _defence + _addDefnece; } set { _defence = value; } }
    public int AddDefence { get { return _addDefnece; } set { _addDefnece = value; } }
    public bool IsDead { get { return _isDead; } set { _isDead = value; } }

    public virtual void OnAttacked(Stat attacker, int getDamage)
    {
        int damage = Mathf.Max(0, getDamage - Defence);
        _hp -= damage;
        if (_hp < 0)
        {
            _hp = 0;
            if (!IsDead)
            {
                OnDead(attacker);
            }
        }
    }

    protected virtual void OnDead(Stat attacker) { }
}
