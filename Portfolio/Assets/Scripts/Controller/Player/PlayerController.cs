using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //애니메이션과 변수들
    #region Setting
    private Camera cam;
    public Animator anim;
    private CharacterController controller;

    private readonly int hashMove = Animator.StringToHash("Move");
    private readonly int hashRun = Animator.StringToHash("Run");
    private readonly int hashAttack_1 = Animator.StringToHash("Attack_1");
    private readonly int hashAttack_2 = Animator.StringToHash("Attack_2");
    private readonly int hashAttack_3 = Animator.StringToHash("Attack_3");
    private readonly int hashAttack_4 = Animator.StringToHash("Attack_4");
    private readonly int hashBeHitLight = Animator.StringToHash("BeHitLight");
    private readonly int hashRoll = Animator.StringToHash("Roll");
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashSkill = Animator.StringToHash("Skill");
    #endregion
    [SerializeField]
    PlayerStat _stat;
    [SerializeField]
    Define.State _state = Define.State.Idle;
    public Define.WorldObject WorldObjectType { get; protected set; } = Define.WorldObject.Unknown;

    //이동관련

    [SerializeField]
    private bool isRun = false;
    [SerializeField]
    private float _moveSpeed = 5.0f;
    [SerializeField]
    //private float _turnSpeed = 0.1f;

    //상태관련
    public bool isStop = false;

    private bool _isClicked = false;
    private bool _isRoll = false;
    private bool _isDie = false;
    private bool _isHitted = false;

    //사운드 관련
    private bool playWalkSound = false;

    [SerializeField]
    public int PlayerWeaponCode = 0;
    [SerializeField]
    public GameObject PlayerWeapon;
    [SerializeField]
    public GameObject[] PlayerWeaponList;
    [SerializeField]
    public RuntimeAnimatorController[] animControllerList;

    [SerializeField]
    private Image ScreenEffect;

    //중력 관련
    private Vector3 _gravityDir;
    [SerializeField]
    private float _gravity = 20.0f;

    //가장 가까운 몹 찾기
    [SerializeField]
    private List<GameObject> _FoundObjects;
    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    public float shortDis;

    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        _stat = gameObject.GetComponent<PlayerStat>();
        cam = Camera.main;

        Init();

        PlayerWeapon.tag = "PLAYER_ATTACK";
        StartCoroutine(FindClosestMonster());
        StartCoroutine(HearMonsterMoan());
        ScreenEffect = GameObject.Find("Hitted").GetComponent<Image>();
    }

    IEnumerator ScreenEffectControl()
    {
        Color color = ScreenEffect.color;
        color.a = 0.3f;
        ScreenEffect.color = color;
        while (color.a > 0)
        {
            color.a -= 0.005f;
            ScreenEffect.color = color;
            yield return null;
        }
    }

    void Update()
    {
        if (isStop)
        {
            _state = Define.State.Idle;
        }

        switch (_state)
        {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Moving:
                UpdateMoving();
                UpdateRun();
                break;
            case Define.State.Idle:
                UpdateIdle();
                break;
            case Define.State.Attack:
                UpdateAttack();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
        }
    }

    void Init()
    {
        WorldObjectType = Define.WorldObject.Player;
        Managers.Database._playerStat = this.GetComponent<PlayerStat>();
    }

    void UpdateMoving()
    {
        RecoverSP();
        anim.SetBool(hashMove, true);

        //이동에 대한 부분
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(h, 0f, v).normalized;
        Vector3 moveDir;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.rotation.eulerAngles.y;

            if (_enemy != null && Vector3.Distance(transform.position, _enemy.transform.position) < 10f && !isRun)
                transform.LookAt(_enemy.transform);
            else
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            controller.Move(moveDir.normalized * _moveSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.LeftControl) && !isRun)
            {
                if (_stat.Sp >= 10)
                {
                    _stat.Sp -= 10;
                    transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
                    StartCoroutine(UpdateDodgeRoll(moveDir));
                }
                else
                {
                    Managers.Notify.SetNotification("스테미나가 부족합니다.");
                }
            }
        }

        //중력을 받는 부분
        _gravityDir.y -= _gravity * Time.deltaTime;
        controller.Move(_gravityDir * Time.deltaTime);

        if (!playWalkSound)
        {
            playWalkSound = true;
            Managers.Sound.PlayAtPoint(gameObject, "PlayerSound/footsteps", true);
        }

        //입력에 따른 상태 변경
        if (Input.GetMouseButtonDown(0) && !isRun)
        {
            _isClicked = true;
            _state = Define.State.Attack;
            return;
        }

        if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3)) && !isRun)
        {
            _state = Define.State.Skill;
            return;
        }

        if (v == 0f && h == 0f)
        {
            _state = Define.State.Idle;
            return;
        }
    }

    void UpdateRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _stat.Sp > 0.01)
        {
            Managers.Sound.ControlPitch(gameObject, 2.5f);
            isRun = true;
            anim.SetBool(hashRun, true);
            _moveSpeed = 10f;

            _stat.Sp -= 5f * Time.deltaTime;
        }
        else
        {
            Managers.Sound.ControlPitch(gameObject, 1.5f);
            isRun = false;
            anim.SetBool(hashRun, false);
            _moveSpeed = 5f;
        }
    }

    int noOfClicks = 0;
    float lastClickedTime = 0;
    float maxComboDelay = 4f;
    void UpdateAttack()
    {
        if (PlayerWeapon == null)
        {
            Managers.Notify.SetNotification("공격할 무기가 없습니다.");
            _state = Define.State.Idle;
            return;
        }

        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }

        if (Input.GetMouseButtonDown(0) || _isClicked)
        {
            _isClicked = false;
            lastClickedTime = Time.time;
            noOfClicks++;
            if (noOfClicks == 1)
            {
                anim.SetBool(hashAttack_1, true);
            }
            noOfClicks = Mathf.Clamp(noOfClicks, 0, 4);
        }
    }

    void UpdateIdle()
    {

        if (playWalkSound)
        {
            Managers.Sound.StopAtPoint(gameObject);
            Managers.Sound.ControlPitch(gameObject, 1.5f);
        }
        playWalkSound = false;

        RecoverSP();

        if (!_isRoll)
            anim.SetBool(hashMove, false);

        if (Input.GetMouseButtonDown(0))
        {
            _isClicked = true;
            _state = Define.State.Attack;
            return;
        }

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            _state = Define.State.Moving;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            _state = Define.State.Skill;
            return;
        }

        if (_enemy != null && Vector3.Distance(transform.position, _enemy.transform.position) < 10 && !_isRoll)
            transform.LookAt(_enemy.transform);
    }

    void UpdateDie()
    {
        if (!_isDie)
        {
            _isDie = true;
            anim.SetTrigger(hashDie);
            StopAllCoroutines();
            controller.enabled = false;
            Managers.Game.playerAlive = false;
        }

    }

    void OnTriggerEnter(Collider coll)
    {
        if (_isDie)
        {
            return;
        }

        if ((coll.CompareTag("MONSTER_ATTACK") || coll.CompareTag("MONSTER_KNOCKBACK")))
        {
            StartCoroutine(ScreenEffectControl());

            anim.SetTrigger(hashBeHitLight);

            noOfClicks = 0;

            Managers.Sound.PlayAtPoint(gameObject, "PlayerSound/punch");

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

            if (_stat.IsDead)
            {
                _state = Define.State.Die;
                return;
            }

            if (coll.CompareTag("MONSTER_KNOCKBACK"))
            {
                Vector3 dir = coll.transform.position - transform.position;
                StartCoroutine(ControllerForced(dir));
            }
        }
    }

    IEnumerator ControllerForced(Vector3 dir)
    {
        float i = 2;
        isStop = true;
        while (i > 0)
        {
            i -= Time.deltaTime;
            controller.Move(dir.normalized * 3f * Time.deltaTime);

            _gravityDir.y -= _gravity * Time.deltaTime;
            controller.Move(_gravityDir * Time.deltaTime);
            yield return null;
        }
        isStop = false;
    }

    IEnumerator FindClosestMonster()//update로 쓸 경우 낭비가 심할 것 같으니 코루틴으로 설정
    {
        while (!_stat.IsDead)
        {
            if (GameObject.FindGameObjectWithTag("MONSTER") == null)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            _FoundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("MONSTER"));
            shortDis = Vector3.Distance(transform.position, _FoundObjects[0].transform.position); // 첫번째를 기준으로 잡아주기 

            _enemy = _FoundObjects[0]; // 첫번째를 먼저 

            foreach (GameObject found in _FoundObjects)
            {
                float Distance = Vector3.Distance(transform.position, found.transform.position);

                if (Distance < shortDis) // 위에서 잡은 기준으로 거리 재기
                {
                    _enemy = found;
                    shortDis = Distance;
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator HearMonsterMoan() //몬스터가 일정 거리 이상 가까워 지면 가장 가까운 못스터가 소리를 냄
    {
        while (!_stat.IsDead)
        {
            yield return new WaitForSeconds(15f);

            if (_enemy == null || _enemy.GetComponent<MonsterController>() == null)
            {
                continue;
            }
            else
            {
                if (shortDis < 20)
                {
                    _enemy.GetComponent<MonsterController>().Moaning();
                }
            }
        }
    }

    void UpdateSkill()
    {
        StartCoroutine(SkillCorutine());
    }

    IEnumerator UpdateDodgeRoll(Vector3 dir)
    {
        anim.SetTrigger(hashRoll);
        Managers.Sound.PlayAtPoint(gameObject, "PlayerSound/dodge");

        float i = 0.9f;
        isStop = true;
        _isRoll = true;

        PlayerWeapon.SetActive(false);

        while (i > 0)
        {
            if (_isHitted)
            {
                break;
            }

            i -= Time.deltaTime;
            controller.Move(dir.normalized * 10f * Time.deltaTime);

            _gravityDir.y -= _gravity * Time.deltaTime;
            controller.Move(_gravityDir * Time.deltaTime);
            yield return null;
        }
        PlayerWeapon.SetActive(true);
        isStop = false;
        _isRoll = false;
        _isHitted = false;
    }

    Vector3 _skillTarget;
    GameObject _skill;
    IEnumerator SkillCorutine()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            if (shortDis > 15 || _enemy == null)
            {
                Managers.Notify.SetNotification("적이 스킬 사거리 밖에 있습니다.");
            }
            else
            {
                GameObject explosion = Managers.Skill.PlayerUseSkill(0, _stat);
                if (explosion == null || shortDis > 15)
                {
                    Managers.Notify.SetNotification("스킬이 아직 준비되지 않았거나 마나가 부족합니다.");
                    Debug.Log("스킬이 아직 준비되지 않았거나 마나가 부족합니다.");
                }
                else
                {
                    anim.SetTrigger(hashSkill);
                    _skillTarget = _enemy.transform.position + (Vector3.up * 2.0f);
                    _skill = Instantiate(explosion, _skillTarget, Quaternion.identity);
                    Destroy(_skill.gameObject, 1.8f);
                    isStop = true;
                    yield return new WaitForSeconds(1f);
                    isStop = false;
                }
            }
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            GameObject bladeStorm = Managers.Skill.PlayerUseSkill(1, _stat);
            if (bladeStorm == null)
            {
                Managers.Notify.SetNotification("스킬이 아직 준비되지 않았거나 마나가 부족합니다.");
                Debug.Log("스킬이 아직 준비되지 않았거나 마나가 부족합니다.");
            }
            else
            {
                anim.SetTrigger(hashSkill);
                
                _skill = Instantiate(bladeStorm, transform);

                _skillTarget = transform.position + (Vector3.up * 0.5f);
                _skill.transform.position = _skillTarget;

                Destroy(_skill.gameObject, 2.5f);
                isStop = true;
                yield return new WaitForSeconds(1f);
                isStop = false;
            }
        }
        else if (Input.GetKey(KeyCode.Alpha3))
        {
            GameObject heal = Managers.Skill.PlayerUseSkill(2, _stat);
            if (heal == null)
            {
                Managers.Notify.SetNotification("스킬이 아직 준비되지 않았거나 마나가 부족합니다.");
                Debug.Log("스킬이 아직 준비되지 않았거나 마나가 부족합니다.");
            }
            else
            {
                anim.SetTrigger(hashSkill);
                
                _skill = Instantiate(heal, transform);

                _skillTarget = transform.position + (Vector3.up * 0.5f);
                _skill.transform.position = _skillTarget;

                int healAmount = _skill.GetComponent<SkillObject>().damage + _stat.Hp;

                _stat.Hp = Mathf.Clamp(healAmount, 0, _stat.MaxHp);
                Destroy(_skill.gameObject, 2.0f);
                isStop = true;
                yield return new WaitForSeconds(1f);
                isStop = false;
            }
        }

        _state = Define.State.Idle;
    }

    float recoverTime = 3;
    float leftTime = 3;
    void RecoverSP() // 스테미나 회복에 대한 함수
    {
        if (!isRun && !_isRoll)
        {
            if (leftTime > 0)
                leftTime -= Time.deltaTime;

            if (leftTime <= 0)
            {
                leftTime = 0;
                _stat.Sp = Mathf.Clamp(_stat.Sp + 10f * Time.deltaTime, 0f, _stat.MaxSp);
            }
        }
        else
        {
            leftTime = recoverTime;
        }
    }

    //
    //애니메이션 컨트롤러에 들어가는 함수
    //

    public void P_EnableWeapon()
    {
        Managers.Sound.PlayAtPoint(gameObject, "PlayerSound/SWORD");
        PlayerWeapon.GetComponent<Collider>().enabled = true;
        PlayerWeapon.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void P_DisableWeapon()
    {
        PlayerWeapon.GetComponent<Collider>().enabled = false;
        PlayerWeapon.transform.GetChild(0).gameObject.SetActive(false);
    }

    GameObject StepDust;
    void MakeDust()
    {
        StepDust = Managers.Resource.Instantiate("Particle/DustSmoke");
        StepDust.transform.position = transform.position;

        Invoke("RemoveDust", 0.5f);
    }

    void RemoveDust() => Managers.Resource.Destroy(StepDust);

    void return1()
    {
        if (noOfClicks >= 2)
        {
            anim.SetBool(hashAttack_2, true);
        }
        else
        {
            anim.SetBool(hashAttack_1, false);
            noOfClicks = 0;
            _state = Define.State.Idle;
        }
    }

    void return2()
    {
        if (noOfClicks >= 3)
        {
            anim.SetBool(hashAttack_3, true);
        }
        else
        {
            anim.SetBool(hashAttack_1, false);
            anim.SetBool(hashAttack_2, false);
            noOfClicks = 0;
            _state = Define.State.Idle;
        }
    }

    void return3()
    {
        if (noOfClicks >= 4)
        {
            anim.SetBool(hashAttack_4, true);
        }
        else
        {
            anim.SetBool(hashAttack_1, false);
            anim.SetBool(hashAttack_2, false);
            anim.SetBool(hashAttack_3, false);
            noOfClicks = 0;
            _state = Define.State.Idle;
        }
    }

    void return4()
    {
        anim.SetBool(hashAttack_1, false);
        anim.SetBool(hashAttack_2, false);
        anim.SetBool(hashAttack_3, false);
        anim.SetBool(hashAttack_4, false);
        noOfClicks = 0;
        _state = Define.State.Idle;
    }
}
