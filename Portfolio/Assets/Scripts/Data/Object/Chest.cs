using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator anim;
    public Item _item;

    private bool check = false;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void OnActive()
    {
        if (check == false)
        {
            anim.SetTrigger("IsOpen");
            StartCoroutine(GetItem());
            check = true;
        }
    }

    private IEnumerator GetItem()
    {
        yield return new WaitForSeconds(1f);
            Managers.Database.GetItem(_item); 
    }
}
