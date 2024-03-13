using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KeySystem
{
    public class KeyDoorController : MonoBehaviour
    {
        private Animator doorAnim;
        private bool doorOpen = false;

        [SerializeField] private GameObject greenDoor;

        [Header("Animation Names")]
        [SerializeField] private string openAnimationName = "DoorOpen";
        [SerializeField] private string closeAnimationName = "DoorClose";

        [SerializeField] private int timeToShowUI = 1;
        [SerializeField] private GameObject showDoorLockedUI = null;

        [SerializeField] private KeyInventory _keyInventory = null;

        [SerializeField] private int waitTimer = 1;
        [SerializeField] private bool pauseInteraction = false;

        private void Awake() 
        {
            doorAnim = gameObject.GetComponent<Animator>();
        }

        private IEnumerator PauseDoorInteraction()
        {
            pauseInteraction = true;
            yield return new WaitForSeconds(waitTimer);
            pauseInteraction = false;
        }

        public void PlayAnimation()
        {
            if (_keyInventory.hasYellowKey)
            {
                //OpenDoor();
                Debug.Log("Door is open!");
                Destroy(GameObject.Find("YellowDoor"));
            }

            else if (_keyInventory.hasGreenKey)
            {
                //OpenDoor();
                Debug.Log("Door is open!");
                Destroy(GameObject.Find("GreenDoor"));
            }
            else
            {
                StartCoroutine(ShowDoorLocked());
            }
        }

        void OpenDoor()
        {
            if(!doorOpen && !pauseInteraction)
            {
                doorAnim.Play(openAnimationName, 0, 0.0f);
                doorOpen = true;
                StartCoroutine(PauseDoorInteraction());
            }

            else if (doorOpen && !pauseInteraction)
            {
                doorAnim.Play(closeAnimationName, 0, 0.0f);
                doorOpen = false;
                StartCoroutine(PauseDoorInteraction());
            }
        }

        IEnumerator ShowDoorLocked()
        {
          showDoorLockedUI.SetActive(true);
          yield return new WaitForSeconds(timeToShowUI);
          showDoorLockedUI.SetActive(false);
        }
    }
}
