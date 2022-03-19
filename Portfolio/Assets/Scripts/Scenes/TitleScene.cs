using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Title;
        Managers.UI.ShowSceneUI<TitleUI>();

        Managers.Sound.Play("Sounds/Bgm/TitleScreen", Define.Sound.Bgm);
    }

    public override void Clear()
    {
        Managers.UI._sceneUI = null;
    }
}
