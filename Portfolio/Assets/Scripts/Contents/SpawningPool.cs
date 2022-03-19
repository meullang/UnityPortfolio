using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawningPool : MonoBehaviour
{
    [SerializeField]
    int _monsterCount = 0;
    [SerializeField]
    int _reserveCount = 0;

    [SerializeField]
    int _keepMonsterCount = 0;

    [SerializeField]
    public Transform _spawnPos;
    [SerializeField]
    float _nonSpawnRadius = 3.0f;
    [SerializeField]
    float _spawnRadius = 30.0f;
    [SerializeField]
    float _minSpawnTime = 5.0f;
    [SerializeField]
    float _maxSpawnTime = 10.0f;

    public string SetMonster { get; set; }
    public void AddMonsterCount(int value) { _monsterCount += value; }
    public void SetKeepMonsterCount(int count) { _keepMonsterCount = count; }

    void Start()
    {
        switch (SetMonster)
        {
            case "Skeleton":
                Managers.Game.SkeletonSpawnEvent -= AddMonsterCount;
                Managers.Game.SkeletonSpawnEvent += AddMonsterCount;
                break;
            case "Orc":
                Managers.Game.OrcSpawnEvent -= AddMonsterCount;
                Managers.Game.OrcSpawnEvent += AddMonsterCount;
                break;
            case "Insect":
                Managers.Game.InsectSpawnEvent -= AddMonsterCount;
                Managers.Game.InsectSpawnEvent += AddMonsterCount;
                break;
        }
    }

    void Update()
    {
        while (_reserveCount + _monsterCount < _keepMonsterCount)
        {
            StartCoroutine("ReserveSpawn", SetMonster);
        }
    }

    IEnumerator ReserveSpawn(string _monster)
    {
        _reserveCount++;
        yield return new WaitForSeconds(Random.Range(_minSpawnTime, _maxSpawnTime));

        GameObject obj = null;
        NavMeshAgent nma = null;

        switch (_monster)
        {
            case "Skeleton":
                obj = Managers.Game.Spawn(Define.WorldObject.Skeleton, "Skeleton", this.transform);
                nma = obj.GetOrAddComponent<NavMeshAgent>();
                break;
            case "Orc":
                obj = Managers.Game.Spawn(Define.WorldObject.Orc, "Orc", this.transform);
                nma = obj.GetOrAddComponent<NavMeshAgent>();
                break;
            case "Insect":
                obj = Managers.Game.Spawn(Define.WorldObject.Insect, "Insect", this.transform);
                nma = obj.GetOrAddComponent<NavMeshAgent>();
                break;
        }

        if(obj == null || nma == null)
        {
            Debug.Log("can not Seek Monster!!");
        }

        Vector3 randPos;
        Vector3 spawnPoint;

        while (true)
        {
            Vector3 randDir = Random.insideUnitCircle * Random.Range(_nonSpawnRadius, _spawnRadius);
            randPos = _spawnPos.position + new Vector3(randDir.x, 0, randDir.y);
            
            RaycastHit hit;
            Debug.DrawRay(randPos, Vector3.down * 100, Color.red, 20.0f);
            if (Physics.Raycast(randPos, Vector3.down, out hit, 100))
            {
                spawnPoint = hit.point;
            }
            else
            {
                continue;
            }

            NavMeshPath path = new NavMeshPath();
            if (nma.CalculatePath(spawnPoint, path))
                break;
        }

        float range = Random.Range(0f, 360f);
        obj.transform.rotation = Quaternion.Euler(0, range, 0);
        obj.transform.localPosition = spawnPoint;

        _reserveCount--;
    }
}
