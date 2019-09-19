using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{

  public GameObject doorOpenerCube;
  public static bool doorIsThere = true;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(doorIsThere == true)
        {
          doorOpenerCube.SetActive (true);
        }
        else
        {
          doorOpenerCube.SetActive (false);
        }
    }
    void OnTriggerEnter(Collider other)
    {
      if (other.tag == ("DoorBall"))
        {
            doorIsThere = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
      if (other.tag == ("DoorBall"))
        {
            doorIsThere = true;
        }
    }
}
