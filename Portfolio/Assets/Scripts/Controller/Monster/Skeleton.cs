using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonsterController
{
    [SerializeField]
    bool skillCheck = false;
    [SerializeField]
    float skillDuration = 20.0f;
    GameObject loadSkill;

    public override void Init()
    {
        base.Init();

        WorldObjectType = Define.WorldObject.Skeleton;
        _stat = gameObject.GetComponent<MonsterStat>();

        _stat.SetStat((int)WorldObjectType);

        loadSkill = Managers.Resource.Load<GameObject>("Prefabs/Particle/MonsterSkill/Skeleton_Skill");
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
        else if(distance <= traceDist && _stat.Hp <= (_stat.MaxHp / 2) && !skillCheck)
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
        skillCheck = true;

        agent.isStopped = true;

        monsterSkill = Managers.Resource.Instantiate("Particle/MonsterSkill/Skeleton_Skill", transform);
        monsterSkill.transform.position = transform.position;

        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/SkeletonSkill");

        StartCoroutine(GetAttackPoint());
    }

    private IEnumerator GetAttackPoint()
    {
        _stat.AddAttack = 10;
        _stat.AddDefence = 10;
        yield return new WaitForSeconds(skillDuration);

        Managers.Resource.Destroy(monsterSkill.gameObject);
        skillCheck = false;

        _stat.AddAttack = 0;
        _stat.AddDefence = 0;

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

        _stat.AddAttack = 0;
        _stat.AddDefence = 0;

        skillCheck = false;

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
