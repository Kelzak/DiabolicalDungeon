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
  public Vector3 bulletDirection ;
  private Vector3 bulletspawn = (transform.position.x, transform.position.y - 3, transform.position.z);
  bool playerIsInSideWallShotRange = false;
    // Start is called before the first frame update
    void Start()
    {

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
        else {
          playerIsInSideWallShotRange = false;
        }
    }

    void FixedUpdate()
    {
      if ( playerIsInSideWallShotRange == true && Time.time >= timeCheck)
        {
         fire();
         timeCheck = Time.time + shotDelay;
        }
    }

      void fire()
      {
          GameObject newBullet = Instantiate(bullet, bulletDirection, transform.rotation);
          newBullet.GetComponent<Rigidbody>().AddRelativeForce(bulletDirection);
          Destroy(newBullet, 4.0f);
      }
      void OnTriggerEnter(Collider other)
      {
        if (other.tag == ("Player"))
        {
        playerIsInSideWallShotRange = true;
          Debug.Log("Player is in shooting range");
        }
      }
      void OnTriggerExit(Collider other)
      {
        if (other.tag == ("Player"))
        {
        playerIsInSideWallShotRange = false;
          Debug.Log("Player has exited shooting range");
        }
      }
}
