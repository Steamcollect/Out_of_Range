using UnityEngine;
using System.Collections.Generic;

public class BehaviourTree : Node
{
    public BehaviourTree(string name) : base(name) { }

    public override Status Process()
    {
        while (currentChild < childrens.Count)
        {
            Status status = childrens[currentChild].Process();
            if(status != Status.Success)
                return status;

            currentChild++;
        }

        return Status.Success;
    }
}

public class Leaf : Node
{
    readonly IStrategy strategy;

    public Leaf(string name, IStrategy strategy) : base(name)
    {
        this.strategy = strategy;
    }

    public override Status Process() => strategy.Process();
    public override void Reset() => strategy.Reset();
}

public class Node
{
    public enum Status{ Success, Failure, Running}

    public readonly string name;

    public readonly List<Node> childrens = new();
    protected int currentChild;

    public Node(string name = "Node")
    {
        this.name = name;
    }

    public void AddChild(Node child) => childrens.Add(child);

    public virtual Status Process() => childrens[currentChild].Process();
    public virtual void Reset()
    {
        currentChild = 0;
        foreach (Node child in childrens)
        {
            child.Reset();
        }
    }
}
