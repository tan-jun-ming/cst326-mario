using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int max_durability = 100;
    public int curr_durability;

    public bool coinbox = false;

    private Texture[] destroy_stage;
    private Texture blank;
    private AudioClip[] dig_sound;
    private AudioClip[] break_sound;
    private bool remain = true;

    private int dig_interval = 0;
    private int death_timer = -1;

    private bool released = true;

    // Start is called before the first frame update
    void Start()
    {
        BlockBreaker blockbreaker = (BlockBreaker)GameObject.Find("BlockBreaker").GetComponent(typeof(BlockBreaker));
        destroy_stage = blockbreaker.destroy_stage;
        blank = blockbreaker.blank;
        dig_sound = blockbreaker.dig_sound;
        break_sound = blockbreaker.break_sound;

        curr_durability = max_durability;
    }

    // Update is called once per frame
    void Update()
    {
        if (released)
        {
            curr_durability = max_durability;
            dig_interval = 0;
        }

        if (death_timer > 0)
        {
            death_timer--;

            if (death_timer <= 0)
            {
                GameObject.Destroy(gameObject);
            }
        }
        if (curr_durability != max_durability)
        {
            int stage = (int)Mathf.Floor(((float)curr_durability / (float)max_durability) * (float)destroy_stage.Length);
            set_texture(destroy_stage[destroy_stage.Length-1-stage]);
            remain = false;

            if (curr_durability <= 0)
            {
                play_sound(1);

                ((Renderer)gameObject.GetComponent(typeof(Renderer))).enabled = false;
                ((Collider)gameObject.GetComponent(typeof(Collider))).enabled = false;

                death_timer = 20;
            }
        } else if (!remain)
        {
            set_texture(blank);
            remain = true;
        }

        released = true;

    }
    public int damage(int amount)
    {
        if (max_durability > 0)
        {
            curr_durability -= amount;
            released = false;
        }

        if (dig_interval == 0)
        {
            play_sound(0);
        }

        dig_interval++;
        dig_interval = dig_interval % 15;

        return curr_durability;
    }

    void play_sound(int sound_type)
    {
        AudioClip to_play = null;
        float volume = 1f;

        switch (sound_type)
        {
            case 0:
                to_play = dig_sound[Random.Range(0, dig_sound.Length)]; volume = 0.5f;  break;
            default:
                to_play = break_sound[Random.Range(0, break_sound.Length)]; volume = 2.0f; break;
        }
        AudioSource speaker = (AudioSource)gameObject.GetComponent(typeof(AudioSource));
        speaker.PlayOneShot(to_play, volume);
    }

    void set_texture(Texture tex)
    {
        ((Renderer)gameObject.GetComponent(typeof(Renderer))).materials[1].SetTexture("_MainTex", tex);
    }
}
