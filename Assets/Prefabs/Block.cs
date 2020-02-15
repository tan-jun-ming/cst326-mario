using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int max_durability = 100;
    public int curr_durability;
    public GameObject dropitem;

    public bool coinbox = false;

    private Texture[] destroy_stage;
    private Texture blank;
    private AudioClip[] dig_sound;
    private AudioClip[] break_sound;
    private AudioClip brick_sound;
    private AudioClip coin_sound;

    private UIManager uimanager;

    private bool remain = true;

    private int dig_interval = 0;

    private bool dead = false;
    private bool released = true;

    // Start is called before the first frame update
    void Start()
    {
        uimanager = ((UIManager)GameObject.Find("UIManager").GetComponent(typeof(UIManager)));

        BlockBreaker blockbreaker = (BlockBreaker)GameObject.Find("BlockBreaker").GetComponent(typeof(BlockBreaker));
        destroy_stage = blockbreaker.destroy_stage;
        blank = blockbreaker.blank;
        dig_sound = blockbreaker.dig_sound;
        break_sound = blockbreaker.break_sound;
        brick_sound = blockbreaker.brick_sound;
        coin_sound = blockbreaker.coin_sound;

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

        if (curr_durability != max_durability)
        {
            int stage = (int)Mathf.Floor(((float)curr_durability / (float)max_durability) * (float)destroy_stage.Length);
            set_texture(destroy_stage[destroy_stage.Length-1-stage]);
            remain = false;

            if (curr_durability <= 0)
            {
                play_sound(1, false);

                kill_with_delay(true);
            }
        } else if (!remain)
        {
            set_texture(blank);
            remain = true;
        }

        released = true;

    }

    void kill_with_delay(bool drop_item)
    {
        if (!dead)
        {
            dead = true;
            ((Renderer)gameObject.GetComponent(typeof(Renderer))).enabled = false;
            ((Collider)gameObject.GetComponent(typeof(Collider))).enabled = false;

            if (drop_item)
            {
                GameObject drop = Instantiate(dropitem, gameObject.transform.position, Quaternion.identity);
                ((Renderer)drop.transform.GetChild(0).GetComponent(typeof(Renderer))).material = ((Renderer)gameObject.GetComponent(typeof(Renderer))).material;
            }

            GameObject.Destroy(gameObject, 1f);

        }
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
            play_sound(0, false);
        }

        dig_interval++;
        dig_interval = dig_interval % 15;

        return curr_durability;
    }

    public void flat_destroy()
    {
        int to_play = 2;
        if (coinbox)
        {
            to_play = 3;
            uimanager.add_coins(1);
        } else
        {
            uimanager.add_score(50);
            kill_with_delay(false);
        }

        play_sound(to_play, true);
    }

    void play_sound(int sound_type, bool play_global)
    {
        AudioClip to_play = null;
        float volume = 1f;

        switch (sound_type)
        {
            case 0:
                to_play = dig_sound[Random.Range(0, dig_sound.Length)]; volume = 0.5f;  break;
            case 1:
                to_play = break_sound[Random.Range(0, break_sound.Length)]; volume = 2.0f; break;
            case 2:
                to_play = brick_sound; break;
            case 3:
                to_play = coin_sound; break;
        }
        AudioSource speaker = ((AudioSource)(play_global ? GameObject.Find("Camera Speaker") : gameObject).GetComponent(typeof(AudioSource)));
        speaker.PlayOneShot(to_play, volume);
    }

    void set_texture(Texture tex)
    {
        ((Renderer)gameObject.GetComponent(typeof(Renderer))).materials[1].SetTexture("_MainTex", tex);
    }
}
