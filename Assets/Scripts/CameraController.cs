

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
    [SerializeField] private Transform player;
    //Check Doc for more info
    private void Update()
    {
        transform.position = new Vector3(player.position.x + lookAhead, player.position.y + aheadY, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, aheadDistance * player.localScale.x, Time.deltaTime * cameraSpeed);
    }


}
