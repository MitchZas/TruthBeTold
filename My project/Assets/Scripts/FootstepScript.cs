using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepScript : MonoBehaviour
{
    public GameObject footstep;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("w"))
        {
            footsteps();
        }

        if(Input.GetKeyDown("s"))
        {
            footsteps();
        }

        if(Input.GetKeyDown("a"))
        {
            footsteps();
        }
        
        if(Input.GetKeyDown("d"))
        {
            footsteps();
        }

        if(Input.GetKeyUp("w"))
        {
            StopFootstep();
        }

        if(Input.GetKeyUp("s"))
        {
            StopFootstep();
        }

        if(Input.GetKeyUp("a"))
        {
            StopFootstep();
        }

        if(Input.GetKeyUp("d"))
        {
            StopFootstep();
        }
    }

    void footsteps()
    {
        footstep.SetActive(true);
    }

    void StopFootstep()
    {
        footstep.SetActive(false);
    }
}
