using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disable_UI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject ui = GameObject.Find("PlayerUI");
        if (ui)
       {
        ui.SetActive(false);
       } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
