using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShooter : MonoBehaviour
{
  public GameObject bullet;
  public float speedX = 0;
  public float speedY = 0;
  public float speedZ = 0;
  float shotDelay = 1f;
  float timeCheck;
  public Vector3 bulletDirection;
  bool playerIsInSideWallShotRange = false;
    // Start is called before the first frame update
    void Start()
    {
        //bulletDirection = (speedX, speedY, speedZ);
    }

    // Update is called once per frame
    void Update()
    {
        var orientation = transform.up;
        if(Physics.BoxCast(transform.position + (5* transform.up), new Vector3(5f, 5f, 5f), -orientation, Quaternion.identity, 100f, LayerMask.GetMask("Player")))
        {
          playerIsInSideWallShotRange = true;
            Debug.Log("hitting player");


        }
    }

    void FixedUpdate()
    {
      //if ( playerIsInSideWallShotRange == true && Time.time >= timeCheck)
      //  {
      //  fire();
      //  timeCheck = Time.time + shotDelay;
      //  }
    }



      void fire()
      {
          GameObject newBullet = Instantiate(bullet, transform.position, transform.rotation);
          newBullet.GetComponent<Rigidbody>().AddRelativeForce(bulletDirection);
          Destroy(newBullet, 4.0f);
      }
}
