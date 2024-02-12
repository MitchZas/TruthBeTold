using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using TransitionsPlus;

public class InspectRaycast : MonoBehaviour
{
   public GameObject baseball;
   public GameObject cylinder;
   public GameObject box;


   
   [SerializeField] private int rayLength = 5;
   [SerializeField] private LayerMask layerMaskInteract;
   private ObjectController raycastedObj;

   [SerializeField] private Image crosshair;
   private bool isCrosshairActive;
   
   private bool doOnce;

   public bool keepRepeating = true;

   public FirstPersonController firstPersonControllerScript;

   public Texture2D picture;

  
private void Start() 
{
   baseball = GameObject.Find("Baseball");
   cylinder = GameObject.Find("Cylinder");
   box = GameObject.Find("Box");
}
  
  private void Update() 
   {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, layerMaskInteract.value))
        {
            if (hit.collider.CompareTag("InteractObject"))
            {
                if (!doOnce)
                {
                    raycastedObj = hit.collider.gameObject.GetComponent<ObjectController>();
                    raycastedObj.ShowObjectName();
                    CrosshairChange(true);
                    
                }

                isCrosshairActive = true;
                doOnce = true;

                if(Input.GetMouseButtonDown(0))
                {
                    StartCoroutine(LevelChange());
                    //keepRepeating = false;   
                }
            }
        }
        else
        {
           if (isCrosshairActive)
           {
                raycastedObj.HideObjectName();
                CrosshairChange(false);
                doOnce = false;
           }
        }
    }

   void CrosshairChange(bool on)
   {
        if (on && !doOnce)
        {
            crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
            isCrosshairActive = false;
        }
   }

   IEnumerator LevelChange()
   {
        raycastedObj.ShowExtraInfo();
        
        if(raycastedObj.gameObject.name == baseball.name)
        {
            FindObjectOfType<AudioManager>().Play("ShannonMitchBaseball");
        }
        else if (raycastedObj.gameObject.name == cylinder.name)
        {
            FindObjectOfType<AudioManager>().Play("CylinderSound");
        }

        else if (raycastedObj.gameObject.name == box.name)
        {
            FindObjectOfType<AudioManager>().Play("BoxTest");
        }
        
        firstPersonControllerScript.canMove = false;

        yield return new WaitForSeconds(5);

        TransitionAnimator.Start(TransitionType.DoubleWipe, vignetteIntensity: .45f, color: Color.white, sceneNameToLoad: "Level2");
   }
}
