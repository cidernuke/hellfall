using UnityEngine;

public class ItemRespawner : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isCollected = false;


    // private void Awake()
    // {
    //     initialPosition = transform.position;
    //     initialRotation = transform.rotation;
    //     GameManager.Instance.RegisterItem(this);
    // }

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
        // if (renderer != null) renderer.enabled = false;

        // var collider = GetComponent<Collider2D>();
        // if (collider != null) collider.enabled = false;
    }

    public void RespawnItem()
    {
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
