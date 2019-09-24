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

    public Canvas cooldownCanvas;
    public Slider cooldownSlider;
    public GameObject range;
    private Vector3 respawnPosition;
    private GameObject swapTarget;
    private bool canSwap = true;
    private bool targetInRange = false;
    private float currentSwapCooldown;

    private Rigidbody rb;
    private Camera cam;
    private AudioSource auso;

    public AudioClip teleport;
    public AudioClip playerGetsHit;
    public AudioClip playerDeath;
    public AudioClip doorOpen;

    private GameObject[] enemies;
    private GameObject[] doorBalls;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        respawnPosition = transform.position;

        cooldownSlider.maxValue = swapCooldown;

        auso = GetComponent<AudioSource>();

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        doorBalls = GameObject.FindGameObjectsWithTag("DoorBall");
    }

    // Update is called once per frame
    void Update()
    {
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

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        if ((int)(moveDirection.magnitude * 100) != 0)
        {
            transform.rotation = Quaternion.Euler(0, 360 - (Mathf.Rad2Deg * Mathf.Atan2(moveDirection.z, moveDirection.x)), 0);
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
        //Deselect if out of range
        if (swapTarget != null && Vector3.Distance(swapTarget.transform.position, transform.position) > swapRange)
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

        //Teleporting
        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) && canSwap && swapTarget != null && targetInRange)
        {
            StartCoroutine(SwapTeleport(swapTarget.transform));
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
    }

IEnumerator SwapTeleport(Transform target)
{
        target.GetComponent<NavMeshAgent>().enabled = false;

        Vector3 tempPos = transform.position;
        transform.position = target.position;
        target.position = tempPos;
        auso.PlayOneShot(teleport, 0.3f);

        yield return new WaitForSeconds(0.1f);

        if (target.GetComponent<Rigidbody>().isKinematic == true)
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
      if(playerLives <= 0)
      {
        transform.position = respawnPosition;
        playerLives = 5;
        SceneManager.LoadScene(0);
      }

        if(Input.GetKeyDown(KeyCode.R))
        {
          transform.position = respawnPosition;
          playerLives = 5;
          SceneManager.LoadScene(0);
        }
          if(Input.GetKeyDown(KeyCode.T))
          {
            transform.position = skipTopuzzle;
            playerLives = 5;
            }

    }

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
    void OnCollisionEnter(Collision Collision)
    {
if(Collision.collider.tag == ("Enemy"))
{
  playerLives -= 1;
  Debug.Log("Player walked into enemy");
}
    }
}
