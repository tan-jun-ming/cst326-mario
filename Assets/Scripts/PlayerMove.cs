using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public BlockBreaker blockbreaker;
    public float speed_limit = 10f;

    private Rigidbody player;
    private AudioClip[] walk_sound;

    private int step_cooldown = 0;

    private bool fps_mode = false;

    // Start is called before the first frame update
    void Start()
    {
        player = (Rigidbody)gameObject.GetComponent(typeof(Rigidbody));
        walk_sound = blockbreaker.dig_sound;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = player.velocity;

        float delta_x = Input.GetAxis("Horizontal");
        float delta_y = Input.GetAxis("Vertical");
        
        if (fps_mode)
        {
            float jump = Input.GetAxis("Jump");

            if (player.velocity.y == 0)
            {
                player.velocity += Vector3.up * jump * 5;
            }

            if (step_cooldown > 0)
            {
                step_cooldown--;
            }

            Vector3 translation = new Vector3(delta_x, 0, delta_y);
            if (translation.magnitude >= 1f)
            {
                translation.Normalize();
            }
            translation *= 0.2f;
            Vector3 angle = GameObject.Find("Camera 4").transform.eulerAngles;
            angle.x = 0;
            angle.z = 0;

            translation = Quaternion.Euler(angle) * translation;
            gameObject.transform.position += translation;

            if (translation.magnitude > 0f && step_cooldown == 0)
            {
                ((AudioSource)GameObject.Find("Feet").GetComponent(typeof(AudioSource))).PlayOneShot(walk_sound[Random.Range(0, walk_sound.Length)], 1f);
                step_cooldown = 10;
            }


        } else
        {
            if (delta_x != 0 && velocity.x > -speed_limit && velocity.x < speed_limit)
            {
                velocity.x -= delta_x;
            }
            velocity.y += delta_y;


            player.velocity = velocity;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ((AudioSource)GameObject.Find("Drop Speaker").GetComponent(typeof(AudioSource))).Play();
        }
        

    }

    public void set_fps(bool state)
    {
        fps_mode = state;

        ((Canvas)GameObject.Find("Crosshair Canvas").GetComponent(typeof(Canvas))).enabled = state; // crosshair;
        ((Canvas)GameObject.Find("Text Canvas").GetComponent(typeof(Canvas))).enabled = !state; // crosshair;
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None; // hidden cursor

        ((Renderer)GameObject.Find("Mario").GetComponent(typeof(Renderer))).enabled = !state; // player sprite

        ((MouseLook)GameObject.Find("Camera 4").GetComponent(typeof(MouseLook))).enabled = state; // movement script

        if (!state)
        { // reset rotation
            GameObject.Find("Camera 4").transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public bool get_fps()
    {
        return fps_mode;
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        //print(contact.normal);

        if (contact.normal.y == -1)
        {
            if (contact.otherCollider.CompareTag("block"))
            {
                blockbreaker.break_block(contact.otherCollider.transform);
            }
        }
    }
}
