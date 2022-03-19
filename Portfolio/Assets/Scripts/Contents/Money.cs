using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public int thisMoney;

    void OnEnable()
    {
        int random = Random.Range(50, 100);
        thisMoney = random;
    }
}
