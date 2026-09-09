using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TxtDmgPrefab : NetworkBehaviour
{
    [SerializeField] TMP_Text _txt;

    [Networked] int _damage { get; set; }

    public override void Spawned()
    {
        RefreshText();
    }

    public override void Render()
    {
        RefreshText();
    }

    public void SetDamage(int damage)
    {
        if (!HasStateAuthority)
            return;

        _damage = damage;
    }

    void RefreshText()
    {
        if (_txt == null)
            return;

        _txt.text = _damage.ToString();
    }
}
