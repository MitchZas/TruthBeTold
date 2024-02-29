using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
    public class KeyItemController : MonoBehaviour
    {
        [SerializeField] private bool yellowDoor = false;
        [SerializeField] private bool yellowKey = false;

        [SerializeField] private bool greenDoor = false;
        [SerializeField] private bool greenKey = false;

        [SerializeField] private KeyInventory _keyInventory = null;

        private KeyDoorController doorObject;

        private void Start() 
        {
            if (yellowDoor)
            {
                doorObject = GetComponent<KeyDoorController>();
            }

            else if(greenDoor)
            {
                doorObject = GetComponent<KeyDoorController>();
            }
        }

        public void ObjectInteraction()
        {
            if (yellowDoor)
            {
                doorObject.PlayAnimation();
            }

            else if (yellowKey)
            {
                _keyInventory.hasYellowKey = true;
                gameObject.SetActive(false);
            }

            if (greenDoor)
            {
                doorObject.PlayAnimation();
            }

            else if (greenKey)
            {
                _keyInventory.hasYellowKey = true;
                gameObject.SetActive(false);
            }
        }
    }
}
