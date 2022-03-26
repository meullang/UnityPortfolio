using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCData : MonoBehaviour
{
    public string NPCname;
    public int npcID;
    public int num;
    public GameObject Mark;

    public UI_Inven _Inven;
    public UI_Shop _Shop;

    private void Awake()
    {
        Managers.Quest.NPCs[num] = this;
        Mark = transform.Find("Mark").gameObject;
        QuestMarkDisappear();
    }

    private void Start()
    {
        _Inven = Managers.Quest._inven;
        _Shop = Managers.Quest._shop;
    }

    public void OnActive()
    {
        if (npcID == 6000)
        {
            gameObject.GetComponent<FollowNPC>().StartQuest();
        }
        else if (npcID == 4000)
        {
            Managers.UI.ShowPopupUI<UI_Inven>();
            Managers.UI.ShowPopupUI<UI_Shop>();
            _Inven.moveArea.transform.position = _Shop.moveArea.transform.position + new Vector3(400, 0, 0);
        }
        else if (npcID == 5000)
        {
            Stat _playerStat = Managers.Game.GetPlayer().GetComponent<Stat>();
            _playerStat.Hp = _playerStat.MaxHp;
            _playerStat.Mp = _playerStat.MaxMp;

            GameObject Heal = Managers.Resource.Instantiate("Particle/PlayerSkill/Healing", Managers.Game.GetPlayer().transform);
            Heal.transform.position = Managers.Game.GetPlayer().transform.position;
            Destroy(Heal, 2f);
        }
        else
        {
            return;
        }
    }

    public void QuestMarkAppear()
    {
        Mark.gameObject.SetActive(true);
    }

    public void QuestMarkDisappear()
    {
        Mark.gameObject.SetActive(false);
    }
}
