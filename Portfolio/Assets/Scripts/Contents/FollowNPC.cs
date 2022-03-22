using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowNPC : MonoBehaviour
{
    private Stat _stat;
    private Transform _targetTr;
    private NavMeshAgent _agent;
    private Animator _anim;

    //애니메이션 해시
    private readonly int hashStart = Animator.StringToHash("Start");
    private readonly int hashFollow = Animator.StringToHash("Follow");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashDie = Animator.StringToHash("Die");

    [SerializeField]
    private Define.State _state = Define.State.Idle;

    [SerializeField]
    private float _distance;
    private const float _stopDist = 3.0f;

    private bool _isDie = false;

    private bool _isHitted = false;
    private float _hittedTime = 1.5f;

    GameObject _hpBar;

    void Awake()
    {
        Init();

        //캐싱
        _stat = GetComponent<Stat>();
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.destination = _targetTr.position;
        _agent.isStopped = true;
    }

    private void Start()
    {
        _agent.isStopped = true;

        _stat.MaxHp = 100;
        _stat.Hp = 100;
    }

    public void Init()
    {
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform);

        if (gameObject.GetComponentInChildren<UI_HPBar>() == true)
        {
            _hpBar = transform.Find("UI_HPBar").gameObject;
            _hpBar.SetActive(false);
        }

        _targetTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
}

    

    public void StartQuest()
    {
        _anim.SetTrigger(hashStart);

        this.gameObject.tag = "PLAYER";  //플레이어로 변경하여 공격 대상이 되도록 함

        StartCoroutine(CheckNPCState());
        StartCoroutine(NPCAction());
    }

    public void FailQuest() // 캐릭터가 죽을 경우 캐릭을 멈추고 퀘스트를 처음으로 초기화
    {
        StopAllCoroutines();
        Managers.Quest.questActionIndex = 0;
    }

    void Update()
    {
        if (_agent.remainingDistance >= 3.0f)
        {
            Vector3 direction = _agent.desiredVelocity;
            if (direction != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      rot,
                                                      Time.deltaTime * 10.0f);
            }
        }
    }

    protected IEnumerator CheckNPCState()
    {
        while (!_isDie)
        {
            yield return new WaitForSeconds(0.1f);

            if (_state == Define.State.Die) yield break;

            _distance = Vector3.Distance(_targetTr.position, transform.position);

            _state = ChangeState();
        }
    }

    protected Define.State ChangeState()
    {
        if (_isHitted == true)
        {
            return Define.State.Hit;
        }
        else if (_distance <= _stopDist)
        {
            return Define.State.Idle;
        }
        else
        {
            return Define.State.Moving;
        }
    }

    protected IEnumerator NPCAction()
    {
        while (!_isDie)
        {
            switch (_state)
            {
                case Define.State.Idle:
                    _agent.isStopped = true;
                    _anim.SetBool(hashFollow, false);
                    break;

                case Define.State.Moving:
                    _agent.SetDestination(_targetTr.position);
                    _agent.isStopped = false;
                    _anim.SetBool(hashFollow, true);
                    break;

                case Define.State.Hit:
                    _anim.SetTrigger(hashHit);
                    _agent.isStopped = true;
                    yield return new WaitForSeconds(_hittedTime);
                    _isHitted = false;
                    break;

                case Define.State.Die:
                    _agent.isStopped = true;
                    _anim.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    gameObject.tag = "CORPSE";
                    Destroy(gameObject, 5f);
                    FailQuest();
                    break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }



    void OnTriggerEnter(Collider coll)
    {
        if (coll.CompareTag("MONSTER_ATTACK") || coll.CompareTag("MONSTER_KNOCKBACK"))
        {
            _hpBar.SetActive(true);

            if (coll.transform.parent.GetComponent<SkillObject>() is null)
            {
                Transform _monster = coll.transform.parent;
                while (_monster.GetComponent<MonsterStat>() is null)
                {
                    _monster = _monster.parent;
                }

                MonsterStat _monsterStat = _monster.GetComponent<MonsterStat>();
                _stat.OnAttacked(_monsterStat, _monsterStat.Attack);
            }
            else
            {
                SkillObject _skill = coll.transform.parent.GetComponent<SkillObject>();

                Stat _monsterStat = _skill.skillUser;
                _stat.OnAttacked(_monsterStat, _skill.damage);
            }

            if (_stat.Hp <= 0)
            {
                _stat.Hp = 0;
                _state = Define.State.Die;
            }

            _hpBar.SetActive(true);

            _isHitted = true;
        }

        if (coll.CompareTag("Destination"))
        {
            Managers.Quest.isDoneCheck = true;
        }
    }

    private void OnTriggerExit(Collider coll)
    {
        if (coll.CompareTag("Destination"))
        {
            Managers.Quest.isDoneCheck = false;
        }
    }
}
