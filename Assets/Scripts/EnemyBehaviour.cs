using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    private Behaviour halo;
    private NavMeshAgent agent;
    private LayerMask layerToIgnore;

    private Transform player;

    private enum State { Idle, Attacking };
    private State currentState;

    private void Start()
    {
        halo = (Behaviour)GetComponent("Halo");
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        layerToIgnore = ~LayerMask.GetMask("Ignore Raycast");
        StartCoroutine(Idle());
    }

    private void LateUpdate()
    {
        if (currentState == State.Attacking)
        {
            var lookPos = new Vector3(player.position.x, this.transform.position.y, player.position.z);
            transform.LookAt(lookPos);
        }
    }

    public void MakeTarget(bool on)
    {
        halo.enabled = on;
    }

    IEnumerator Idle()
    {

        Vector3 playerPos;
        agent.SetDestination(transform.position);

        while (currentState == State.Idle)
        {
            playerPos = player.position;
            
            RaycastHit hit;
            Physics.Raycast(transform.position, (playerPos - transform.position).normalized, out hit, Mathf.Infinity, layerToIgnore);
            if (hit.collider != null && hit.collider.tag == "Player")
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
            playerPos = player.position;
            agent.SetDestination(playerPos);
            Physics.Raycast(transform.position, (playerPos - transform.position).normalized, out hit, Mathf.Infinity, layerToIgnore);
            if (hit.collider != null && hit.collider.tag != "Player")
            {
                currentState = State.Idle;
            }
            yield return null;
        }
        StartCoroutine(Idle());
    }
}
