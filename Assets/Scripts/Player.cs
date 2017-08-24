using System.Collections;
using UnityEngine;

/// <summary>
///Takes input from the user and then sends to the Controller script
///Raycast Controller handles player movement, slopes and moving platforms far more consistently than Rigidbody
/// </summary>

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    //Much more intuitive values than gravity and jumpVelocity to play with...
    //How high do you want the character to jump
    float jumpHeight = 2.5f;
    //How much time does the character require to get to the max height
    float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accellerationTimeGrounded = .1f;

    float moveSpeed = 4f;
    Controller2D controller;
    float gravity;
    float jumpVelocity;
    Vector3 velocity;

    float velocityXSmoothing;

    //s = ut + 0.5*g*t*t;

    private void Start() {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
    }

    private void Update() {
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below) {
            velocity.y = jumpVelocity;     
        }
        float targetVelocityX = input.x * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, controller.collisions.below? accellerationTimeGrounded:accelerationTimeAirborne);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }


}
