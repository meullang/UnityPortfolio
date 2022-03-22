using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insect : MonsterController
{
    [SerializeField]
    private float _skillDist = 10;

    public override void Init()
    {
        base.Init();

        WorldObjectType = Define.WorldObject.Insect;
        stat = gameObject.GetComponent<MonsterStat>();

        stat.SetStat((int)WorldObjectType);
    }

    void Start()
    {
        skillTime = 0f;
        hittedTime = 0.3f;
        attackDist = 3f;
    }

    protected override Define.State ChangeState()
    {
        if (hitted == true)
        {
            return Define.State.Hit;
        }
        else if (distance <= attackDist)
        {
            return Define.State.Attack;
        }
        else if(distance <= _skillDist)
        {
            return Define.State.Skill;
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
        isLook = true;
        agent.isStopped = true;
        anim.SetBool(hashAttack, false);
        anim.SetBool(hashSkill, true);
    }

    protected override void MakeDropItem()
    {
        base.MakeDropItem();

        GameObject dropItem = Instantiate(Managers.Database.ItemList[5].itemPrefab, this.transform.position, Quaternion.identity);
        Destroy(dropItem.gameObject, 20.0f);
    }

    void DoSkill()
    {
        Vector3 dir = _targetTr.transform.position - transform.position;
        monsterSkill = Managers.Resource.Instantiate("Particle/MonsterSkill/InsectSkill");
        monsterSkill.transform.position = transform.position;
        monsterSkill.transform.rotation = transform.rotation * Quaternion.Euler(new Vector3(-90, -90, 0));
        StartCoroutine(RemoveSkill());
    }

    protected override void MonsterDead()
    {
        base.MonsterDead();

        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/InsectDead");
    }

    public override void Moaning()
    {
        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/InsectMoan");
    }

    IEnumerator RemoveSkill()
    {
        yield return new WaitForSeconds(1.5f);
        Managers.Resource.Destroy(monsterSkill.gameObject);
    }

    void Insect_EnableWeapon() => monsterWeapon.GetComponent<Collider>().enabled = true;

    void Insect_DisableWeapon() => monsterWeapon.GetComponent<Collider>().enabled = false;
}
