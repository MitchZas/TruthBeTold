using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class InspectRaycast : MonoBehaviour
{
   [SerializeField] private int rayLength = 5;
   [SerializeField] private LayerMask layerMaskInteract;
   private ObjectController raycastedObj;

   [SerializeField] private Image crosshair;
   private bool isCrosshairActive;
   
   private bool doOnce;

  //public LevelLoader levelloader;

  void Start() 
  {
    //levelloader.LoadNextLevel();
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
                    //raycastedObj.ShowExtraInfo();
                    //FindObjectOfType<Audio Manager>().Play("ShannonMitchBaseball");   
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
        FindObjectOfType<AudioManager>().Play("ShannonMitchBaseball");

        yield return new WaitForSeconds(8);

        SceneManager.LoadScene(1);
   }
}
