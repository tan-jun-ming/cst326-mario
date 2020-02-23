using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    public enum BlockType {Coin, Coinbox, BreakableBlock, UnbreakableBlock };
    public BlockType blocktype;

    private AudioClip brick_sound;
    private AudioClip coin_sound;

    private Texture hitblock;
    private Texture[] coin_anim;

    private UIManager uimanager;

    private bool dead = false;

    private ParticleSystem ps;

    private int coin_anim_counter = 0;
    private int coin_anim_counter_max = 16; // how many frames
    private int coin_anim_frame_counter = 0;
    private int coin_anim_frame_counter_max = 8; // how long each frame is
    private int coin_anim_frame_index_counter = 0;


    // Start is called before the first frame update
    void Start()
    {
        uimanager = ((UIManager)GameObject.Find("UIManager").GetComponent(typeof(UIManager)));

        if (gameObject.transform.childCount > 0)
        {
            ps = (ParticleSystem)gameObject.transform.GetChild(0).GetComponent(typeof(ParticleSystem));
        }

        BlockBreaker blockbreaker = (BlockBreaker)GameObject.Find("BlockBreaker").GetComponent(typeof(BlockBreaker));
        brick_sound = blockbreaker.brick_sound;
        coin_sound = blockbreaker.coin_sound;
        hitblock = blockbreaker.hitblock;
        coin_anim = blockbreaker.coin_anim;
    }

    // Update is called once per frame
    void Update()
    {
        if (coin_anim_counter > 0)
        {
            if (coin_anim_frame_counter <= 0)
            {
                int ind = coin_anim_frame_index_counter;
                Texture new_texture = coin_anim[ind];
                ((ParticleSystemRenderer)gameObject.transform.GetChild(0).GetComponent(typeof(ParticleSystemRenderer))).material.SetTexture("_MainTex", new_texture);

                coin_anim_frame_counter = coin_anim_frame_counter_max;
                coin_anim_counter--;
                coin_anim_frame_index_counter = (coin_anim_frame_index_counter + 1) % coin_anim.Length;

            }

            coin_anim_frame_counter--;
        }
    }

    void kill_with_delay()
    {
        if (!dead)
        {
            dead = true;
            ((Renderer)gameObject.GetComponent(typeof(Renderer))).enabled = false;
            ((Collider)gameObject.GetComponent(typeof(Collider))).enabled = false;

            GameObject.Destroy(gameObject, 10f);

        }
    }

    public void flat_destroy()
    {
        int to_play = 1;
        if (blocktype == BlockType.Coinbox || blocktype == BlockType.Coin)
        {
            to_play = 3;
            uimanager.add_coins(1);

            if (blocktype == BlockType.Coin)
            {
                kill_with_delay();
            } else
            {
                blocktype = BlockType.UnbreakableBlock;
                ((Renderer)gameObject.GetComponent(typeof(Renderer))).material.SetTexture("_MainTex", hitblock);

                ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();
                ep.velocity = Vector3.forward * 6f;

                ps.Emit(ep, 1);

                coin_anim_counter = coin_anim_counter_max;
                coin_anim_frame_counter = coin_anim_frame_counter_max;

            }
        } else if (blocktype == BlockType.BreakableBlock)
        {
            to_play = 2;

            uimanager.add_score(100);
            ParticleSystem.EmitParams ep = new ParticleSystem.EmitParams();


            ep.position = Vector3.forward * 1f;
            for (int i=0; i<4; i++)
            {
                float vertical_dir = i % 2 == 0 ? 5 : 3;
                float horizontal_dir = 1.5f * (i / 2 == 0 ? 1 : -1);

                ep.velocity = (Vector3.forward + Vector3.up * vertical_dir + Vector3.left * horizontal_dir);
                ep.angularVelocity = Random.Range(0, 2) == 0 ? 90f : -90f;
                ep.rotation = Random.Range(0f, 359f);

                ps.Emit(ep, 1);
            }

            kill_with_delay();
        }

        play_sound(to_play);
    }

    void play_sound(int sound_type)
    {
        AudioClip to_play = null;
        float volume = 1f;

        switch (sound_type)
        {
            case 1:
                return; // play nothing
            case 2:
                to_play = brick_sound; break;
            case 3:
                to_play = coin_sound; break;
        }
        AudioSource speaker = ((AudioSource)GameObject.Find("Camera Stand").GetComponent(typeof(AudioSource)));
        speaker.PlayOneShot(to_play, volume);
    }
}
