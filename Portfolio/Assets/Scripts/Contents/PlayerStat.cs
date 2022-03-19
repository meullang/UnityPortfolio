using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : Stat
{
    [SerializeField]
    protected int _skillPoint;
    [SerializeField]
    protected float _sp;
    [SerializeField]
    protected float _maxSp;
    [SerializeField]
    protected int _exp;
    [SerializeField]
    protected int _totalExp;
    [SerializeField]
    protected int _preTotalExp;

    public int SkillPoint { get { return _skillPoint; } set { _skillPoint = value; } }
    public float Sp { get { return _sp; } set { _sp = value; } }
    public float MaxSp{ get { return _maxSp; } set { _maxSp = value; } }
    public int Exp 
    { 
        get { return _exp; } 
        set  // 경험치를 받는 과정에서 조건에 충족하면 자동 레벨 업
        {
            if(Level >= 5)
            {
                return;
            }

            _exp = value;

            int level = Level;

            if (_exp >= _totalExp)
                ++level;

            while (true)
            {
                Data.Stat stat;
                if (Managers.Data.StatDict.TryGetValue(level + 1, out stat) == false)
                    break;
                if (_exp < stat.totalExp)
                    break;
                ++level;   
            }

            if(level != Level)
            {
                Debug.Log("Level up!");

                GameObject levelUpEffet = Managers.Resource.Load<GameObject>("Prefabs/Particle/LevelUpEffect");
                GameObject effect = Instantiate(levelUpEffet, this.transform.position, Quaternion.identity);
                Destroy(effect.gameObject, 1f);

                Managers.Sound.Play("OutWorld/Levelup");

                Managers.Notify.SetNotification("Level UP !!!");
                
                if(Level >= 5)
                {
                    Level = 5;
                    Managers.Notify.SetNotification("만렙이 되어 더이상 경험치를 획득할 수 없습니다.");
                    return;
                }

                Level = level;
                SetStat(Level);
            }
        } 
    }
    public int TotalExp { get { return _totalExp; } set { _totalExp = value; } }
    public int PreTatalExp { get { return _preTotalExp; } set { _preTotalExp = value; } }

    private void Awake()
    {
        _level = 1;

        _exp = 0;
        _totalExp = 0;
        _preTotalExp = 0;

        _maxSp = 100;

        _addAttack = 0;
        _addDefnece = 0;

        _skillPoint = 0;
        
        SetStat(_level);

        Managers.SaveLoad.thePlayer = this;
        Managers.Quest._stat = this;
    }

    public void SetStat(int level) // 레벨에 따른 딕셔러리 갱신
    {
        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        Data.Stat stat = dict[level];

        _preTotalExp = TotalExp;

        _hp = stat.maxHp;
        _maxHp = stat.maxHp;
        _mp = stat.maxMp;
        _maxMp = stat.maxMp;
        _attack = stat.attack;
        _defence = stat.defence;
        _totalExp = stat.totalExp;

        _skillPoint++;
        _sp = _maxSp;
    }

    protected override void OnDead(Stat attacker)
    {
        IsDead = true;

        StartCoroutine(GoTitleByDead());
        StartCoroutine(ScreenGetGray());
    }

    private IEnumerator GoTitleByDead()
    {
        Managers.Notify.SetNotification("플레이어가 죽었습니다");
        yield return new WaitForSeconds(3f);
        Managers.Notify.SetNotification("5초 후 타이틀로 돌아갑니다.");
        yield return new WaitForSeconds(5f);
        Managers.Scene.LoadScene(Define.Scene.Title);
    }

    IEnumerator ScreenGetGray()
    {
        Image GetGray = GameObject.Find("Dead").GetComponent<Image>();
        Color color = GetGray.color;
        color.a = 0f;
        GetGray.color = color;
        while (color.a < 0.5)
        {
            color.a += 0.005f;
            GetGray.color = color;
            yield return null;
        }
    }
}
