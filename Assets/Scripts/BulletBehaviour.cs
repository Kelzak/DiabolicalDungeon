using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    AudioSource arrow;
    AudioSource Die;
    void Start()
    {
        AudioSource[] src = GetComponents<AudioSource>();
        arrow = src[0];
        Die = src[1];
    }

    void Awake()
    {
        arrow.Play();
    }

    private int startTime = 3;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall" && startTime == 0)
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Enemy" && startTime == 0)
        {
            Die.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" && startTime == 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(startTime > 0)
        {
            startTime = 3;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (startTime > 0)
        {
            startTime = 3;
        }
    }

    private void Update()
    {
        if (startTime > 0)
        {
            startTime--;
        }
    }
}
