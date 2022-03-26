using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerEx
{
    //생존 확인
    public bool playerAlive;
    public bool bossAlive;

    //매니저를 이용한 빠른 접근
    GameObject _player;
    GameObject _boss;

    HashSet<GameObject> _skeleton = new HashSet<GameObject>();
    HashSet<GameObject> _orc = new HashSet<GameObject>();
    HashSet<GameObject> _insect = new HashSet<GameObject>();

    public Action<int> SkeletonSpawnEvent;
    public Action<int> OrcSpawnEvent;
    public Action<int> InsectSpawnEvent;

    public GameObject GetPlayer() { return _player; }
    public GameObject GetBoss() { return _boss; }

    public GameObject Spawn(Define.WorldObject type, string path, Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        switch(type)
        {
            case Define.WorldObject.Player:
                _player = go;
                break;
            case Define.WorldObject.Skeleton:
                _skeleton.Add(go);
                if (SkeletonSpawnEvent != null)
                    SkeletonSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Orc:
                _orc.Add(go);
                if (OrcSpawnEvent != null)
                    OrcSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Insect:
                _insect.Add(go);
                if (InsectSpawnEvent != null)
                    InsectSpawnEvent.Invoke(1);
                break;
            case Define.WorldObject.Boss:
                _boss = go;
                break;
        }

        return go;
    }

    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        MonsterController mc = go.GetComponent<MonsterController>();
        if(mc != null)
            return mc.WorldObjectType;

        PlayerController pc = go.GetComponent<PlayerController>();
        if (pc != null)
            return pc.WorldObjectType;

        Boss bs = go.GetComponent<Boss>();
        if (bs != null)
            return bs.WorldObjectType;

        return Define.WorldObject.Unknown;
    }

    public void Despawn(GameObject go)
    {
        Define.WorldObject type = GetWorldObjectType(go);

        switch(type)
        {
            case Define.WorldObject.Player:
                if (_player == go)
                    _player = null;
                break;
            case Define.WorldObject.Skeleton:
                {
                    if (_skeleton.Contains(go))
                    {
                        _skeleton.Remove(go);
                        if (SkeletonSpawnEvent != null)
                            SkeletonSpawnEvent.Invoke(-1);
                    }
                }
                break;
            case Define.WorldObject.Orc:
                {
                    if (_orc.Contains(go))
                    {
                        _orc.Remove(go);
                        if (OrcSpawnEvent != null)
                            OrcSpawnEvent.Invoke(-1);
                    }
                }
                break;
            case Define.WorldObject.Insect:
                {
                    if (_insect.Contains(go))
                    {
                        _insect.Remove(go);
                        if (InsectSpawnEvent != null)
                            InsectSpawnEvent.Invoke(-1);
                    }
                }
                break;
            case Define.WorldObject.Boss:
                if (_boss == go)
                    _boss = null;
                break;
        }

        Managers.Resource.Destroy(go);
    }
}
