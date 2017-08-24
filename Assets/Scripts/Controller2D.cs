using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {


    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;

    //Send the rays from inside of the Player
    const float skinWidth = .015f;
    //Number of rays to be fired...Controller from the inspector
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    /// <summary>
    /// Spacing between the rays....The bounds width of the collider divided by the number of rays in a particular direction
    /// </summary>
    float horizontalRaySpacing, verticalRaySpacing;

    public LayerMask collisionMask;
    public CollisionInfo collisions;

    float maxClimbAngle = 80;

    void Start() {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

   
    //Update the raycast origins...
    //Check for vertical Collisions and then translate
    public void Move(Vector3 velocity) {
        UpdateRaycastOrigins();
        collisions.Reset();


        if(velocity.y != 0)
            VerticalCollisions(ref velocity);

        if(velocity.x != 0)
            HorizontalCollisions(ref velocity);

        transform.Translate(velocity);
    }

    void VerticalCollisions(ref Vector3 velocity) {

        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        for (int i = 0; i < verticalRayCount; i++) {
            //If the ray direction is going to the bottom, then choose the bottomLeft as the origin for the rays
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;


            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i, Vector2.up * -2, Color.red);
            if (hit) {

                //When hit.distance - skinWidth = 0, this means that the object comes to rest
                //The hit.distance - skinWidth will become 0 when the object will totally collide with the surface of the floor...
                velocity.y = (hit.distance - skinWidth) * directionY;

                //To avoid the Player from going through the object when it is in its path...
                //Very important...do not miss
                rayLength = hit.distance;

                collisions.above = (directionY == 1);
                collisions.below = directionY == -1;
            }
        }
    }

    void HorizontalCollisions(ref Vector3 velocity) {

        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        for (int i = 0; i < horizontalRayCount; i++) {
            //If the ray direction is going to the bottom, then choose the bottomLeft as the origin for the rays
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;


            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionX * rayLength, Color.red);
            if (hit) {

                //Find the angle at which the ray hits the surface
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(i == 0 && slow)
                 print(slopeAngle);

               


                //When hit.distance - skinWidth = 0, this means that the object comes to rest
                //The hit.distance - skinWidth will become 0 when the object will totally collide with the surface of the floor...
                velocity.x = (hit.distance - skinWidth) * directionX;

                //To avoid the Player from going through the object when it is in its path...
                //Very important...do not miss
                rayLength = hit.distance;

                collisions.left = (directionX == -1);
                collisions.right = directionX == 1;
            }
        }
    }


    void UpdateRaycastOrigins() {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }


    void CalculateRaySpacing() {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        //Make sure there are atleast 2 rays firing in each direction
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //Find the spacing between the rays...
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    //A struct that defines the Raycast Origins originating from the Player Collider
    struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    //Handles all the information about the collision taking place
    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public void Reset() {
            above = below = left = right = false;
        }
    }
}
