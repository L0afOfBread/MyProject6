using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EyeSight : MonoBehaviour
{
    public TMP_Text actionText;
    public bool canDrive = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("repairedCar"))
        {
            actionText.text = "[E] Drive";
            canDrive = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("repairedCar"))
        {
            actionText.text = "";
            canDrive = false;
        }
    }
}
