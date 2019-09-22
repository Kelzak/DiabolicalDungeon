using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraBehaviour : MonoBehaviour
{
    public Vector3 offset;
    public GameObject player;
    [RangeAttribute(0.0f, 1.0f)]
    public float followStrictness;
    public Material wallMaterial;
    public Material transparentWallMaterial;

    private Vector3 target;
    private ArrayList lastTransparentWalls;
    private float yPos;
    private GameObject[] enemies;
    private GameObject[] doorBalls;

    void Start()
    {
        transform.position = player.transform.position + offset;
        yPos = transform.position.y;
        lastTransparentWalls = new ArrayList();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        doorBalls = GameObject.FindGameObjectsWithTag("DoorBall");
    }

    void Update()
    {
        target = player.transform.position + offset;
        target.y = yPos;

        //determines what proportion of the distance to the player to move the camera, depending on deltaTime
        float moveProportion = Mathf.Pow(followStrictness, Time.deltaTime);

        //move the camera towards the player
        transform.position = moveProportion * transform.position + (1.0f - moveProportion) * target;
    }

    void LateUpdate()
    {
        foreach (MeshRenderer lastTransparentWall in lastTransparentWalls)
        {
            lastTransparentWall.material = wallMaterial;
            lastTransparentWall.transform.gameObject.layer = 0;
        }

        lastTransparentWalls.Clear();

        Ray playerView = GetComponent<Camera>().ScreenPointToRay(GetComponent<Camera>().WorldToScreenPoint(player.transform.position));
        RaycastHit wallDetector;
        Physics.Raycast(playerView, out wallDetector, Mathf.Infinity);
        while (wallDetector.transform != null && wallDetector.transform.CompareTag("Wall"))
        {
            MeshRenderer lastTransparentWall = wallDetector.transform.gameObject.GetComponent<MeshRenderer>();

            lastTransparentWall.material = transparentWallMaterial;
            lastTransparentWall.transform.gameObject.layer = 2;
            lastTransparentWalls.Add(lastTransparentWall);

            Physics.Raycast(playerView, out wallDetector, Mathf.Infinity);
        }

        foreach(GameObject enemy in enemies)
        {
            if (enemy != null && enemy.GetComponent<MeshRenderer>().isVisible)
            {
                Ray enemyView = GetComponent<Camera>().ScreenPointToRay(GetComponent<Camera>().WorldToScreenPoint(enemy.transform.position));
                RaycastHit enemyWallDetector;
                Physics.Raycast(enemyView, out enemyWallDetector, Mathf.Infinity);
                while (enemyWallDetector.transform != null && enemyWallDetector.transform.CompareTag("Wall"))
                {
                    MeshRenderer lastTransparentWall = enemyWallDetector.transform.gameObject.GetComponent<MeshRenderer>();

                    lastTransparentWall.material = transparentWallMaterial;
                    lastTransparentWall.transform.gameObject.layer = 2;
                    lastTransparentWalls.Add(lastTransparentWall);

                    Physics.Raycast(enemyView, out enemyWallDetector, Mathf.Infinity);
                }
            }
        }

        foreach (GameObject doorBall in doorBalls)
        {
            if (doorBall != null && doorBall.GetComponent<MeshRenderer>().isVisible)
            {
                Ray doorBView = GetComponent<Camera>().ScreenPointToRay(GetComponent<Camera>().WorldToScreenPoint(doorBall.transform.position));
                RaycastHit doorBWallDetector;
                Physics.Raycast(doorBView, out doorBWallDetector, Mathf.Infinity);
                while (doorBWallDetector.transform != null && doorBWallDetector.transform.CompareTag("Wall"))
                {
                    MeshRenderer lastTransparentWall = doorBWallDetector.transform.gameObject.GetComponent<MeshRenderer>();

                    lastTransparentWall.material = transparentWallMaterial;
                    lastTransparentWall.transform.gameObject.layer = 2;
                    lastTransparentWalls.Add(lastTransparentWall);

                    Physics.Raycast(doorBView, out doorBWallDetector, Mathf.Infinity);
                }
            }
        }
    }
}
