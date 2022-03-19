using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleaveController : MonoBehaviour
{
    [SerializeField]
    private float velo = 10f;
    private SphereCollider coll;

    private void Start()
    {
        coll = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        transform.localPosition = Vector3.zero;

        StartCoroutine(DisableCollider());
    }

    private void Update()
    {
        transform.Translate(Vector3.right * Time.deltaTime * velo);
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(1.1f);
    }
}
