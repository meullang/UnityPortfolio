using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DragSlot : UI_Base
{
    static public UI_DragSlot instance;

    enum GameObjects
    {
        DragSlot,
    }

    public UI_Slot dragSlot;

    public Transform moveItem;
    private Image imageItem;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        imageItem = Get<GameObject>((int)GameObjects.DragSlot).GetComponent<Image>();
        instance = this;

        moveItem = Get<GameObject>((int)GameObjects.DragSlot).GetComponent<Transform>();
    }

    public void DragSetImage(Image _itemImage)
    {
        imageItem.sprite = _itemImage.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = imageItem.color;
        color.a = _alpha;
        imageItem.color = color;
    }
}
