using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonsterController
{
    [SerializeField]
    bool _skillCheck = false;
    [SerializeField]
    float _skillDuration = 20.0f;

    public override void Init()
    {
        base.Init();

        WorldObjectType = Define.WorldObject.Skeleton;
        stat = gameObject.GetComponent<MonsterStat>();

        stat.SetStat((int)WorldObjectType);
    }

    void Start()
    {
        skillTime = 1.1f;
        hittedTime = 0.3f;
    }

    protected override Define.State ChangeState()
    {
        if (hitted == true)
        {
            return Define.State.Hit;
        }
        else if(distance <= traceDist && stat.Hp <= (stat.MaxHp / 2) && !_skillCheck)
        {
            return Define.State.Skill;
        }
        else if (distance <= attackDist)
        {
            return Define.State.Attack;
        }
        else if (distance <= traceDist)
        {
            return Define.State.Moving;
        }
        else
        {
            return Define.State.Idle;
        }
    }

    protected override void UseSkill()
    {
        anim.SetTrigger(hashSkill);
        _skillCheck = true;

        agent.isStopped = true;

        monsterSkill = Managers.Resource.Instantiate("Particle/MonsterSkill/Skeleton_Skill", transform);
        monsterSkill.transform.position = transform.position;

        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/SkeletonSkill");

        StartCoroutine(GetAttackPoint());
    }

    private IEnumerator GetAttackPoint()
    {
        stat.AddAttack = 10;
        stat.AddDefence = 10;
        yield return new WaitForSeconds(_skillDuration);

        Managers.Resource.Destroy(monsterSkill.gameObject);
        _skillCheck = false;

        stat.AddAttack = 0;
        stat.AddDefence = 0;

    }

    protected override void MakeDropItem()
    {
        base.MakeDropItem();

        GameObject dropItem = Instantiate(Managers.Database.ItemList[3].itemPrefab, this.transform.position, Quaternion.identity);
        Destroy(dropItem.gameObject, 20.0f);

    }

    protected override void MonsterDead()
    {
        base.MonsterDead();

        stat.AddAttack = 0;
        stat.AddDefence = 0;

        _skillCheck = false;

        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/SkeletonDead");
    }

    public override void Moaning()
    {
        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/SkeletonMoan");
    }

    //애니메이션 컨트롤러

    void Skeleton_EnableWeapon()
    {
        monsterWeapon.GetComponent<Collider>().enabled = true;
    }

    void Skeleton_DisableWeapon()
    {
        monsterWeapon.GetComponent<Collider>().enabled = false;
    }
}
