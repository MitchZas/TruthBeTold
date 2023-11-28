using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    AudioSource source;
    
    void Awake()
    {
        source = GetComponent<AudioSource>();
        GameObject baseball = GameObject.Find("Baseball");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && gameObject.name == "Baseball")
            {
                source.Play();
            }
    }
}
