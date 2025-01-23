using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Transform player;

    /// <summary>
    /// Sets the position of the minimap camera to the player's position.
    /// </summary>
    void LateUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.z = transform.position.z;
        transform.position = newPosition;
    }
}
