using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
public float rotSpeed1 = 0f;
public float rotSpeed2 = 0f;
public float rotSpeed3 = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(rotSpeed1, rotSpeed2, rotSpeed3)*Time.deltaTime); 
    }
}
