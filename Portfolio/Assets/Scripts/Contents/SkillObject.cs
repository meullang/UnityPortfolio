using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
    public SkillInfo skillInfo;

    public int damage = 0;
    public PlayerStat skillUser;

    private void OnEnable()
    {
        if (skillInfo.skillCode >= 0)
            damage = skillInfo.damage + skillInfo.damageIncreaseRate * Managers.Skill.playerSkillLevel[skillInfo.skillCode];
        else
            damage = skillInfo.damage;

        Managers.Sound.PlayAtPoint(gameObject, skillInfo.skillSound);
    }
}
