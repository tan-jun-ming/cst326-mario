using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBreaker : MonoBehaviour
{
    public CameraMove cameramove;
    public PlayerMove playermove;
    public Texture[] destroy_stage;
    public Texture blank;
    public AudioClip[] dig_sound;
    public AudioClip[] break_sound;
    public AudioClip brick_sound;
    public AudioClip coin_sound;
    public AudioClip pop_sound;

    private int break_cooldown = 0;
    private Transform drop_camera;
    // Start is called before the first frame update
    void Start()
    {
        drop_camera = GameObject.Find("Camera 4").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (break_cooldown > 0)
        {
            break_cooldown--;
        }

        if (playermove.get_fps() ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0))
        {
            Ray ray = cameramove.get_camera().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            float distance = playermove.get_fps() ? 5f : 50f;

            if (Physics.Raycast(ray, out hit, distance))
            {
                break_block(hit.transform);
            }

        }
    }

    public void break_block(Transform block)
    {
        Block blockscript = ((Block)block.gameObject.GetComponent(typeof(Block)));

        if (playermove.get_fps())
        {
            if (block.CompareTag("block") && break_cooldown == 0)
            {
                int curr_durability = blockscript.damage(1);
                if (curr_durability <= 0)
                {
                    break_cooldown = 50;
                }
            }
        } else
        {
            blockscript.flat_destroy();
        }
        
    }
}
