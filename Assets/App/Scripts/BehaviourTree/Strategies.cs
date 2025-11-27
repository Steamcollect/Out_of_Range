using UnityEngine;
using UnityEngine.AI;

public interface IStrategy
{
    Node.Status Process();
    void Reset();
}

public class PatrolStrategy : IStrategy
{
    readonly Rigidbody rb;
    readonly NavMeshAgent agent;
    readonly Transform[] patrolPoints;

    readonly float moveSpeed;

    int currentIndex;

    public PatrolStrategy(Rigidbody rb, NavMeshAgent agent, Transform[] patrolPoints, float moveSpeed, int currentIndex)
    {
        this.rb = rb;
        this.agent = agent;
        this.patrolPoints = patrolPoints;
        this.moveSpeed = moveSpeed;
        this.currentIndex = currentIndex;

        currentIndex = 0;
    }

    public Node.Status Process()
    {
        return Node.Status.Running;
    }

    public void Reset()
    {
        currentIndex = 0;
    }
}