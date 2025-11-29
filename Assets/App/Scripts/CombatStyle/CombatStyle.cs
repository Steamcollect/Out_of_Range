using System;
using UnityEngine;

public class CombatStyle : MonoBehaviour
{
    public Action<int /*current*/, int /*max*/> OnAMMOChange;
    public Action OnReload;

    public virtual void Attack() { }
    public virtual void Reload() { }
}