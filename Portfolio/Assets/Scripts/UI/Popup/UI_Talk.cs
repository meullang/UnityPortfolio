using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Talk : UI_Popup
{
    enum GameObjects
    {
        Talk,
        IsQuest,
        Accept,
        Decline,
        TalkText,
        NameText,
    }

    GameObject _isQuest;
    Button _Accept;
    Button _Decline;
    Text _TalkText;
    Text _NameText;
    ActionController _action;

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));
        _action = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<ActionController>();
        _action._talk = this;

        _isQuest = Get<GameObject>((int)GameObjects.IsQuest);
        _Accept = Get<GameObject>((int)GameObjects.Accept).GetComponent<Button>();
        _Decline = Get<GameObject>((int)GameObjects.Decline).GetComponent<Button>();
        _TalkText = Get<GameObject>((int)GameObjects.TalkText).GetComponent<Text>();
        _NameText = Get<GameObject>((int)GameObjects.NameText).GetComponent<Text>();

        _isQuest.SetActive(false);
        _Accept.gameObject.SetActive(false);
        _Decline.gameObject.SetActive(false);
    }

    public void GetText(string talkData)
    {
        _TalkText.text = talkData;
    }

    public void GetName(string _name)
    {
        _NameText.text = _name;
    }
}
