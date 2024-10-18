

using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;
    private float currentPosX;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float cameraSpeed;
    private float lookAhead;
    [SerializeField] private Transform player;

    private void Update()
    {
        transform.position = new Vector3(player.position.x + lookAhead, player.position.y, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * cameraSpeed);
    }


}
