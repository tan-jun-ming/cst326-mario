using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public BlockBreaker blockbreaker;
    public float speed_limit = 10f;
    public float turbo_speed_limit = 20f;
    public Animator animator;

    public AudioClip boing;
    public AudioClip deathsound;
    public AudioClip gameover;
    public AudioClip poleslide;
    public AudioClip clear;

    private Rigidbody player;

    private Vector3 last_position;
    private Quaternion target_rotation;

    private float run_strength = 0;

    private bool has_won = false;
    private bool dead = false;
    private int death_counter = 30;

    private float pole_pos = 0;
    private float pole_max = 18f;
    private int winphase = 0;

    private UIManager uimanager;

    private List<GameObject> afterimages = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        player = (Rigidbody)gameObject.GetComponent(typeof(Rigidbody));
        uimanager = ((UIManager)GameObject.Find("UIManager").GetComponent(typeof(UIManager)));

        last_position = gameObject.transform.position;
        target_rotation = Quaternion.Euler(0, -90, 0);
    }

    // Update is called once per frame
    void Update()
    {

        manage_afterimages();

        Transform model = GameObject.Find("MarioModel").transform;

        if (dead)
        {
            if (death_counter >= 0)
            {
                death_counter--;
            }
            if (death_counter == 0)
            {
                player.isKinematic = false;
                player.velocity = Vector3.up * 8 + Vector3.forward * 3;
            } else if (death_counter == -1 && gameObject.transform.position.y < 0)
            {
                player.isKinematic = true;
                uimanager.game_over();
                play_sound(2);

                death_counter--;
            }
            return;
        } else if (has_won)
        {
            Vector3 new_pos = gameObject.transform.position;
            float walk_speed = 0.05f;
            switch (winphase)
            {
                case 0:
                    float descend_speed = 0.1f;
                    if (pole_pos >= 8.8f)
                    {
                        new_pos.y = Mathf.Min(new_pos.y, pole_pos);
                    }
                    else if (pole_pos < 8.0f)
                    {
                        model.localRotation = Quaternion.Euler(0, 90, 0);
                        new_pos.x = -206.5f;
                        winphase++;
                    }
                    pole_pos -= descend_speed;
                    break;
                case 1:
                    player.isKinematic = false;
                    animator.SetBool("Dead", false);
                    animator.SetFloat("WalkBlender", 0.5f);
                    target_rotation = Quaternion.Euler(0, -90, 0);
                    winphase++;
                    break;
                case 2:
                    model.localRotation = Quaternion.RotateTowards(model.localRotation, target_rotation, 30f);
                    new_pos += Vector3.left * walk_speed;

                    if (new_pos.x < -208)
                    {
                        play_sound(4);
                        winphase++;
                    }
                    break;
                case 3:
                    new_pos += Vector3.left * walk_speed;
                    if (new_pos.x < -222f)
                    {
                        animator.SetFloat("WalkBlender", 0f);
                        player.isKinematic = true;
                        winphase++;
                    }
                    break;
                case 4:
                    new_pos += Vector3.left * walk_speed + Vector3.up * walk_speed;
                    gameObject.transform.localRotation *= Quaternion.Euler(0, 0, 5f);

                    break;

            }
            gameObject.transform.position = new_pos;

            return;
        }

        Vector3 velocity = player.velocity;

        float delta_x = Input.GetAxis("Horizontal");
        float jump = Input.GetAxis("Jump");
        float turbo = Input.GetAxis("Turbo");

        float target_direction = 0;

        float effective_speed_limit = turbo > 0 ? turbo_speed_limit : speed_limit;
        if (player.velocity.y == 0)
        {
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", false);
            if (gameObject.transform.position.y == last_position.y && jump > 0)
            {
                BoxCollider box = (BoxCollider)gameObject.GetComponent(typeof(BoxCollider));
                float boxsize = box.size.x/2;

                Vector3 feetpos = GameObject.Find("Feet").transform.position;
                float distance = 0.1f;

                bool to_jump = false;
                for (int i= -1; i < 1; i++)
                {
                    // Raycast to check if grounded
                    if (Physics.Raycast(feetpos + Vector3.right * i, Vector3.down, distance))
                    {
                        to_jump = true;
                        break;
                    }
                }

                
                if (to_jump)
                {
                    velocity += Vector3.up * jump * 10;
                    play_sound(0);
                    animator.SetBool("Jumping", true);
                }

            }
        }
        else if (gameObject.transform.position.y < last_position.y)
        {
            animator.SetBool("Falling", true);
        } else if (gameObject.transform.position.y > last_position.y && jump == 0)
        {
            velocity += Vector3.down * 0.3f;
        }

        if (delta_x != 0)
        {
            if (velocity.x > -effective_speed_limit && delta_x > 0)
            {
                velocity.x -= delta_x;
            } else if (velocity.x < effective_speed_limit && delta_x < 0)
            {
                velocity.x -= delta_x;
            }
            float run_strength_max = turbo > 0 ? 1.5f : 1f;

            run_strength = Mathf.Min(run_strength_max, run_strength + 0.1f);
            target_direction = delta_x > 0 ? -1 : 1;
            target_rotation = Quaternion.Euler(0, target_direction * 90, 0);

            animator.SetFloat("JumpBlender", target_direction == -1 ? 0 : 1);

        } else
        {
            run_strength = Mathf.Max(0f, run_strength - 0.1f);
        }

        if (turbo > 0 && velocity.magnitude > 0f) {
            add_afterimage();
        };

        model.localRotation = Quaternion.RotateTowards(model.localRotation, target_rotation, 30f);

        player.velocity = velocity;

        animator.SetFloat("WalkBlender", run_strength);

        last_position = gameObject.transform.position;
        

    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);

        if (contact.otherCollider.CompareTag("death"))
        {
            die();
        }

        if (contact.otherCollider.CompareTag("flag"))
        {
            win();
        }

        if (contact.normal.y == -1)
        {
            if (contact.otherCollider.CompareTag("block"))
            {
                blockbreaker.break_block(contact.otherCollider.transform);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("block"))
        {
            blockbreaker.break_block(collider.transform);
        }
    }

    void play_sound(int sound_type)
    {
        AudioClip to_play = null;
        float volume = 1f;

        switch (sound_type)
        {
            case 0:
                to_play = boing; break;
            case 1:
                to_play = deathsound; break;
            case 2:
                to_play = gameover; break;
            case 3:
                to_play = poleslide; break;

            case 4:
                to_play = clear; break;
        }
        AudioSource speaker = ((AudioSource)GameObject.Find("Camera Stand").GetComponent(typeof(AudioSource)));
        speaker.PlayOneShot(to_play, volume);

    }
    public void die()
    {
        if (!dead && !has_won)
        {
            dead = true;
            player.velocity = Vector3.zero;
            player.constraints = RigidbodyConstraints.FreezePositionX;
            player.detectCollisions = false;
            player.isKinematic = true;
            gameObject.transform.position += Vector3.forward * 2f;

            GameObject.Find("MarioModel").transform.localRotation = Quaternion.Euler(-30, 0, 0);
            play_sound(1);
            animator.SetBool("Dead", true);
        }
    }

    public void win()
    {
        if (!has_won)
        {
            has_won = true;
            player.velocity = Vector3.zero;
            player.isKinematic = true;

            GameObject.Find("MarioModel").transform.localRotation = Quaternion.Euler(0, -90, 0);

            Vector3 new_pos = gameObject.transform.position;
            new_pos.x = -205.5f; // Slide down the flag from here;
            new_pos.y = Mathf.Min(new_pos.y, pole_max);

            pole_pos = pole_max;

            gameObject.transform.position = new_pos;

            int earned_score = 5000;
            if (new_pos.y < 18f)
            {
                earned_score = 4000;
                if (new_pos.y < 16f)
                {
                    earned_score = 2000;
                    if (new_pos.y < 14f)
                    {
                        earned_score = 800;
                        if (new_pos.y < 12f)
                        {
                            earned_score = 400;
                            if (new_pos.y < 10f)
                            {
                                earned_score = 100;
                            }
                        }
                    }
                }
            }

            uimanager.add_score(earned_score);

            play_sound(3);
            animator.SetBool("Dead", true);
        }
    }

    void add_afterimage()
    {
        GameObject curr_model = GameObject.Find("MarioModel");

        Vector3 afterimage_pos = last_position;
        afterimage_pos.y += curr_model.transform.localPosition.y;

        GameObject afterimage = GameObject.Instantiate(curr_model, afterimage_pos, target_rotation);
        afterimage.name = "Afterimage";
        Destroy(afterimage.GetComponent(typeof(Animator)));

        foreach (Transform c in afterimage.transform)
        {
            Renderer r = (Renderer)c.gameObject.GetComponent(typeof(Renderer));
            if (r != null)
            {
                StandardShaderUtils.ChangeRenderMode(r.material, StandardShaderUtils.BlendMode.Fade);
            }
        }

        afterimages.Add(afterimage);
    }

    void manage_afterimages()
    {
        

        int i = 0;
        while (i < afterimages.Count){
            GameObject a = afterimages[i];
            bool kill_obj = false;
            foreach (Transform c in a.transform)
            {
                Renderer r = (Renderer)c.gameObject.GetComponent(typeof(Renderer));
                if (r != null)
                {

                    Color color = r.material.color;
                    color.a -= 0.1f;
                    r.material.color = color;

                    if (color.a <= 0)
                    {
                        kill_obj = true;
                        break;
                    }
                }
            }

            if (kill_obj)
            {
                afterimages.RemoveAt(i);
                GameObject.Destroy(a);
                continue;
            }
            i++;
        }

    }

}
