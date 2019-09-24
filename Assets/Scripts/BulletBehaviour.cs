using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    private bool starting = true;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall" && !starting)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" && !starting)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall" && starting)
        {
            starting = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Wall" && starting)
        {
            starting = false;
        }
    }
}
