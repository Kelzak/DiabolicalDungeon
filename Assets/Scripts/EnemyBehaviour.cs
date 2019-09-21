using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public GameObject projectile;
    public float shootSpeed;
    public float followDistance = 3;
    public float sightRange = 20f;
    public enum PathingType { Follow, Path}
    public PathingType pathType;
    public Vector3[] pathMovements;
    public float moveSpeed = 2.5f;
    public bool canShoot = true;
    public bool canDie = true;

    private Behaviour halo;
    private Color baseEmissionColor;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private LayerMask layerToIgnore;
    private Material mat;
    private Renderer rend;

    private Transform player;

    private enum State { Idle, Attacking };
    private State currentState;

    private void Start()
    {
        halo = (Behaviour)GetComponent("Halo");
        baseEmissionColor = new Color(0 / 255f, 15 / 255f, 0 / 255f);

        player = GameObject.FindGameObjectWithTag("Player").transform;
        mat = GetComponent<Renderer>().material;
        mat.EnableKeyword("_EMISSION");

        rend = GetComponent<Renderer>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.speed = moveSpeed;

        layerToIgnore = ~LayerMask.GetMask("Targetable");

        if(pathType == PathingType.Path && pathMovements.Length != 0)
        {
            if (!isPathing)
            {
                StartCoroutine(FollowPath());
            }
        }
        else
        {
            pathType = PathingType.Follow;
        }
        StartCoroutine(Idle());
    }

    private void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity);
        if(hit.collider.tag == "LavaPit")
        {
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void LateUpdate()
    {
        if (currentState == State.Attacking)
        {
            var lookPos = new Vector3(player.position.x, this.transform.position.y, player.position.z);
            transform.LookAt(lookPos);
        }
    }

    //public void MakeTarget(bool on)
    //{
    //    halo.enabled = on;
    //    flash = true;
        
    //}

    bool flash = false;
    //Off = 0, Flash = 1, Solid = 2
    public void MakeTarget(int state)
    {
        switch (state)
        {
            //Off
            case 0:
                flash = false;
                mat.SetColor("_EmissionColor", baseEmissionColor);
                break;
            //Flash
            case 1:
                flash = true;
                StartCoroutine(EmissionGlow());
                break;
            //Solid
            case 2:
                flash = false;
                mat.SetColor("_EmissionColor", baseEmissionColor * 8);
                break;
        }
    }

    IEnumerator Idle()
    {

        Vector3 playerPos;
        if (pathType == PathingType.Follow)
        {
            agent.SetDestination(transform.position);
        }

        while (currentState == State.Idle)
        {
            playerPos = player.position;

            RaycastHit hit;
            Physics.Raycast(transform.position, (playerPos - transform.position).normalized, out hit, sightRange, layerToIgnore);
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
            //Shootin
            if (!shooting)
            {
                StartCoroutine(Shoot());
            };

            //Movement
            playerPos = player.position;
            if (pathType == PathingType.Follow)
            {
                agent.SetDestination(playerPos + (transform.position - playerPos).normalized * followDistance);
            }
            Physics.Raycast(transform.position, (playerPos - transform.position).normalized, out hit, sightRange, layerToIgnore);
            if (hit.collider != null && hit.collider.tag != "Player")
            {
                currentState = State.Idle;
            }
            yield return null;
        }
        StartCoroutine(Idle());
    }

    bool shooting = false;
    private IEnumerator Shoot()
    {
        shooting = true;
        yield return new WaitForSecondsRealtime(shootSpeed / 2);
        while (currentState == State.Attacking && canShoot == true && rend.isVisible)
        {
            var spawnPosition = transform.position + (transform.forward * 1.25f);
            var newBullet = Instantiate<GameObject>(projectile, spawnPosition, Quaternion.Euler(0, transform.eulerAngles.y + 90f ,90f));
            newBullet.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 500, 0));
            Destroy(newBullet, 4.0f);
            yield return new WaitForSecondsRealtime(shootSpeed);
        }
        shooting = false;
    }

    bool isPathing = false;
    private IEnumerator FollowPath()
    {
        isPathing = true;
        while (pathType == PathingType.Path)
        {
            foreach (Vector3 point in pathMovements)
            {
                var newDestination = transform.position + point;
                if (agent.enabled == true)
                {
                    agent.destination = newDestination;
                    while (agent.enabled == true && !(agent.remainingDistance != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0))
                    {
                        yield return null;
                    }
                }
                yield return null;
            }
            yield return null;
        }
        isPathing = false;
    }

    public bool isFlashing()
    {
        return flash;
    }

    private IEnumerator EmissionGlow()
    {
        float intensityMin, intensityMax, changeSpeed;
        intensityMin = 4.0f;
        intensityMax = 8.0f;
        changeSpeed = 10;

        float currentIntensity = intensityMin;
        mat.SetColor("_EmissionColor", baseEmissionColor * currentIntensity);

        while (flash)
        {
            while (currentIntensity < intensityMax && flash)
            {
                currentIntensity += Time.deltaTime * changeSpeed;
                mat.SetColor("_EmissionColor", baseEmissionColor * currentIntensity);
                yield return null;
            }
            while (currentIntensity > intensityMin && flash)
            {
                currentIntensity -= Time.deltaTime * changeSpeed;
                mat.SetColor("_EmissionColor", baseEmissionColor * currentIntensity);
                yield return null;
            }

            yield return null;
        }

        mat.SetColor("_EmissionColor", baseEmissionColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "LavaPit" || other.tag == "WallShooterBullet" || other.tag == "Pitfall" || other.tag == "EnemyBullet" && canDie == true)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }

    }


}
