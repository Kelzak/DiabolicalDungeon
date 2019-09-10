using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public Vector3 offset;
    public GameObject player;
    [RangeAttribute(0.0f, 1.0f)]
    public float followStrictness;

    private Vector3 target;

    void Update()
    {
        target = player.transform.position + offset;

        //determines what proportion of the distance to the player to move the camera, depending on deltaTime
        float moveProportion = Mathf.Pow(followStrictness, Time.deltaTime);

        //move the camera towards the player
        transform.position = moveProportion * transform.position + (1.0f - moveProportion) * target;
    }
}
