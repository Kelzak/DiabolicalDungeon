using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float moveSpeed;
    private float playerLives = 3;
    private vector3 respawnPosition;

    private Rigidbody rb;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
        respawnPosition = transform.position
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

        var moveDirection = forward * yValue + right * xValue;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }

    void respawn()
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
          playerLives -= 1;
        }
    }
}
