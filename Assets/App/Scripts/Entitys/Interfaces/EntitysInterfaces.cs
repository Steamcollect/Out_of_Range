using UnityEngine;

public interface IMovement
{
    void ResetVelocity();

    void Move(Vector3 input);
}

public interface ILookAtTarget
{
    void LookAt(Vector3 target);
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