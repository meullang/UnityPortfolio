using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : UI_Scene
{
    enum GameObjects
    {
        StartButton,
        LoadButton,
        ExitButton,
    }
    private Button _start;
    private Button _load;
    private Button _exit;

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        //¹öÆ°
        _start = Get<GameObject>((int)GameObjects.StartButton).GetComponent<Button>();
        _start.onClick.AddListener(StartGame);
        _load = Get<GameObject>((int)GameObjects.LoadButton).GetComponent<Button>();
        _load.onClick.AddListener(LoadGame);
        _exit = Get<GameObject>((int)GameObjects.ExitButton).GetComponent<Button>();
        _exit.onClick.AddListener(ExitGame);
    }

    private void StartGame()
    {
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

    private void LoadGame()
    {
        Managers.SaveLoad.Load = true;
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
