using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Camera[] cameras;
    private int camera_count = -1;

    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        swap_camera();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 new_camera_pos = gameObject.transform.position;

        new_camera_pos.x = Mathf.Max(-222f, player.position.x);

        gameObject.transform.position = new_camera_pos;


        if (Input.GetKeyDown(KeyCode.Z))
        {
            swap_camera();
        }
    }

    public Camera get_camera()
    {
        return cameras[camera_count];
    }
    void swap_camera()
    {
        camera_count++;
        camera_count = camera_count % cameras.Length;

        for (int i = 0; i < cameras.Length; i++)
        {
            cameras[i].enabled = i == camera_count;
        }

    }
}
