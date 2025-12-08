using UnityEngine;

public class EntityTrigger : MonoBehaviour
{
    //[Header("Settings")]
    //[Header("References")]
    private EntityController m_Controller;

    //[Header("Input")]
    //[Header("Output")]

    public void SetController(EntityController controller)
    {
        this.m_Controller = controller;
    }

    public EntityController GetController()
    {
        return m_Controller;
    }
}