using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public enum _Color { Pink, Yellow };
    public _Color color = _Color.Pink;

  public GameObject doorOpenerCube;
  public bool doorIsThere = true;
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
      if (other.tag == ("DoorBall") && ((int) other.GetComponent<DoorCubeProperties>().color) == ((int) color))
        {
            doorIsThere = false;
        }
    
    }

    void OnTriggerExit(Collider other)
    {
      if (other.tag == ("DoorBall") && (int)other.GetComponent<DoorCubeProperties>().color == (int)color)
        {
            doorIsThere = true;
        }
    }
}
