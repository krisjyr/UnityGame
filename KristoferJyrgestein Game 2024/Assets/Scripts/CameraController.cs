using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public GameObject Wall;

    public GameObject player;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.transform.position + Vector3.up * 3 + Vector3.back * 5;
        transform.LookAt(player.transform);
        offset = transform.position - player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + offset;

        // mouse look around the player radius when right mouse button is pressed (aka change position (not rotation) of the camera around the player)

        if (Input.GetMouseButton(1))
        {
            float horizontal = Input.GetAxis("Mouse X");
            float vertical = Input.GetAxis("Mouse Y");

            transform.RotateAround(player.transform.position, Vector3.up, horizontal * 2);
            transform.RotateAround(player.transform.position, transform.right, -vertical * 2);
        }
    

    }
}
