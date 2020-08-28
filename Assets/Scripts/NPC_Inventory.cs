using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Inventory : MonoBehaviour
{
    [SerializeField]
    private float m_MaxCarryWeight = 30;
    private float m_WeightCarrying = 0;

    public bool PickUp()
    {

        return false;
    }

}
