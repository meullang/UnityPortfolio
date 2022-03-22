using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthQuakeController : MonoBehaviour
{
    [SerializeField]
    private float raiseSpeed = 0.03f;

    private void OnEnable()
    {
        transform.localScale = new Vector3(1, 1, 1);
        //StartCoroutine(DisableCollider());
    }

    private void Update()
    {
        if(transform.localScale.z < 5)
        {
            transform.localScale += new Vector3(0, 0, raiseSpeed);
        }
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(1.8f);
    }
}
