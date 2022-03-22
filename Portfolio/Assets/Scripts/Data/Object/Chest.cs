using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator _anim;
    [SerializeField]
    private Item _item;

    private bool _isOpen = false;

    private void Start()
    {
        _anim = gameObject.GetComponent<Animator>();
    }

    public void OnActive()
    {
        if (_isOpen == false)
        {
            _anim.SetTrigger("IsOpen");
            StartCoroutine(GetItem());

            Managers.Sound.Play("OutWorld/Boxopen");
            _isOpen = true;
        }
    }

    private IEnumerator GetItem()
    {
        yield return new WaitForSeconds(1f);
            Managers.Database.GetItem(_item); 
    }
}
