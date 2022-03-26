using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour
{
    #region setting
    protected MonsterStat stat;
    protected NavMeshAgent agent;
    protected Animator anim;

    //애니메이션 해시
    protected readonly int hashTrace = Animator.StringToHash("IsTrace");
    protected readonly int hashAttack = Animator.StringToHash("IsAttack");
    protected readonly int hashHit = Animator.StringToHash("Hit");
    protected readonly int hashSkill = Animator.StringToHash("Skill");
    protected readonly int hashDie = Animator.StringToHash("Die");
    protected readonly int hashReady = Animator.StringToHash("Ready");
    #endregion
    //몬스터의 공격
    [SerializeField]
    protected GameObject monsterWeapon;
    protected GameObject monsterSkill;
    private CameraController _cam;

    //가장 가까운 표적 찾기
    [SerializeField]
    protected List<GameObject> FoundObjects;
    [SerializeField]
    protected GameObject _targetTr;
    [SerializeField]
    protected float shortDis;

    //상태 값
    [SerializeField]
    protected Define.State _state = Define.State.Idle;
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;


    [SerializeField]
    private Image ScreenEffect;
    [SerializeField]
    private Light lt;

    //공격, 추적 범위의 값
    protected float distance;
    [SerializeField]
    protected float traceDist = 20.0f;
    [SerializeField]
    protected float attackDist = 2.0f;

    //애니메이션과 코드 매칭을 위한 애니메이션 재생 시간
    protected float hittedTime;
    protected float skillTime;

    //체력바 온 오프를 위한 변수
    GameObject _hpBar;

    //상태 변수
    protected bool isDie = false;
    protected bool hitted = false;

    void Awake()
    {
        Init();

        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.destination = _targetTr.transform.position;
        _cam = Camera.main.GetComponent<CameraController>();
        ScreenEffect = GameObject.Find("Attack").GetComponent<Image>();
        lt = GameObject.Find("Directional Light").GetComponent<Light>();
    }

    public virtual void Init()
    {
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

        if (gameObject.GetComponentInChildren<UI_HPBar>() == true)
        {
            _hpBar = transform.Find("UI_HPBar").gameObject;
            _hpBar.SetActive(false);
        }

        _targetTr = GameObject.FindWithTag("PLAYER");
    }

    void OnEnable()
    {
        gameObject.tag = "MONSTER";
        monsterWeapon.tag = "MONSTER_ATTACK";
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
        StartCoroutine(FindClosestMonster());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    protected bool isLook = false;
    protected float lookTime = 0.5f;
    void Update()
    {
        if(_targetTr == null)
        {
            return;
        }

        if (agent.remainingDistance >= 2.0f && !isDie)
        {
            Vector3 direction = agent.desiredVelocity;
            if (direction != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      rot,
                                                      Time.deltaTime * 10.0f);
            }
        }

        if (isLook)
        {
            Quaternion rot = Quaternion.LookRotation(_targetTr.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10.0f);
        }

    }

    IEnumerator FindClosestMonster()//update로 쓸 경우 낭비가 심할 것 같으니 코루틴으로 설정
    {
        while (!isDie)
        {
            if (GameObject.FindGameObjectWithTag("PLAYER") == null)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            FoundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("PLAYER"));
            shortDis = Vector3.Distance(transform.position, FoundObjects[0].transform.position); // 첫번째를 기준으로 잡아주기 

            _targetTr = FoundObjects[0]; // 첫번째를 먼저 

            foreach (GameObject found in FoundObjects)
            {
                float Distance = Vector3.Distance(transform.position, found.transform.position);

                if (_targetTr.GetComponent<Collider>().enabled == false)
                {
                    continue;
                }

                if (Distance < shortDis) // 위에서 잡은 기준으로 거리 재기
                {
                    _targetTr = found;
                    shortDis = Distance;
                }
            }

            yield return new WaitForSeconds(3f);
        }
    }

    protected IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.1f);

            if (_targetTr == null)
            {
                _state = Define.State.Idle;
                continue;
            }

            if (_state == Define.State.Die) yield break;

            distance = Vector3.Distance(_targetTr.transform.position, transform.position);

            _state = ChangeState();
        }
    }

    protected virtual Define.State ChangeState()
    {
        if (hitted == true)
        {
            return Define.State.Hit;
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

    protected IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (_state)
            {
                case Define.State.Idle:
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;

                case Define.State.Moving:
                    agent.SetDestination(_targetTr.transform.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashSkill, false);
                    anim.SetBool(hashAttack, false);
                    break;

                case Define.State.Hit:
                    agent.isStopped = true;
                    yield return new WaitForSeconds(hittedTime);
                    hitted = false;
                    break;

                case Define.State.Attack:
                    isLook = true;
                    agent.isStopped = true;
                    yield return new WaitForSeconds(lookTime);
                    isLook = false;
                    Attack();
                    break;

                case Define.State.Skill:
                    isLook = true;
                    yield return new WaitForSeconds(1f);
                    isLook = false;
                    UseSkill();
                    yield return new WaitForSeconds(skillTime);
                    break;

                case Define.State.Die:

                    MonsterDead();
                    MakeDropItem();

                    yield return new WaitForSeconds(7.0f);

                    _hpBar.SetActive(false);
                    stat.IsDead = false;
                    stat.Hp = stat.MaxHp;
                    stat.Mp = stat.MaxMp;
                    Managers.Game.Despawn(gameObject);

                    GetComponent<CapsuleCollider>().enabled = true;
                    anim.SetTrigger(hashReady);
                    hitted = false;
                    _state = Define.State.Idle;
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    protected virtual void UseSkill()
    {
        return;
    }

    private GameObject _blood;
    protected void MakeBlood(Vector3 pos)
    {
        _blood = Managers.Resource.Instantiate("Particle/BloodExplosion");
        _blood.transform.position = pos;
        Invoke("RemoveBlood", 0.5f);
    }

    protected void RemoveBlood()
    {
        Managers.Resource.Destroy(_blood.gameObject);
    }

    protected virtual void MonsterDead()
    {
        gameObject.tag = "CORPSE";
        agent.isStopped = true;
        isLook = false;
        anim.SetTrigger(hashDie);
        if (monsterSkill != null)
            Managers.Resource.Destroy(monsterSkill.gameObject);
        GetComponent<CapsuleCollider>().enabled = false;
    }

    protected virtual void Attack()
    {
        anim.SetBool(hashSkill, false);
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        anim.SetBool(hashAttack, true);
    }

    private GameObject _dropMoney;
    protected virtual void MakeDropItem()
    {
        _dropMoney = Managers.Resource.Instantiate("Item_Prefab/Crystal");
        _dropMoney.transform.position = this.transform.position + new Vector3(0f, 0.1f, 0f);
        Invoke("RemoveMoney", 20.0f);
    }

    protected void RemoveMoney()
    {
        Managers.Resource.Destroy(_dropMoney.gameObject);
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

            if (coll.transform.parent.GetComponent<SkillObject>() is null)
            {
                Managers.Sound.PlayAtPoint(gameObject, "MonsterSound/swordHitted");

                Transform _player = coll.transform.parent;
                while (_player.GetComponent<PlayerStat>() == null)
                {
                    _player = _player.parent;
                }

                PlayerStat _playerStat = _player.GetComponent<PlayerStat>();
                stat.OnAttacked(_playerStat, _playerStat.Attack);
                StartCoroutine(TimeEffect());
                StartCoroutine(_cam.Shake(0.2f, 0.1f));
            }
            else
            {
                Transform skillObj = coll.transform.parent;
                SkillObject _skill = skillObj.GetComponent<SkillObject>();

                PlayerStat _playerStat = _skill.skillUser;

                stat.OnAttacked(_playerStat, _skill.damage);
            }

            if (stat.IsDead)
                _state = Define.State.Die;

            hitted = true;
        }
    }

    IEnumerator ScreenEffectControl()
    {
        Color color = ScreenEffect.color;
        color.a = 0.3f;
        ScreenEffect.color = color;

        lt.intensity = 2;

        while (color.a > 0)
        {
            if(lt.intensity <= 1)
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

    public virtual void Moaning()
    {
        return;
    }

    protected IEnumerator TimeEffect()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1f;
    }
}
