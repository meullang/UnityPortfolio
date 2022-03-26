using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Setting : UI_Popup
{
    enum GameObjects
    {
        Setting,
        SettingText,
        SoundSlider,
        BGMSlider,
        CameraSlider,
        MouseSlider,
        SaveButton,
        ExitButton,
        TitleButton,
    }
    public CameraController _camera;
    public AudioMixer mixer;
    public PlayerController _player;

    private Slider effectSound;
    private Slider backgoundSound;
    private Slider cameraDistance;
    private Slider mouseSensitivity;

    private Button _save;
    private Button _title;
    private Button _exit;

    public override void Init()
    {
        base.Init();
        _camera = Camera.main.GetComponent<CameraController>();
        _player = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<PlayerController>();

        Bind<GameObject>(typeof(GameObjects));

        //슬라이더
        effectSound = Get<GameObject>((int)GameObjects.SoundSlider).GetComponent<Slider>();
        effectSound.onValueChanged.AddListener(SetEffectSount);
        backgoundSound = Get<GameObject>((int)GameObjects.BGMSlider).GetComponent<Slider>();
        backgoundSound.onValueChanged.AddListener(SetBackgoundSound);
        mouseSensitivity = Get<GameObject>((int)GameObjects.MouseSlider).GetComponent<Slider>();
        mouseSensitivity.onValueChanged.AddListener(SetMouseSensitivity);
        cameraDistance = Get<GameObject>((int)GameObjects.CameraSlider).GetComponent<Slider>();
        cameraDistance.onValueChanged.AddListener(SetCameraDistance);

        //창 이동
        GameObject movePoint = Get<GameObject>((int)GameObjects.SettingText);
        GameObject moveArea = Get<GameObject>((int)GameObjects.Setting);
        BindEvent(movePoint, (PointerEventData data) => { moveArea.transform.position = data.position; }, Define.UIEvent.Drag);

        //버튼
        _save = Get<GameObject>((int)GameObjects.SaveButton).GetComponent<Button>();
        _save.onClick.AddListener(SaveGame);
        _title = Get<GameObject>((int)GameObjects.TitleButton).GetComponent<Button>();
        _title.onClick.AddListener(GOTitle);
        _exit = Get<GameObject>((int)GameObjects.ExitButton).GetComponent<Button>();
        _exit.onClick.AddListener(ExitGame);

        mixer = Managers.Sound.GetAudioMixer();

        ResetSlider();
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    private void SetEffectSount(float _value)
    {
        mixer.SetFloat("EffectVol", _value);
    }

    private void SetBackgoundSound(float _value)
    {
        mixer.SetFloat("BgmVol", _value);
    }

    private void SetMouseSensitivity(float _value)
    {
        _camera.MouseSensitivity = _value;
    }

    private void SetCameraDistance(float _value)
    {
        _camera.DistanceFromTarget = _value;
    }

    private void SaveGame()
    {
        if(_player.shortDis < 20)
        {
            Managers.Notify.SetNotification("몬스터가 가까이에 있을 때는 저장할 수 없습니다.");
        }
        else
        {
            Managers.SaveLoad.SaveData();
            Managers.Notify.SetNotification("저장되었습니다.");
        }
    }

    private void GOTitle()
    {
        Managers.Scene.LoadScene(Define.Scene.Title);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void ResetSlider()
    {
        float _value;

        effectSound.maxValue = 20.0f;
        effectSound.minValue = -80.0f;
        mixer.GetFloat("EffectVol", out _value);
        effectSound.value = _value;

        backgoundSound.maxValue = 20.0f;
        backgoundSound.minValue = -80.0f;
        mixer.GetFloat("BgmVol", out _value);
        backgoundSound.value = _value;

        mouseSensitivity.maxValue = 3.0f;
        mouseSensitivity.minValue = 1.0f;
        mouseSensitivity.value = _camera.MouseSensitivity;

        cameraDistance.maxValue = 30.0f;
        cameraDistance.minValue = 15.0f;
        cameraDistance.value = _camera.DistanceFromTarget;
    }
}
