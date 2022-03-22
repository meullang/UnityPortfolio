using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    [SerializeField]
    private int monsterCount = 5;

    GameObject player;
    GameObject boss;
    GameObject cam;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        GameObject Loading = Managers.Resource.Instantiate($"UI/Scene/LoadingUI");
        Destroy(Loading, 3f);

        Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
       
        Dictionary<int, Data.Stat> monsterDict = Managers.Data.MonsterStatDict;

        player = Managers.Game.Spawn(Define.WorldObject.Player, "player");


        Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player.transform);
        Managers.Game.playerAlive = true;

        StartCoroutine(releaseBlock());

        boss = Managers.Game.Spawn(Define.WorldObject.Boss, "Boss");
        Managers.Game.bossAlive = true;


        MakeSpawningPool("Skeleton", monsterCount);
        MakeSpawningPool("Orc", monsterCount);
        MakeSpawningPool("Insect", monsterCount);

        Managers.Quest._questNote = Managers.UI.ShowSceneUI<GamePlayUI>();
        
        MakePopUpUI();

        if (Managers.SaveLoad.Load == true)
        {
            Managers.SaveLoad.LoadData();
            Managers.SaveLoad.Load = false;
        }
        
        StartCoroutine(Managers.Skill.cooling());

        //Managers.Quest.SetFirstQuest();

        Managers.Sound.Play("Sounds/Bgm/FarmDay", Define.Sound.Bgm);
    }

    private void MakeSpawningPool(string _monster, int _count)
    {
        GameObject _pool = new GameObject { name = $"{_monster}SpawningPool" };
        SpawningPool _spawnPool = _pool.GetOrAddComponent<SpawningPool>();
        _spawnPool.SetMonster = $"{_monster}";
        _spawnPool._spawnPos = GameObject.Find($"{_monster}SpawnPoint").transform;
        _spawnPool.SetKeepMonsterCount(_count);
    }

    private void MakePopUpUI()
    {
        Managers.UI.MakePopupUI<UI_Inven>();
        Managers.UI.MakePopupUI<UI_PlayerInfo>();
        Managers.UI.MakePopupUI<UI_Setting>();
        Managers.UI.MakePopupUI<UI_Shop>();
        Managers.UI.MakeSubItem<UI_DragSlot>();
        Managers.UI.MakeSubItem<UI_SlotToolTip>();
        Managers.UI.MakePopupUI<UI_Talk>();
        
    }

    public override void Clear()
    {
        Managers.UI._sceneUI = null;
        Managers.UI.CloseAllPopuiUI();

        Managers.Quest.Clear();
        Managers.Skill.Clear();
        StopAllCoroutines();
    }

    public IEnumerator releaseBlock()
    {
        player.GetComponent<PlayerController>().isStop = true;
        Camera.main.gameObject.GetComponent<CameraController>().isStop = true;
        yield return new WaitForSeconds(3f);
        player.GetComponent<PlayerController>().isStop = false;
        Camera.main.GetComponent<CameraController>().isStop = false;

        Managers.Quest.SetFirstQuest();
    }
}
