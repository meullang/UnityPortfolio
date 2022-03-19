using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BossHP : UI_Base
{
    enum GameObjects
    {
        HP_Bar
    }

    Stat _stat;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        _stat = transform.parent.GetComponent<Stat>();

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        float ratio = _stat.Hp / (float)_stat.MaxHp;

        SetHpRatio(ratio);
    }

    public void SetHpRatio(float ratio)
    {
        GetObject((int)GameObjects.HP_Bar).GetComponent<Image>().fillAmount = ratio;
    }
}
