using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "New Skill/skill")]
public class SkillInfo : ScriptableObject
{
    public int skillCode;
    public string skillName;
    public string skillDesc;
    public int ManaCost;
    public int damage;
    public float coolingTime;

    public int damageIncreaseRate;
    public int coolingTimeDecrease;

    public Sprite skillImage;
    public GameObject skillPrefab;
    public AudioClip skillSound;
}
