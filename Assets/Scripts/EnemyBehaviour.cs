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
    public enum PathingType { Follow, Path ,Static}
    public PathingType pathType;
    public Vector3[] pathMovements;
    public float moveSpeed = 2.5f;
    public bool canShoot = true;
    public bool canDie = true;
    public ParticleSystem ps;

    private Behaviour halo;
    private List<Color> baseEmissionColors;
    private NavMeshAgent agent;
    private NavMeshPath path;
    private LayerMask layerToIgnore;
    private List<Material> matList;
    private Renderer rend;

    private Transform player;

    private enum State { Idle, Attacking };
    private State currentState;
    private Vector3 respawnPos;

    private bool inRange;

    private void Start()
    {
        respawnPos = transform.position;
        halo = (Behaviour)GetComponent("Halo");
       

        player = GameObject.FindGameObjectWithTag("Player").transform;

        matList = new List<Material>();
        baseEmissionColors = new List<Color>();

        Renderer targetRenderer;
        if(transform.name.Contains("barrel"))
        {
            targetRenderer = transform.GetChild(0).GetComponent<Renderer>();
        }
        else
        {
            targetRenderer = GetComponent<Renderer>();
        }

        foreach(Material x in targetRenderer.materials)
        {
            x.EnableKeyword("_EMISSION");
            matList.Add(x);
            baseEmissionColors.Add(x.GetColor("_EmissionColor"));
        }

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
        if (currentState == State.Attacking && tag != "DoorBall")
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
                UpdateEmissionColor();
                break;
            //Flash
            case 1:
                flash = true;
                StartCoroutine(EmissionGlow());
                break;
            //Solid
            case 2:
                flash = false;
                UpdateEmissionColor(8);
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
            if (pathType == PathingType.Follow && agent.enabled)
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
            newBullet.AddComponent<BulletBehaviour>();
            newBullet.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 500, 0));
            Destroy(newBullet, 3.0f);
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
        UpdateEmissionColor(currentIntensity);

        while (flash)
        {
            while (currentIntensity < intensityMax && flash)
            {
                currentIntensity += Time.deltaTime * changeSpeed;
                UpdateEmissionColor(currentIntensity);
                yield return null;
            }
            while (currentIntensity > intensityMin && flash)
            {
                currentIntensity -= Time.deltaTime * changeSpeed;
                UpdateEmissionColor(currentIntensity);
                yield return null;
            }

            yield return null;
        }

        UpdateEmissionColor();
    }

    private void UpdateEmissionColor(float multiplier = 1)
    {
        for (int i = 0; i < matList.Count; i++)
        {
                matList[i].SetColor("_EmissionColor", baseEmissionColors[i] * multiplier);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "DeathPlane" || other.tag == "LavaPit" || other.tag == "WallShooterBullet" || other.tag == "Pitfall" || other.tag == "EnemyBullet" && canDie == true)
        {
            if(tag == "DoorBall")
            {
                if(other.tag == "LavaPit" || other.tag == "Pitfall" || other.tag == "DeathPlane")
                {
                    transform.position = respawnPos;
                    GetComponent<NavMeshAgent>().enabled = true;
                }
            }
            else
            {
                StopAllCoroutines();
                ParticleSystem particle = Instantiate(ps, transform.position, Quaternion.identity);
                ParticleSystem.MainModule particlemain = particle.main;
                particlemain.startColor = baseEmissionColors[0];
                Destroy(particle.gameObject, 1);
                gameObject.SetActive(false);
                Destroy(gameObject,1.1f);
            }
        }

    }

    public void SetInRange(bool inInRange)
    {
        inRange = inInRange;
    }

    public bool GetInRange()
    {
        return inRange;
    }
}
