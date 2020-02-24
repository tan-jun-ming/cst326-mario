using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBreaker : MonoBehaviour
{
    public CameraMove cameramove;
    public PlayerMove playermove;

    public AudioClip brick_sound;
    public AudioClip coin_sound;

    public Texture hitblock;
    public Texture[] coin_anim;

    public Material debris;

    private int break_cooldown = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (break_cooldown > 0)
        {
            break_cooldown--;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cameramove.get_camera().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            float distance = 50f;

            if (Physics.Raycast(ray, out hit, distance))
            {
                break_block(hit.transform);
            }

        }
    }

    public void break_block(Transform block)
    {
        Block blockscript = ((Block)block.gameObject.GetComponent(typeof(Block)));

        blockscript.flat_destroy();
        
    }

    public void entered_trigger(Transform block)
    {
        Block blockscript = ((Block)block.gameObject.GetComponent(typeof(Block)));

        blockscript.entered_trigger();

    }
}
