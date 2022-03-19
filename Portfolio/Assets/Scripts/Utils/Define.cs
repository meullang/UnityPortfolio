using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum WorldObject
    {
        Unknown = 0,
        Player = 1,
        Skeleton = 2,
        Orc = 3,
        Insect = 4,
        Boss = 5,
    }

    public enum State
    {
        Die,
        Moving,
        Idle,
        Attack,
        Skill,
        Hit,
    }

    public enum Layer
    {
        Monster = 6,
        Block = 7,
        Floor = 8,
    }

    public enum Scene
    {
        Unknown,
        Title,
        Infinite,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        OnEnter,
        OnExit,
        Drag,
        BeginDrag,
        EndDrag,
        Drop,
    }

    public enum CameraMode
    {
        BackView,
        QuestView,
        MovingView,
    }

    public  enum SlotType
    {
        EveryThing,
        OnlyEquipment,
        OnlyWeapon,
        OnlyArmor,
        OnlyAccessory,
        OnlyUsed,
    }

    public enum BossState
    {
        Sleep,
        Die,
        Moving,
        Idle,
        Attack_1,
        Attack_2,
        Skill_1,
        Skill_2,
        Hit,
        LookTarget,
    }
}
