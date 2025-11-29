using System;
using UnityEngine;

public class CombatStyle : MonoBehaviour
{
    public Action<float /*current*/,float /*max*/> OnAmmoChange;
    public Action OnReload;

    public virtual void Attack() { }
    public virtual void Reload() { }
}