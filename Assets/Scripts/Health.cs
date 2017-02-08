﻿using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [SerializeField] private int _max;

    private int _current;

    public bool IsDefeated { get { return _current <= 0; } }

    private void Awake()
    {
        _current = _max;
    }

    public void AffectHealth(int amount)
    {
        _current += amount;
        if (_current < 0)
        {
            _current = 0;
        }
        else if (_current > _max)
        {
            _current = _max;
        }
    }
}
