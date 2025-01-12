using UnityEngine;

public class ItemRespawner : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public bool isCollected = false;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterItem(this);
        }
        else
        {
            Debug.LogError("GameManager.Instance ist null, Item kann nicht registriert werden.");
        }
    }

    public void CollectItem()
    {
        isCollected = true;
        gameObject.SetActive(false);
    }

    public void RespawnItem()
    {
        //if the item is a weapon it doesn't respawn
        //Layer 10 is the current Weapon Layer
        if(gameObject.layer == 10 && isCollected)
        {
            return;
        }
        if (isCollected)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
            gameObject.SetActive(true);
            isCollected = false;
            print("Item respawned: " + gameObject.name);
        }
        else
        {
            print("Item nicht collected, nichts zu respawnen: " + gameObject.name);
        }
    }
}
