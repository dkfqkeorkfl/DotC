using UnityEngine;
using System.Collections;

public class DcClickToMove : MonoBehaviour
{
    
    readonly float minMove = 0.1f, maxMove = 500.0f;
   
    public float speed = 10f;
    // Movement variables
    private Vector3 destinationPosition;
    private float destinationDistance;
    private new Rigidbody rigidbody;

    public bool is_running {
        get {
            return destinationDistance >= minMove && destinationDistance <= maxMove;
        }
    }
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>(); // Get the animation script
    }

    // FixedUpdate is used for physics based movement
    void FixedUpdate()
    {
        MovementControl(); // Player movement function
    }

    private void OnDisable()
    {
        destinationDistance = 0.0f;
        destinationPosition = transform.position;
    }

    private void MovementControl()
    {
        //Debug.Log(string.Format("{0}", destinationPosition.ToString()));
        MovePlayer(); // Player Move function

        if (Input.GetMouseButton(0)) // If left mouse button is clicked or held down
        {
            RotatePlayer(); // Player Rotate function
        }
        destinationPosition.y = transform.position.y; // Set the destination Y position to your local Y position (allows you to move up ramps)
        destinationDistance = Vector3.Distance(destinationPosition, transform.position); // Distance between the player and where clicked
    }

    private void MovePlayer()
    {
        if (is_running)// If the distance between the player and clicked is greater than the minimum range and less than the maximum range
        {
            rigidbody.MovePosition(rigidbody.position + transform.forward * speed * Time.deltaTime); // Move forward based on players Vector3
            Debug.DrawLine(destinationPosition, transform.position, Color.cyan); // This draws a line in Scene View so you can see where you've clicked
        }
        else // If the distance between the player and clicked is less than the min range and less than the max range then continue
        {
        }
    }

    public void RotatePlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Set ray to the position of your mouse
        Plane playerPlane = new Plane(Vector3.up, transform.position); // Create a plane for the raycast
        float hitdist = 0.0f; // set a float for the position of your click
        if (playerPlane.Raycast(ray, out hitdist)) // If the Raycast has hit something (in this case, the mouse position) then continue
        {
            Vector3 targetPoint = ray.GetPoint(hitdist); // Set a Vector3 for position clicked
            destinationPosition = targetPoint; // Set destination position to position clicked
            rigidbody.MoveRotation(Quaternion.LookRotation(targetPoint - transform.position)); // Rotate player towards position clicked
        }
    }
}