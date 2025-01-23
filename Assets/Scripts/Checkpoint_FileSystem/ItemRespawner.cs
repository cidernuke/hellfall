using UnityEngine;

public class ItemRespawner : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    public bool isCollected = false;

    /// <summary>
    /// Registers the item in the GameManager.
    /// </summary>
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
            Debug.LogError("GameManager.Instance is null, item can't be registered.");
        }
    }

    /// <summary>
    /// Collects the item.
    /// </summary>
    public void CollectItem()
    {
        isCollected = true;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Respawns the item.
    /// </summary>
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
            print("Item not collected, nothing to respawn: " + gameObject.name);
        }
    }
}
