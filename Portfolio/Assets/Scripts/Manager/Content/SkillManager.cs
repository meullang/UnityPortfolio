using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    /*
    public List<SkillInfo> SkillInfos = new List<SkillInfo>();

    public Dictionary<int, SkillInfo> MakeDict()
    {
        Dictionary<int, SkillInfo> dict = new Dictionary<int, SkillInfo>();
        foreach (SkillInfo skill in SkillInfos)
            dict.Add(SkillInfos.code, SkillInfos); //레벨을 키로 받아서 나머지 값을 저장함
        return dict;
    }
    */

    public SkillInfo[] playerSkillSlots = new SkillInfo[3];
    public SkillInfo[] bossSkillSlots = new SkillInfo[2];
    public float[] playerLeftCoolingTime = new float[3];
    public float[] bossLeftCoolingTime = new float[2];
    public int[] playerSkillLevel = new int[3];

    public void init()
    {
        playerSkillSlots[0] = Managers.Resource.Load<SkillInfo>("Data/PlayerSkill/Explosion");
        playerSkillSlots[1] = Managers.Resource.Load<SkillInfo>("Data/PlayerSkill/BladeStorm");
        playerSkillSlots[2] = Managers.Resource.Load<SkillInfo>("Data/PlayerSkill/Heal");

        bossSkillSlots[0] = Managers.Resource.Load<SkillInfo>("Data/BossSkill/EarthQuake");
        bossSkillSlots[1] = Managers.Resource.Load<SkillInfo>("Data/BossSkill/ShokeWave");
        
        for (int i = 0; i < playerLeftCoolingTime.Length; ++i)
        {
            playerLeftCoolingTime[i] = 0;
        }

        for (int i = 0; i < bossLeftCoolingTime.Length; ++i)
        {
            bossLeftCoolingTime[i] = 0;
        }

        for(int i = 0; i < playerSkillLevel.Length; ++i)
        {
            playerSkillLevel[i] = 1;
        }
    }

    public IEnumerator cooling()
    {
        while(true)
        {
            for (int i = 0; i < playerSkillSlots.Length; ++i) 
            {
                playerLeftCoolingTime[i] = Mathf.Clamp(playerLeftCoolingTime[i] -= 0.1f, 0, (playerSkillSlots[i].coolingTime - playerSkillSlots[i].coolingTimeDecrease * (playerSkillLevel[i]-1)));
            }

            for (int i = 0; i < bossSkillSlots.Length; ++i)
            {
                bossLeftCoolingTime[i] = Mathf.Clamp(bossLeftCoolingTime[i] -= 0.1f, 0, bossSkillSlots[i].coolingTime);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public GameObject PlayerUseSkill(int n, float _mana)
    {
        if(playerLeftCoolingTime[n] != 0 || _mana < playerSkillSlots[n].ManaCost)
        {
            return null;
        }
        else
        {
            playerLeftCoolingTime[n] = playerSkillSlots[n].coolingTime - (playerSkillSlots[n].coolingTimeDecrease * (playerSkillLevel[n] - 1));
            return playerSkillSlots[n].skillPrefab;
        }
    }

    public GameObject BossUseSkill(int n)
    {
        bossLeftCoolingTime[n] = bossSkillSlots[n].coolingTime;
        return bossSkillSlots[n].skillPrefab;
    }

    public float leftCoolTingTime(int n)
    {
        return playerLeftCoolingTime[n] / (playerSkillSlots[n].coolingTime - playerSkillSlots[n].coolingTimeDecrease * (playerSkillLevel[n] - 1));
    }

    public void SkillCountUP(int n)
    {
        playerSkillLevel[n]++;
    }

    public void Clear()
    {
        for (int i = 0; i < playerLeftCoolingTime.Length; ++i)
        {
            playerLeftCoolingTime[i] = 0;
        }

        for (int i = 0; i < bossLeftCoolingTime.Length; ++i)
        {
            bossLeftCoolingTime[i] = 0;
        }

        for (int i = 0; i < playerSkillLevel.Length; ++i)
        {
            playerSkillLevel[i] = 1;
        }
    }
}
