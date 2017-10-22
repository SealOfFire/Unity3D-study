using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private Animator anim;

    // Use this for initialization
    private void Start()
    {
        this.anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // anim.SetBool("standing",false);
            anim.SetInteger("manact", 1);
            Debug.Log("GetKeyDown");
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            anim.SetInteger("manact", 0);
            Debug.Log("GetKeyUp");
        }
    }
}
