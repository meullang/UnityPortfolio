using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _mode = Define.CameraMode.BackView;

    [SerializeField]
    private float _mouseSensitivity;
    public float MouseSensitivity { get { return _mouseSensitivity; } set { _mouseSensitivity = value; } }

    private float _rotationY;
    private float _rotationX;

    [SerializeField]
    private Transform _target;
    private Vector3 _fixedTargetTr;

    [SerializeField]
    private float _distanceFromTarget;
    public float DistanceFromTarget { get { return _distanceFromTarget; } set { _distanceFromTarget = value; } }

    private Vector3 _currentRotation;
    private Vector3 _smoothVelocity = Vector3.zero;

    [SerializeField]
    private float _smoothTime = 0.2f;

    [SerializeField]
    private Vector2 _rotationXMinMax = new Vector2(-10, 40);

    public void SetPlayer(Transform player) { _target = player; }

    public bool isStop;

    void Awake()
    {
        isStop = false;
        originPos = transform.parent.transform.localPosition;

        _mouseSensitivity = 1.0f;
        _distanceFromTarget = 15.0f;
    }

    void Update()
    {
        _fixedTargetTr = _target.transform.position + new Vector3(0, 1.5f, 0);

        if (_target == null)
            return;

        if (_mode == Define.CameraMode.BackView && !isStop)
        {
            RaycastHit hit;
            Vector3 _delta = transform.position - _fixedTargetTr;
            if (Physics.Raycast(_fixedTargetTr, _delta.normalized, out hit, _distanceFromTarget, LayerMask.GetMask("BLOCK")))
            {
                float dist = (hit.point - _fixedTargetTr).magnitude * 0.8f;
                LookTarget(dist);

            }
            else
            {
                LookTarget(_distanceFromTarget);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(Shake(3f, 3f));
        }
    }

    public void SetBackView() // 캐릭터를 중심으로 바라봄
    {
        _mode = Define.CameraMode.BackView;
    }

    public void SetQuestView(Transform tr) // NPC의 정면을 바라봐야 됨
    {
        _mode = Define.CameraMode.QuestView;
        transform.LookAt(tr);
        transform.position = tr.position + (tr.forward * 5);
    }

    public void SetMovingView() // 원하는 움직임을 구현 후 백뷰로 전환
    {
        _mode = Define.CameraMode.MovingView;
    }

    public void LookTarget(float distance)
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        _rotationY += mouseX;
        _rotationX -= mouseY;

        _rotationX = Mathf.Clamp(_rotationX, _rotationXMinMax.x, _rotationXMinMax.y);

        Vector3 nextRotation = new Vector3(_rotationX, _rotationY);

        _currentRotation = Vector3.SmoothDamp(_currentRotation, nextRotation, ref _smoothVelocity, _smoothTime);
        transform.localEulerAngles = _currentRotation;

        transform.position = _fixedTargetTr - transform.forward * distance;
    }

    Vector3 originPos;

    public IEnumerator Shake(float amount, float duration)
    {
        float timer = 0;
        while (timer <= duration)
        {
            transform.parent.transform.localPosition = (Vector3)Random.insideUnitCircle * amount + originPos;

            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.parent.transform.localPosition = originPos;
    }
}