using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.position += Vector3.left * 0.5f;
        } else if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.position += Vector3.right * 0.5f;

        }
    }
}
