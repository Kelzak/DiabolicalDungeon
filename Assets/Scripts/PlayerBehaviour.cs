using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float moveSpeed;
    public float swapRange = 8f;
    public float swapCooldown = 5f;
    public float playerLives = 3;

    private Vector3 respawnPosition;
    private GameObject swapTarget;

    private Rigidbody rb;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        respawnPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var yValue = Input.GetAxis("Vertical");
        var xValue = Input.GetAxis("Horizontal");

        var cam = Camera.main;

        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Respawn();

        var moveDirection = forward * yValue + right * xValue;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        //Clicking to Select Teleport Target
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100) && hit.collider.tag == "Enemy" && Vector3.Distance(hit.collider.transform.position, transform.position) < swapRange)
            {
                if (swapTarget != null)
                {
                    swapTarget.GetComponent<EnemyBehaviour>().MakeTarget(false);
                }
                hit.collider.gameObject.GetComponent<EnemyBehaviour>().MakeTarget(true);
                swapTarget = hit.collider.gameObject;
            }
        }
        //Deselect if out of range
        if(swapTarget != null && Vector3.Distance(swapTarget.transform.position, transform.position) > swapRange)
        {
            swapTarget.GetComponent<EnemyBehaviour>().MakeTarget(false);
            swapTarget = null;
        }

        //Teleporting
        if(Input.GetKeyDown(KeyCode.Q) && swapTarget != null)
        {
            SwapTeleport(swapTarget.transform);
        }

    }

    void SwapTeleport(Transform target)
    {
        Vector3 tempStorage = transform.position;
        transform.position = target.position;
        target.position = tempStorage;
    }

    void Respawn()
    {
      if(playerLives <= 0)
      {
        transform.position = respawnPosition;
        playerLives = 3;
    }
    }

    void OnTriggerEnter(Collider other)
    {
      if (other.tag == ("LavaPit"))
        {
            Debug.Log("Lava has been hit");
          playerLives -= 1;
        }
    }
}
