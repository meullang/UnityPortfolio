using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : MonsterController
{
    bool _skillCheck = false;

    float _originSpeed = 3.5f;
    float _maxSpeed = 20.0f;

    public override void Init()
    {
        base.Init();

        WorldObjectType = Define.WorldObject.Orc;
        stat = gameObject.GetComponent<MonsterStat>();

        stat.SetStat((int)WorldObjectType);
    }

    void Start()
    {
        skillTime = 1.8f;
        hittedTime = 1f;
    }

    protected override Define.State ChangeState()
    {
        if (hitted == true)
        {
            return Define.State.Hit;
        }
        else if (distance <= traceDist && !_skillCheck)
        {
            return Define.State.Skill;
        }
        else if (distance <= attackDist)
        {
            agent.speed = _originSpeed;
            return Define.State.Attack;
        }
        else if (distance <= traceDist)
        {
            return Define.State.Moving;
        }
        else
        {
            _skillCheck = false;
            agent.speed = _originSpeed;
            return Define.State.Idle;
        }
    }

    protected override void UseSkill() //Attack이 활성화 될 경우 이속 초기화 Idle상태가 되면 이속과 스킬 초기화
    {
        agent.isStopped = true;
        _skillCheck = true;
        agent.speed = _maxSpeed;
        anim.SetTrigger(hashSkill);
        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/OrcRoar");
    }

    protected override void MonsterDead()
    {
        base.MonsterDead();

        _skillCheck = false;
        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/OrcDead");
    }

    protected override void MakeDropItem()
    {
        base.MakeDropItem();

        GameObject dropItem = Instantiate(Managers.Database.ItemList[4].itemPrefab, this.transform.position, Quaternion.identity);
        Destroy(dropItem.gameObject, 20.0f);
    }

    public override void Moaning()
    {
        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/OrcMoan");
    }

    void Orc_EnableWeapon()
    {
        monsterWeapon.GetComponent<Collider>().enabled = true;
    }

    void Orc_DisableWeapon()
    {
        monsterWeapon.GetComponent<Collider>().enabled = false;
    }
}
