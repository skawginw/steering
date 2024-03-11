using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float maxSpeed = 5f;
    public float rotationSpeed = 10f;
    public float slowingDistance = 2f; // Distance at which player starts slowing down
    private Vector3 targetPosition;
    public float whiskerLength = 0.5f; // Length of the whiskers
    public float whiskerAngle = 45f; // Angle of the whiskers from the forward direction
    public float raycastDistance = 1f; // Distance to cast the ray
    public LayerMask obstacleLayer; // Layer mask for obstacles
    public float avoidanceForce = 2f; // Force to apply for avoiding obstacles
    public Text speedText; // UI text to display player speed

    private Vector2 moveDirection;

    private void FixedUpdate()
    {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0))
        {
            // Set the target position to the mouse click position
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = transform.position.z; // Ensure the z position is the same as the player
            moveDirection = (targetPosition - transform.position).normalized;
        }

        // Calculate direction to move
        Vector2 direction = (targetPosition - transform.position).normalized;

        // Calculate current speed based on distance to target
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float currentSpeed = maxSpeed;

        // Adjust speed based on distance to target
        if (distanceToTarget < slowingDistance)
        {
            currentSpeed *= distanceToTarget / slowingDistance;
        }


        // Cast a raycast in the forward direction
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, whiskerLength, obstacleLayer);
        // Cast whiskers at angles from the forward direction
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Quaternion.AngleAxis(-whiskerAngle, Vector3.forward) * transform.right, whiskerLength, obstacleLayer);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Quaternion.AngleAxis(whiskerAngle, Vector3.forward) * transform.right, whiskerLength, obstacleLayer);

        // Draw debug lines to visualize the raycasts
        Debug.DrawRay(transform.position, transform.right * raycastDistance, Color.red);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(-whiskerAngle, Vector3.forward) * transform.right * whiskerLength, Color.blue);
        Debug.DrawRay(transform.position, Quaternion.AngleAxis(whiskerAngle, Vector3.forward) * transform.right * whiskerLength, Color.green);

        // Check if any of the raycasts hit an obstacle
        if (hit.collider != null || hitLeft.collider != null || hitRight.collider != null)
        {
            // Calculate a new direction to avoid the obstacle
            Vector2 avoidanceDirection = Vector2.zero;
            if (hitLeft.collider != null && hitRight.collider != null)
            {
                avoidanceDirection = (hitRight.point - hitLeft.point).normalized;
            }
            else if (hitLeft.collider != null)
            {
                avoidanceDirection = hitLeft.normal;
            }
            else if (hitRight.collider != null)
            {
                avoidanceDirection = hitRight.normal;
            }
            if (hit.collider != null)
            {
                avoidanceDirection = Vector2.Perpendicular(transform.right).normalized;
            }
            // Apply avoidance force
            moveDirection += avoidanceDirection * avoidanceForce;
        }

        // Normalize the move direction
        moveDirection.Normalize();

        // Calculate player speed in m/s
        float speedInMetersPerSecond = currentSpeed;

        // Display the speed in the UI text
        if (speedText != null)
        {
            speedText.text = "Speed: " + speedInMetersPerSecond.ToString("F2") + " m/s";
        }

        // Move the player in the calculated direction
        transform.position += (Vector3)moveDirection * currentSpeed * Time.fixedDeltaTime;

        // Rotate the player to face the direction of movement
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        // Check if the player has reached the target position
        if (distanceToTarget <= 0.5f)
        {
            moveDirection = Vector2.zero; // Stop movement
            speedText.text = "Current Speed : 0.00 m/s";
        }
    }
}
