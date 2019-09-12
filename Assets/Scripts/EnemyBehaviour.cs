using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Behaviour halo;

    private void Start()
    {
        halo = (Behaviour)GetComponent("Halo");
    }
}
