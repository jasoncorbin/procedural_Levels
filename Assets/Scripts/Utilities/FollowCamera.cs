using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] float cameraHeight = 10f;
    [SerializeField] float followSmoothing = 0.15f;

    Transform target;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("FollowCamera: no GameObject with tag 'Player' found.");
            return;
        }
        target = player.transform;

        // Snap to player immediately on first frame
        transform.position = TargetPosition();
    }

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = Vector3.Lerp(transform.position, TargetPosition(), followSmoothing);
    }

    Vector3 TargetPosition() => new Vector3(target.position.x, target.position.y, -cameraHeight);
}
