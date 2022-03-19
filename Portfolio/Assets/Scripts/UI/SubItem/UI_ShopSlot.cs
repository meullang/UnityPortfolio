using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ShopSlot : UI_Base//IPointerEnterHandler, IPointerExitHandler
{
    enum GameObjects
    {
        Icon,
    }

    public UI_Inven _inven;

    public Item item;
    public UI_SlotToolTip theSlotToolTip;

    public Image item_Icon;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));

        item_Icon = Get<GameObject>((int)GameObjects.Icon).GetComponent<Image>();

        BindEvent(this.gameObject, (PointerEventData data) => { PointerClick(data); }, Define.UIEvent.Click);
        BindEvent(this.gameObject, (PointerEventData data) => { PointerEnter(data); }, Define.UIEvent.OnEnter);
        BindEvent(this.gameObject, (PointerEventData data) => { PointerExit(data); }, Define.UIEvent.OnExit);

        ClearSlot();
    }

    private void Start()
    {
        _inven = GameObject.Find("@UI_Root").transform.Find("UI_Inven").GetComponent<UI_Inven>();
        theSlotToolTip = GameObject.Find("@UI_Root").transform.Find("UI_SlotToolTip").gameObject.GetComponent<UI_SlotToolTip>();
    }

    public void AddItem(Item _item)
    {
        item = _item;
        item_Icon.sprite = item.itemImage;

        SetColor(1);
    }

    private void SetColor(float _alpha)
    {
        Color color = item_Icon.color;
        color.a = _alpha;
        item_Icon.color = color;
    }

    private void ClearSlot()
    {
        item = null;
        item_Icon.sprite = null;
        SetColor(0);
    }

    public void PointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null)
            {
                if (_inven.Money < item.buyCost)
                {
                    Managers.Notify.SetNotification("돈이 부족합니다.");
                    Debug.Log("돈이 부족합니다.");
                }
                else
                {
                    _inven.AcquireItem(item);
                    _inven.Money -= item.buyCost;
                }
            }
        }
    }

    public void PointerEnter(PointerEventData eventData)
    {
        if (item != null)
            theSlotToolTip.ShowToolTip(item, transform.position, true);
    }

    public void PointerExit(PointerEventData eventData)
    {
        theSlotToolTip.HideToolTip();
    }
}
