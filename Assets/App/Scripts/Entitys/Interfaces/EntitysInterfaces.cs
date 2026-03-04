using UnityEngine;

public interface IMovement
{
    void ResetVelocity();

    void Move(Vector3 input);

    public void SetSpeedMult(float mult);
    public void SetCanMove(bool canMove);
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
    void TakeDamage(float damage);

    void Die();
}

public interface IShield
{
    int TakeDamage(int damage);
    bool IsDestroy();
}

public interface ITargetable
{
    Vector3 GetTargetPosition();
    Vector3 GetTargetIndicatorPosition();
}

public interface ISpawnable
{
    void OnSpawn();
}