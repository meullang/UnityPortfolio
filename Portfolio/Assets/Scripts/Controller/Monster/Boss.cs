using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    #region setting
    private MonsterStat _stat;
    private Transform _targetTr;
    private NavMeshAgent agent;
    private Animator anim;
    protected CameraController cam;

    //애니메이션 해시
    private readonly int hashSleep = Animator.StringToHash("Sleep");
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashNormalAttack = Animator.StringToHash("IsNormalAttack");
    private readonly int hashCloseAttack = Animator.StringToHash("IsCloseAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashSkillOne = Animator.StringToHash("SkillOne");
    private readonly int hashSkillTwo = Animator.StringToHash("SkillTwo");
    private readonly int hashDie = Animator.StringToHash("Die");

    [SerializeField]
    private Image ScreenEffect;

    [SerializeField]
    private Light lt;

    #endregion
    //몬스터의 무기
    [SerializeField]
    private GameObject[] monsterWeapon = new GameObject[3];

    GameObject _hpBar;

    //상태 값
    [SerializeField]
    private Define.BossState _state = Define.BossState.Sleep;
    public Define.WorldObject WorldObjectType { get; private set; } = Define.WorldObject.Unknown;

    //공격, 추적 범위의 값
    private float distance;
    [SerializeField]
    private float traceDist = 30.0f;
    [SerializeField]
    private float normalAttackDist = 6.0f;
    [SerializeField]
    private float closeAttackDist = 3.0f;

    //애니메이션과 코드 매칭을 위한 애니메이션 재생 시간
    private readonly float attackOneTime = 2.5f;
    private readonly float attackTwoTime = 2.5f;
    private readonly float skillOneTime = 3f;
    private readonly float skillTwoTime = 1.5f;
    private readonly float hittedTime = 1.5f;

    private float lookTime = 1.0f;

    //상태 변수
    private bool isDie = false;
    private bool isSleep = false;
    private bool hitted = true;

    void Awake()
    {
        _targetTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        cam = Camera.main.GetComponent<CameraController>();

        _stat = GetComponent<MonsterStat>();
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.destination = _targetTr.position;

        WorldObjectType = Define.WorldObject.Boss;
        _stat.SetStat((int)WorldObjectType);

        if (gameObject.GetComponentInChildren<UI_BossHP>() == null)
            Managers.UI.MakeSubItem<UI_BossHP>(transform);

        _hpBar = transform.Find("UI_BossHP").gameObject;

        ScreenEffect = GameObject.Find("Attack").GetComponent<Image>();
        lt = GameObject.Find("Directional Light").GetComponent<Light>();
    }

    void OnEnable()
    {
        StartCoroutine(CheckMonsterBossState());
        StartCoroutine(MonsterAction());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private bool isLook = false;
    void Update()
    {
        if (agent.remainingDistance >= 2f)
        {
            Vector3 direction = agent.desiredVelocity;
            if (direction != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10.0f);
            }
        }

        if(isLook)
        {
            Quaternion rot = Quaternion.LookRotation(_targetTr.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10.0f);
        }

        if(distance < 40)
        {
            _hpBar.gameObject.SetActive(true);
        }
        else
        {
            _hpBar.gameObject.SetActive(false);
        }
    }

    private IEnumerator CheckMonsterBossState()
    {
        while (!isDie)
        {
            yield return null;

            if (_state == Define.BossState.Die) yield break;

            distance = Vector3.Distance(_targetTr.position, transform.position);

            _state = ChangeBossState();
        }
    }

    private Define.BossState ChangeBossState()
    {
        if (isSleep)
        {
            return Define.BossState.Sleep;
        }
        else if(_stat.Hp < (_stat.MaxHp / 2) && hitted)
        {
            hitted = false;
            return Define.BossState.Hit;
        }
        else if(Managers.Skill.bossLeftCoolingTime[0] == 0 && distance < 10)
        {
            return Define.BossState.Skill_1;
        }
        else if (Managers.Skill.bossLeftCoolingTime[1] == 0 && _stat.Hp < (_stat.MaxHp / 2))
        {
            return Define.BossState.Skill_2;
        }
        else if (distance <= closeAttackDist)
        {
            return Define.BossState.Attack_1;
        }
        else if (distance <= normalAttackDist)
        {
            return Define.BossState.Attack_2;
        }
        else if(distance <= traceDist)
        {
            return Define.BossState.Moving;
        }
        else
        {
            return Define.BossState.Idle;
        }
    }

    
    private IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (_state)
            {
                case Define.BossState.Sleep:
                    agent.isStopped = true;
                    break;
                case Define.BossState.Idle:
                    BeIdle();
                    break;
                case Define.BossState.Moving:
                    agent.SetDestination(_targetTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    break;

                case Define.BossState.Hit:
                    anim.SetTrigger(hashHit);
                    agent.isStopped = true;
                    yield return new WaitForSeconds(hittedTime);
                    hitted = false;
                    break;

                case Define.BossState.Attack_1:
                    isLook = true;
                    yield return new WaitForSeconds(lookTime);
                    isLook = false;
                    AttackOne();
                    yield return new WaitForSeconds(attackOneTime);
                    break;
                case Define.BossState.Attack_2:
                    isLook = true;
                    yield return new WaitForSeconds(lookTime);
                    isLook = false;
                    AttackTwo();
                    yield return new WaitForSeconds(attackTwoTime);
                    break;
                case Define.BossState.Skill_1:
                    isLook = true;
                    yield return new WaitForSeconds(lookTime);
                    isLook = false;
                    anim.SetTrigger(hashSkillOne);
                    yield return new WaitForSeconds(0.5f);
                    UseSkillOne();
                    yield return new WaitForSeconds(skillOneTime);
                    break;
                case Define.BossState.Skill_2:
                    isLook = true;
                    yield return new WaitForSeconds(lookTime);
                    isLook = false;
                    anim.SetTrigger(hashSkillTwo);
                    yield return new WaitForSeconds(0.5f);
                    UseSkillTwo();
                    yield return new WaitForSeconds(skillTwoTime);
                    break;

                case Define.BossState.Die:
                    agent.isStopped = true;
                    anim.SetTrigger(hashDie);
                    Managers.Game.bossAlive = false;
                    _hpBar.gameObject.SetActive(false);
                    GetComponent<CapsuleCollider>().enabled = false;

                    yield return new WaitForSeconds(7.0f);
                    //재가공
                    Managers.Game.Despawn(gameObject);
                    break;
            }

            yield return null;
        }
    }

    private void BeIdle()
    {
        agent.isStopped = true;
    }

    private void UseSkillOne()
    {
        agent.isStopped = true; 
        GameObject skillobj = Instantiate(Managers.Skill.BossUseSkill(0), transform.position, transform.rotation);
        Destroy(skillobj.gameObject, 2.0f);
        
    }
    private void UseSkillTwo()
    {
        agent.isStopped = true;
        GameObject skillobj = Instantiate(Managers.Skill.BossUseSkill(1), transform.position - new Vector3(0, 0, 2), Quaternion.identity);
        Destroy(skillobj.gameObject, 2.0f);
    }

    private void AttackOne()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        anim.SetTrigger(hashCloseAttack);
    }

    private void AttackTwo()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        anim.SetTrigger(hashNormalAttack);
    }

    public void Moaning()
    {
        Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/SkeletonMoan");
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("PLAYER_ATTACK"))
        {
            _hpBar.SetActive(true);
            anim.SetTrigger(hashHit);

            Vector3 pos = coll.GetComponent<Transform>().position;
            MakeBlood(pos);

            StartCoroutine(ScreenEffectControl());

            if (coll.transform.parent.GetComponent<SkillObject>() == null)
            {
                Transform _player = coll.transform.parent;
                while (_player.GetComponent<PlayerStat>() == null)
                {
                    _player = _player.parent;
                }

                PlayerStat _playerStat = _player.GetComponent<PlayerStat>();
                _stat.OnAttacked(_playerStat, _playerStat.Attack);

                StartCoroutine(TimeEffect());
                StartCoroutine(cam.Shake(0.2f, 0.1f));
            }
            else
            {
                Transform skillObj = coll.transform.parent;
                SkillObject _skill = skillObj.GetComponent<SkillObject>();

                Stat _playerStat = _skill.skillUser;

                _stat.OnAttacked(_playerStat, _skill.damage);
            }

            if (_stat.IsDead)
                _state = Define.BossState.Die;

            hitted = true;
        }
    }

    GameObject blood;
    protected void MakeBlood(Vector3 pos)
    {
        blood = Managers.Resource.Instantiate("Particle/BloodExplosion");
        blood.transform.position = pos;
        Invoke("RemoveBlood", 0.5f);
    }

    protected void RemoveBlood()
    {
        Managers.Resource.Destroy(blood.gameObject);
    }

    void Boss_EnableWeapon(int n)
    {
        monsterWeapon[n].GetComponent<Collider>().enabled = true;
        if(n == 2)
        {
            Managers.Sound.PlayAtPoint(gameObject, "BossSound/BossKnockBack");
        }
        else
        {
            Managers.Sound.PlayAtPoint(gameObject, "BossSound/BossNormalAttack");
        }
    }

    void Boss_DisableWeapon(int n)
    {
        monsterWeapon[n].GetComponent<Collider>().enabled = false;
    }

    protected IEnumerator TimeEffect()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
    }

    IEnumerator ScreenEffectControl()
    {
        Color color = ScreenEffect.color;
        color.a = 0.3f;
        ScreenEffect.color = color;

        lt.intensity = 2;

        while (color.a > 0)
        {
            if (lt.intensity <= 1)
            {
                lt.intensity = 1;
            }
            else
            {
                lt.intensity -= 0.05f;
            }
            color.a -= 0.005f;
            ScreenEffect.color = color;
            yield return null;
        }
    }
}
