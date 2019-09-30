using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingDoor : MonoBehaviour
{
    public enum Direction { Front, Behind, Left, Right };
    public enum Type { PlayerTrigger, DoorOpen  }
    public Direction triggerDirection = Direction.Front;
    public Type doorType;
    public GameObject otherDoor;
    public float triggerDistance;

    private Transform playerPos;
    private Vector3 triggerDirectionVector;
    private Vector3 goalPos;
    private LayerMask playerLayer;

    // Start is called before the first frame update
    void Start()
    {
        goalPos = transform.position;
        if (doorType == Type.PlayerTrigger)
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform;
            playerLayer = LayerMask.GetMask("Player");

            switch (triggerDirection)
            {
                case Direction.Front:
                    triggerDirectionVector = transform.forward;
                    break;
                case Direction.Behind:
                    triggerDirectionVector = -transform.forward;
                    break;
                case Direction.Left:
                    triggerDirectionVector = -transform.right;
                    break;
                case Direction.Right:
                    triggerDirectionVector = transform.right;
                    break;
                default:
                    triggerDirectionVector = transform.forward;
                    break;
            }
        }

        Vector3 startPos = transform.position;
        startPos.y -= GetComponent<Renderer>().bounds.size.y;

        transform.position = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        //BOXCAST
        
        if(doorType == Type.PlayerTrigger && Physics.BoxCast(goalPos, transform.localScale / 2, triggerDirectionVector, transform.rotation, triggerDistance, playerLayer))
        {
            transform.position = goalPos;
        }
        else if(doorType == Type.DoorOpen && otherDoor.activeSelf == false)
        {
            transform.position = goalPos;
        }
    }
}
