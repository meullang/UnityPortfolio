using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : MonsterController
{
    bool skillCheck = false;
    float maxSpeed = 20.0f;
    float originSpeed = 3.5f;

    public override void Init()
    {
        base.Init();

        WorldObjectType = Define.WorldObject.Orc;
        _stat = gameObject.GetComponent<MonsterStat>();

        _stat.SetStat((int)WorldObjectType);
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
        else if (distance <= traceDist && !skillCheck)
        {
            return Define.State.Skill;
        }
        else if (distance <= attackDist)
        {
            agent.speed = originSpeed;
            return Define.State.Attack;
        }
        else if (distance <= traceDist)
        {
            return Define.State.Moving;
        }
        else
        {
            skillCheck = false;
            agent.speed = originSpeed;
            return Define.State.Idle;
        }
    }

    protected override void UseSkill()
    {
        agent.isStopped = true;
        skillCheck = true;
        agent.speed = maxSpeed;
        anim.SetTrigger(hashSkill);
        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/OrcRoar");
    }//공격하면 이속 초기화 평소상태가 되면 이속과 스킬 초기화

    protected override void MonsterDead()
    {
        base.MonsterDead();

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
