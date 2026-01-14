using UnityEngine;

public interface IMovement
{
    void ResetVelocity();

    void Move(Vector3 input);
}

public enum LookAtAxis
{
    Horizontal,
    Vertical,
    Both
}
public interface ILookAtTarget
{
    void LookAt(Vector3 target, LookAtAxis lookAtAxis = LookAtAxis.Both);
}

public interface IHealth
{
    void TakeDamage(int damage);
}

public interface IShield
{
    int TakeDamage(int damage);
    bool IsDestroy();
}

public interface ITargetable
{
    Vector3 GetTargetPosition();
}

public interface ISpawnable
{
    void OnSpawn();
}