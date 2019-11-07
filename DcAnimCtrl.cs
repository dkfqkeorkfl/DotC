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

    public Animator tartget;
    public float idle_min = 1, idle_max = 2;
    public STATE state { get; private set; }
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
        
        Debug.Log(string.Format("{0} - {1}", horizontal, vertical));
        if (vertical > 0) // If the Space bar is pressed down then continue
        {
            tartget.SetInteger("state", STATE.WALK.GetHashCode());
            tartget.SetBool("right", false);
            tartget.SetBool("left", false);
        }
        else if (horizontal < 0) // If the Space bar is pressed down then continue
        {
            tartget.SetInteger("state", STATE.WALK.GetHashCode());
            tartget.SetBool("right", false);
            tartget.SetBool("left", true);
        }
        else if (horizontal > 0) // If the Space bar is pressed down then continue
        {
            tartget.SetInteger("state", STATE.WALK.GetHashCode());
            tartget.SetBool("right", true);
            tartget.SetBool("left", false);
        }
        else
        {
            tartget.SetInteger("state", STATE.IDLE1.GetHashCode());
            tartget.SetBool("right", false);
            tartget.SetBool("left", false);
        }
    }

    void Update()
    {
        
    }
}
