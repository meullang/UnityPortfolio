using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShokeWaveController : MonoBehaviour
{
    [SerializeField]
    private float raiseSpeed = 1f;

    private void OnEnable()
    {
        transform.localScale = new Vector3(1, 1, 1);
        //StartCoroutine(DisableCollider());
    }

    private void Update()
    {
        if (transform.localScale.z < 19)
        {
            transform.localScale += new Vector3(0, 0, raiseSpeed);
        }
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
