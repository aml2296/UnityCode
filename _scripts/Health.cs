using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public event Action death;

    [SerializeField]
    private float maxHP = 10;
    [SerializeField]
    private float currentHP;


    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
    }
    private void Update()
    {
        if (currentHP <= 0)
            death?.Invoke();
    }
    public void Damage(float dmg)
    {
        currentHP -= dmg;
    }
}
