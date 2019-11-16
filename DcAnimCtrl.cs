using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DcAnimCtrl : MonoBehaviour
{
    public enum STATE
    {
        IDLE1, IDLE2, IDLE3,
        WALK, RUN
    }

    public enum SIDE
    {
        FRONT, LEFT, RIGHT
    }

    public DcCamera main_camera = null;
    public Animator tartget = null;
    public DcClickToMove mover = null;
    public float idle_min = 1, idle_max = 2;

    // Start is called before the first frame update
    void Start()
    {
        Random.Range(idle_min, idle_max);
        tartget = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal"); // set a float to control horizontal input
        float vertical = Input.GetAxis("Vertical"); // set a float to control vertical input
        if (main_camera.is_active)
        {
            mover.enabled = false;
            horizontal = 0;
            vertical = 0;
        }
        else
        {
            mover.enabled = true;
        }

        if (mover.is_running || vertical > 0) // If the Space bar is pressed down then continue
        {
            if (mover.is_running)
                Debug.Log("run!!");
            else
                Debug.Log("vertical!!");
            tartget.SetInteger("state", STATE.WALK.GetHashCode());
            tartget.SetInteger("side", SIDE.FRONT.GetHashCode());
        }
        else if (horizontal < 0) // If the Space bar is pressed down then continue
        {
            tartget.SetInteger("state", STATE.WALK.GetHashCode());
            tartget.SetInteger("side", SIDE.LEFT.GetHashCode());
        }
        else if (horizontal > 0) // If the Space bar is pressed down then continue
        {
            tartget.SetInteger("state", STATE.WALK.GetHashCode());
            tartget.SetInteger("side", SIDE.RIGHT.GetHashCode());
        }
        else
        {
            tartget.SetInteger("state", STATE.IDLE1.GetHashCode());
            tartget.SetInteger("side", SIDE.FRONT.GetHashCode());
        }
    }
}
