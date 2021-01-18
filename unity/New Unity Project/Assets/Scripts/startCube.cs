using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startCube : MonoBehaviour
{

    [SerializeField]
    [Tooltip("The start button for the game")]
    private GameObject m_start;

    public GameObject child;

    void Awake()
    {
        // Inable the button (collider)
        m_start.AddComponent<BoxCollider>();
        m_start.GetComponent<BoxCollider>().isTrigger = true;
        m_start.GetComponent<BoxCollider>().size = new Vector3(10, 1, 0);
    }
}
