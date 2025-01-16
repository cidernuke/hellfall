using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Sets the distance for how far the camera looks ahead, configurable in the editor
    [SerializeField] private float aheadDistance;
    //adds the number to the y position of the player
    [SerializeField] private float aheadY;
    //Sets the speed at which the camera will adjust its position, configurable in the Editor
    [SerializeField] private float cameraSpeed;
    //tracks how far ahead the camera is looking currently
    private float lookAhead;
    //Sets the player which the camera tracks, configurable in the editor

    //[SerializeField] private Transform player;
    private Transform player;

    private void Awake()
    {
        // Falls du den Player nicht schon im Inspector zugewiesen hast,
        // kannst du ihn zur Laufzeit per Tag "Player" suchen:
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            else
            {
                Debug.LogError("No GameObject with tag 'Player' found in the scene!");
            }
        }
    }

    //Check Doc for more info
    private void Update()
    {
        if (player == null)
        {
            // Solange kein Player gefunden ist, nichts tun
            return;
        }

        transform.position = new Vector3(player.position.x + lookAhead, player.position.y + aheadY, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, aheadDistance * player.localScale.x, Time.deltaTime * cameraSpeed);
    }


}
