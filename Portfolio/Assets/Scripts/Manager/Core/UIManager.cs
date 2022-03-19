using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    int _order = 10;

    Stack<UI_Popup> _PopupStack = new Stack<UI_Popup>();
    public UI_Scene _sceneUI = null;
    
    public GameObject Root // UI_Root를 가져오려할 때 없을 경우 생성 있으면 반환한다.
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
                root = new GameObject { name = "@UI_Root" };
            return root;
        }
    }

    public int GetStackSize()
    {
        return _PopupStack.Count;
    }

    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;

        if (sort)
        {
            canvas.sortingOrder = (_order);
            _order++;
        }
        else
        {
            canvas.sortingOrder = 0;
        }
    }

    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null || parent != null) 
            go.transform.SetParent(parent);
        else
            go.transform.SetParent(Root.transform);

        return Util.GetOrAddComponent<T>(go);
    }

    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        Canvas canvas = go.GetOrAddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        return Util.GetOrAddComponent<T>(go);
    }

    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform);

        return sceneUI;
    }

    public void MakePopupUI<T>(string name = null) where T : UI_Popup
    {
        GameObject go;

        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        Util.GetOrAddComponent<T>(go);

        go.transform.SetParent(Root.transform);

        go.SetActive(false);
    }

    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        GameObject go;
        T popup;

        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        if (Root.transform.Find($"{name}") != null) //기존 UI가 존재하고 있는 것을 활성화시킴
        {
            go = Root.transform.Find($"{name}").gameObject;
            popup = go.GetComponent<T>();

            if (go.activeSelf)
            {
                return popup;
            }
            else
            {
                _PopupStack.Push(popup);

                go.SetActive(true);
            }

            return popup;
        }

        Debug.Log($"can't find {name}");

        return null;
    }

    public void ClosePopupUI(UI_Popup popup)
    {
        if (_PopupStack.Count == 0)
            return;

        if(_PopupStack.Peek() != popup)
        {
            Debug.Log("Close Popup Failed!");
            return;
        }

        ClosePopupUI();
    }

    public void ClosePopupUI()
    {
        if (_PopupStack.Count == 0)
            return;

        UI_Popup popup = _PopupStack.Pop();
        popup.gameObject.SetActive(false);

        popup = null;

        _order--;
    }

    public void CloseAllPopuiUI()
    {
        while (_PopupStack.Count > 0)
            ClosePopupUI();
    }

    public void Clear()
    {
        CloseAllPopuiUI();
        _sceneUI = null;
    }
}
