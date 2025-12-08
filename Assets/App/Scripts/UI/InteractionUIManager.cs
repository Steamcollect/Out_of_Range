using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractionUIManager : MonoBehaviour
{
    public static InteractionUIManager S_Instance;

    [FormerlySerializedAs("startingPointerCount")]
    [Header("Settings")]
    [SerializeField] private int m_StartingPointerCount = 3;

    [FormerlySerializedAs("pointerUIPrefab")]
    [Header("References")]
    [SerializeField] private PointerUI m_PointerUIPrefab;

    [FormerlySerializedAs("content")] [SerializeField] private Transform m_Content;

    private readonly Queue<PointerUI> m_Pointers = new();

    private void Awake()
    {
        S_Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < m_StartingPointerCount; i++)
        {
            PointerUI pointer = CreatePointerUI();
            pointer.gameObject.SetActive(false);
            m_Pointers.Enqueue(pointer);
        }
    }

    public PointerUI GetPointer()
    {
        if (m_Pointers.Count <= 0) return CreatePointerUI();

        PointerUI pointer = m_Pointers.Dequeue();
        pointer.gameObject.SetActive(true);
        return pointer;
    }

    public void ReturnPointer(PointerUI pointer)
    {
        pointer.gameObject.SetActive(false);
        m_Pointers.Enqueue(pointer);
    }

    public PointerUI CreatePointerUI()
    {
        return Instantiate(m_PointerUIPrefab, m_Content);
    }
}