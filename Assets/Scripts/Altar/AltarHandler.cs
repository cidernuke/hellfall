using UnityEngine;

public class AltarHandler : MonoBehaviour
{    
    private AltarGUI altarGUI;
    private bool playerInAltar = false;
    
    // Start is called before the first frame update
    void Start()
    {
        altarGUI = GetComponentInChildren<AltarGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInAltar && Input.GetKeyDown(KeyCode.I))
        {            
            altarGUI.toggleAltarGUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {            
            playerInAltar = true;
        }
    }
}



