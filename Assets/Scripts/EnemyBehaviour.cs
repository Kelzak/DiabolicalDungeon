using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Behaviour halo;
    private enum State { Idle, Attacking };
    private State currentState;

    private void Start()
    {
        halo = (Behaviour)GetComponent("Halo");
        StartCoroutine(Idle());
    }

    public void MakeTarget(bool on)
    {
        halo.enabled = on;
    }

    IEnumerator Idle()
    {

        Vector3 playerPos;

        while (currentState == State.Idle)
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            RaycastHit hit;
            Physics.Raycast(transform.position, (playerPos - transform.position).normalized, out hit, Mathf.Infinity);
            if (hit.collider.tag == "Player")
            {
                currentState = State.Attacking;
            }
            
            yield return null;
        }
        StartCoroutine(Attacking());
    }

    IEnumerator Attacking()
    {

        Vector3 playerPos;
        RaycastHit hit;

        while(currentState == State.Attacking)
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
            Physics.Raycast(transform.position, (playerPos - transform.position).normalized, out hit, Mathf.Infinity);
            if (hit.collider.tag != "Player")
            {
                currentState = State.Idle;
            }
            yield return null;
        }
        StartCoroutine(Idle());
    }
}
