using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
//RequireComponent(typeof(AudioSource))]

public class PlayerBehaviour : MonoBehaviour
{
    public float moveSpeed;
    public float swapRange = 8f;
    public float swapCooldown = 5f;
    public float playerLives = 3;
    public Vector3 skipTopuzzle;
    public Image[] Hearts;

    public Canvas cooldownCanvas;
    public Slider cooldownSlider;
    public GameObject range;
    private RespawnController respawnController;
    private GameObject swapTarget;
    private bool canSwap = true;
    private bool targetInRange = false;
    private float currentSwapCooldown;
    private List<KeyValuePair<GameObject, float>> enemyList = null;

    private Rigidbody rb;
    private Camera cam;
    private AudioSource auso;

    public AudioClip teleport;
    public AudioClip playerGetsHit;
    public AudioClip playerDeath;
    public AudioClip doorOpen;

    private GameObject[] enemies;
    private GameObject[] doorBalls;

    public float dashSpeedMultiplier;
    public int dashTime;
    public int dashCoolDown;

    private int dashCoolingDown;
    private int dashTimer;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;

        respawnController = GameObject.FindGameObjectWithTag("RespawnController").GetComponent<RespawnController>();

        if(!respawnController.positionSet)
        {
            respawnController.respawnPosition = transform.position;
            respawnController.positionSet = true;
        }

        transform.position = respawnController.respawnPosition;

        cooldownSlider.maxValue = swapCooldown;

        auso = GetComponent<AudioSource>();

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        doorBalls = GameObject.FindGameObjectsWithTag("DoorBall");

        dashTimer = 0;
        dashCoolingDown = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if (playerLives == 4)
        {
            Hearts[4].GetComponent<Animator>().SetBool("damaged", true);
            print("Health = 4");
        }
        else if (playerLives == 3)
        {
            Hearts[3].GetComponent<Animator>().SetBool("damaged", true);
            print("Health = 3");
        }
        else if (playerLives == 2)
        {
            Hearts[2].GetComponent<Animator>().SetBool("damaged", true);
            print("Health = 2");
        }
        else if (playerLives == 1)
        {
            Hearts[1].GetComponent<Animator>().SetBool("damaged", true);
            print("Health = 1");
        }
        else if (playerLives == 0)
        {
            Hearts[0].GetComponent<Animator>().SetBool("damaged", true);
            print("Health = 0");
        }


        Respawn();

        //Movement
        var yValue = Input.GetAxis("Vertical");
        var xValue = Input.GetAxis("Horizontal");

        var cam = Camera.main;

        var forward = cam.transform.forward;
        var right = cam.transform.right;
        var up = cam.transform.up;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        var moveDirection = forward * yValue + right * xValue;

        RaycastHit sweepHit;
        if (!rb.SweepTest(moveDirection, out sweepHit, moveSpeed * (1 + dashTimer * dashSpeedMultiplier) * Time.deltaTime))
        {
            transform.Translate(moveDirection * moveSpeed * (1 + dashTimer * dashSpeedMultiplier) * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        if ((int)(moveDirection.magnitude * 100) != 0)
        {
            transform.rotation = Quaternion.Euler(0, 360 - (Mathf.Rad2Deg * Mathf.Atan2(moveDirection.z, moveDirection.x)), 0);
        }

        if(dashTimer > 0)
        {
            dashTimer--;
            if(dashTimer == 0)
            {
                dashCoolingDown = dashCoolDown;
            }
        }

        if(dashCoolingDown > 0)
        {
            dashCoolingDown--;
        }

        //Cooldown Canvas Rotation
        cooldownCanvas.transform.LookAt(Camera.main.transform);
        cooldownCanvas.transform.Rotate(0, 180f, 0);

        range.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        float rangeVO = range.transform.position.y - transform.position.y;
        rangeVO = Mathf.Abs(rangeVO);
        float rangeW = Mathf.Sqrt((4 * swapRange * swapRange) - (rangeVO * rangeVO));
        range.transform.localScale = new Vector3(rangeW, 0.01f, rangeW);

        //Clicking to Select Teleport Target
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, LayerMask.GetMask("Targetable")) && (hit.collider.tag == "Enemy" || hit.collider.tag == "DoorBall"))
            {

                if (swapTarget != null)
                {
                    swapTarget.GetComponent<EnemyBehaviour>().MakeTarget(0);
                }
                hit.collider.gameObject.GetComponent<EnemyBehaviour>().MakeTarget(1);
                swapTarget = hit.collider.gameObject;
            }
            else
            {
                if (swapTarget != null)
                {
                    swapTarget.GetComponent<EnemyBehaviour>().MakeTarget(0);
                }
                swapTarget = null;
                targetInRange = false;
            }
        }
        //Selection Handling
        if(swapTarget != null && Vector3.Distance(swapTarget.transform.position, transform.position) > swapRange)
        {
            swapTarget.GetComponent<EnemyBehaviour>().MakeTarget(2);
            targetInRange = false;
        }
        else if (swapTarget != null && Vector3.Distance(swapTarget.transform.position, transform.position) <= swapRange)
        {
            if (swapTarget.GetComponent<EnemyBehaviour>().isFlashing() == false)
            {
                swapTarget.GetComponent<EnemyBehaviour>().MakeTarget(1);
            }
            targetInRange = true;
        }

        //Swap select code
        if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.E))
        {
            List<KeyValuePair<GameObject, float>> tempList = GetEnemiesInRange();

            //If enemyList doesn't exist yet or enemyList is outdated from new list, assign it to the list from most recent check
            if(enemyList == null || !EnemyListEqual(enemyList, tempList))
            {
                enemyList = tempList;
            }
            //enemyList is up to date and exists
            if(enemyList.Count != 0)
            {
                int currIndex = 0;
                //If current target is already in list continue from there
                foreach(KeyValuePair<GameObject, float> x in enemyList)
                {
                    if (swapTarget == x.Key)
                    {
                        currIndex = enemyList.IndexOf(x);
                    }
                }


                //If there is another entry
                if (Input.GetKeyDown(KeyCode.Q) && currIndex + 1 < enemyList.Count)
                {
                    currIndex++;
                }
                else if(Input.GetKeyDown(KeyCode.Q) && currIndex + 1 >= enemyList.Count)
                {
                    currIndex = 0;
                }
                else if(Input.GetKeyDown(KeyCode.E) && currIndex - 1 >= 0)
                {
                    currIndex--;
                }
                //No other entry/end of list
                else
                {
                    currIndex = enemyList.Count - 1;
                }

                if (swapTarget != null && swapTarget != enemyList[currIndex].Key)
                {
                    swapTarget.GetComponent<EnemyBehaviour>().MakeTarget(0);
                }

                swapTarget = enemyList[currIndex].Key;

            }
        }

        //Teleporting
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) && canSwap && swapTarget != null && targetInRange)
        {
            SwapTeleport(swapTarget.transform);
        }

        //Label objects in range
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && Vector3.Distance(enemy.transform.position, transform.position) > swapRange)
            {
                enemy.GetComponent<EnemyBehaviour>().SetInRange(false);
            }
            else if(enemy != null)
            {
                enemy.GetComponent<EnemyBehaviour>().SetInRange(true);
            }
        }

        foreach (GameObject doorBall in doorBalls)
        {
            if (doorBall != null && Vector3.Distance(doorBall.transform.position, transform.position) > swapRange)
            {
                doorBall.GetComponent<EnemyBehaviour>().SetInRange(false);
            }
            else if(doorBall != null)
            {
                doorBall.GetComponent<EnemyBehaviour>().SetInRange(true);
            }
        }

        if((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && dashTimer == 0 && dashCoolingDown == 0)
        {
            if (Mathf.Abs(rb.velocity.y) < 0.01)
            {
                dashTimer = dashTime;
            }
        }
    }



    void SwapTeleport(Transform target)
    {
        target.GetComponent<NavMeshAgent>().enabled = false;

        Vector3 tempPos = transform.position;
        transform.position = target.position;
        target.position = tempPos;
        auso.PlayOneShot(teleport, 0.3f);

        RaycastHit info;
        if (!Physics.Linecast(target.position, target.position + (Vector3.down * 5), out info, LayerMask.GetMask("Default"), QueryTriggerInteraction.Collide) || info.collider.tag != "LavaPit")
        {
            target.GetComponent<NavMeshAgent>().enabled = true;
        }

        if (!cooldownActive)
        {
            StartCoroutine(SwapTeleportCooldown());
        }
}

    private bool cooldownActive = false;
    IEnumerator SwapTeleportCooldown()
    {
        cooldownActive = true;
        canSwap = false;
        currentSwapCooldown = 0;
        cooldownSlider.gameObject.SetActive(true);
        while (currentSwapCooldown < swapCooldown)
        {
            cooldownSlider.value = currentSwapCooldown;
            currentSwapCooldown += Time.deltaTime;
            yield return null;
        }
        cooldownSlider.gameObject.SetActive(false);
        canSwap = true;
        cooldownActive = false;
    }

    void Respawn()
    {
        if (playerLives <= 0)
        {
            playerLives = 5;
            SceneManager.LoadScene(0);
            transform.position = respawnController.respawnPosition;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            playerLives = 5;
            SceneManager.LoadScene(0);
            transform.position = respawnController.respawnPosition;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            transform.position = skipTopuzzle;
            playerLives = 5;
        }

    }

    //Cycle Select Functions
    private List<KeyValuePair<GameObject, float>> GetEnemiesInRange()
    {
        EnemyBehaviour[] enemyComponents = FindObjectsOfType<EnemyBehaviour>();
        List<KeyValuePair<GameObject, float>> eList = new List<KeyValuePair<GameObject, float>>();

        foreach (EnemyBehaviour x in enemyComponents)
        {
            float dist = Vector3.Distance(x.transform.position, transform.position);

            if (x.gameObject.GetComponent<Renderer>().IsVisibleFrom(Camera.main))
            {
                eList.Add(new KeyValuePair<GameObject, float>(x.gameObject, dist));
            }
        }

        eList.Sort((enemy1, enemy2) => enemy2.Value.CompareTo(enemy1.Value));

        return eList;
    }

    private bool EnemyListEqual(List<KeyValuePair<GameObject, float>> l1, List<KeyValuePair<GameObject, float>> l2)
    {
        //Lists are the same size
        if(l1.Count != l2.Count)
        {
            return false;
        }

        //Check individuals
        for(int i = 0; i < l1.Count; i++)
        {
            if(l1[i].Key != l2[i].Key)
            {
                return false;
            }
        }
        //Passed all checks
        return true;
    }



    //ON TRIGGER ENTER / ON COLLISION ENTER
    void OnTriggerEnter(Collider other)
    {
      if (other.tag == ("LavaPit"))
        {
            Debug.Log("Lava has been hit");
          playerLives = 0;
          auso.PlayOneShot(playerGetsHit, 0.3f);
            Debug.Log("Lava damaged the player");
        }

        if (other.tag == ("Pitfall"))
          {
              Debug.Log("Pitfall has been hit");
            playerLives = 0;
              Debug.Log("Pitfall damaged the player");
              auso.PlayOneShot(playerGetsHit, 0.3f);
            }

        if (other.tag == ("WallShooterBullet"))
          {
            playerLives -= 1;
            Destroy(other.gameObject);
            Debug.Log("Bullet damaged the player");
            auso.PlayOneShot(playerGetsHit, 0.3f);
          }

          if (other.tag == ("EnemyBullet"))
            {
              playerLives -= 1;
              Destroy(other.gameObject);
              auso.PlayOneShot(playerGetsHit, 0.3f);
              Debug.Log("Bullet damaged the player");
            }
        if (other.tag == ("DeathPlane"))
        {
            playerLives = 0;
        }

        if (other.tag == ("PlayerWin"))
        {
            playerLives = 0;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Enemy")
        {
            playerLives -= 1;
            Debug.Log("Player walked into enemy");
        }

        if(collision.collider.tag == "Respawn")
        {
            respawnController.respawnPosition = new Vector3(collision.transform.position.x, respawnController.respawnPosition.y, collision.transform.position.z);
        }
    }
}
