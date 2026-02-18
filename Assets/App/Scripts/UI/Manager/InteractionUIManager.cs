using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractionUIManager : RegularSingleton<InteractionUIManager>
{

    [FormerlySerializedAs("startingPointerCount")]
    [Header("Settings")]
    [SerializeField] private int m_StartingPointerCount = 3;

    [FormerlySerializedAs("pointerUIPrefab")]
    [Header("References")]
    [SerializeField] private UI_Pointer m_PointerUIPrefab;

    [FormerlySerializedAs("content")] [SerializeField] private Transform m_Content;

    private readonly Queue<UI_Pointer> m_Pointers = new();

    private void Start()
    {
        for (int i = 0; i < m_StartingPointerCount; i++)
        {
            UI_Pointer pointer = CreatePointerUI();
            pointer.gameObject.SetActive(false);
            m_Pointers.Enqueue(pointer);
        }
    }

    public UI_Pointer GetPointer()
    {
        if (m_Pointers.Count <= 0) return CreatePointerUI();

        UI_Pointer pointer = m_Pointers.Dequeue();
        pointer.gameObject.SetActive(true);
        return pointer;
    }

    public void ReturnPointer(UI_Pointer pointer)
    {
        pointer.gameObject.SetActive(false);
        m_Pointers.Enqueue(pointer);
    }

    public UI_Pointer CreatePointerUI()
    {
        return Instantiate(m_PointerUIPrefab, m_Content);
    }
}