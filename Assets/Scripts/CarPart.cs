using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPart : MonoBehaviour
{
    public GameObject carPartUI;
    public GameObject carPart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            carPartUI.SetActive(true);
            Debug.Log("Ты подобрал 1 часть машины");
            carPart.SetActive(false);
        }
    }
}
