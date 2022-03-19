using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationManager
{
    public UI_SlotToolTip _toolTip;
    public GamePlayUI _game;

    public void SetNotification(string message)
    {
        _game.SetNewNotification(message);
    }
}
