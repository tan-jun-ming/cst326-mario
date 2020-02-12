using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpin : MonoBehaviour
{
    public AudioClip pop_sound;

    private float wave = 0;
    private int bus = 25;
    private bool flying = false;
    private bool dead = false;

    private Transform cube;
    private Rigidbody cube_rb;
    private Transform player_camera;

    // Start is called before the first frame update
    void Start()
    {
        cube = gameObject.transform.GetChild(0);
        cube_rb = ((Rigidbody)gameObject.GetComponent(typeof(Rigidbody)));
        player_camera = GameObject.Find("Camera 4").transform;

        cube.Rotate(Vector3.up * Random.Range(0f, 360f));
        wave = Random.Range(0f, 3.14f);
        do_wave();

        Vector3 rot = Random.rotationUniform * Vector3.up;
        rot.y = Mathf.Abs(rot.y);
        cube_rb.velocity = rot * 2;
    }

    // Update is called once per frame
    void Update()
    {

        if (cube.position.y < -50)
        {
            GameObject.Destroy(gameObject);
        }

        cube.Rotate(Vector3.up * 0.5f);

        if (flying)
        {
            if ((gameObject.transform.position - player_camera.position).magnitude < 0.1f)
            {
                kill_pickup();
            } else
            {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, player_camera.position, 0.2f);
            }
        } else
        {
            if (cube_rb.velocity.y == 0)
            {
                do_wave();
            }
            

            if (bus > 0)
            {
                bus--;
            }
            if (bus <= 0)
            {
                Collider[] hitColliders = Physics.OverlapSphere(gameObject.transform.position, 1.0f);
                foreach (Collider c in hitColliders)
                {
                    if (c.name == "Player")
                    {
                        flying = true;
                        ((Collider)gameObject.GetComponent(typeof(Collider))).enabled = false;
                        cube_rb.useGravity = false;

                        break;
                    }
                }

                bus = 0;
            }
            
        }
        
    }

    void do_wave()
    {
        gameObject.transform.Translate(Vector3.down * 0.001f);
        cube.localPosition = Vector3.up * (Mathf.Sin(wave) / 2 + 1f);
        wave += 0.02f;
    }
    void kill_pickup()
    {
        if (!dead)
        {
            dead = true;
            play_block_pop();
            GameObject.Destroy(gameObject, pop_sound.length);
        }
    }

    void play_block_pop()
    {
        GameObject obj = new GameObject();
        obj.transform.position = player_camera.position;
        obj.AddComponent<AudioSource>();

        AudioSource aud = (AudioSource)obj.GetComponent(typeof(AudioSource));
        aud.pitch = Random.Range(1.5f, 2.0f);
        aud.volume = 0.3f;
        aud.PlayOneShot(pop_sound, 1f);

        GameObject.Destroy(obj, pop_sound.length);
    }

}
