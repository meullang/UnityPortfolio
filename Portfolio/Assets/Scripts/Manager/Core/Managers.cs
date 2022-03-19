using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//최상위 매니저가 다른 매니저들을 가지고 있고 각 매니저를 거쳐갈 수 있게 함

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

    #region Contents
    GameManagerEx _game = new GameManagerEx();
    SkillManager _skill = new SkillManager();
    TalkManager _talk = new TalkManager();
    QuestManager _quest = new QuestManager();
    NotificationManager _notify = new NotificationManager();
    ItemDataBase _database = new ItemDataBase();

    public static GameManagerEx Game { get { return Instance._game; } }
    public static SkillManager Skill { get { return Instance._skill; } }
    public static TalkManager Talk { get { return Instance._talk; } }
    public static QuestManager Quest { get { return Instance._quest; } }
    public static NotificationManager Notify { get { return Instance._notify; } }
    public static ItemDataBase Database { get { return Instance._database; } }
    #endregion

    #region Core
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();
    SaveLoadManager _saveload = new SaveLoadManager();

    public static DataManager Data { get { return Instance._data; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }
    public static SaveLoadManager SaveLoad { get { return Instance._saveload; } }
    #endregion
    void Start()
    {
        Init();
    }

    static void Init()
    {
        if (s_instance == null) //게임 메니저에 인스턴스가 들어와 있지 않거나 게임 메니저가 없을 경우 생성 후 코드 할당
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._sound.Init();

            s_instance._database.Init();
            s_instance._talk.Init();
            s_instance._quest.Init();
            s_instance._skill.init();
        }
    }

    public static void Clear()
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}
