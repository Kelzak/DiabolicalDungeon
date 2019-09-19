using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoorBehavior : MonoBehaviour
{
    public bool doorIsThere = true;
    public GameObject[] requiredEnemies;

    // Update is called once per frame
    void Update()
    {
        if(doorIsThere == true)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }

        doorIsThere = false;
        foreach(GameObject enemy in requiredEnemies)
        {
            if(enemy != null)
            {
                doorIsThere = true;
            }
        }
    }
}
