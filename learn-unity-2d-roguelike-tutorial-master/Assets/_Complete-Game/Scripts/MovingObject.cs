using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace Scripts
{
    //The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
    public abstract class MovingObject : MonoBehaviour
    {
        private BoxCollider2D _boxCollider; //The BoxCollider2D component attached to this object.
        private float _inverseMoveTime; //Used to make movement more efficient.
        private Rigidbody2D _rb2D; //The Rigidbody2D component attached to this object.
        public LayerMask blockingLayer; //Layer on which collision will be checked.
        public float moveTime = 0.1f; //Time it will take object to move, in seconds.


        //Protected, virtual functions can be overridden by inheriting classes.
        protected virtual void Start()
        {
            //Get a component reference to this object's BoxCollider2D
            _boxCollider = GetComponent<BoxCollider2D>();

            //Get a component reference to this object's Rigidbody2D
            _rb2D = GetComponent<Rigidbody2D>();

            //By storing the reciprocal of the move time we can use it by multiplying instead of dividing, this is more efficient.
            _inverseMoveTime = 1f / moveTime;
        }


        //Move returns true if it is able to move and false if not. 
        //Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
        [CanBeNull]
        protected Transform Move(int xDir, int yDir)
        {
            //Store start position to move from, based on objects current transform position.
            Vector2 start = transform.position;

            // Calculate end position based on the direction parameters passed in when calling Move.
            var end = start + new Vector2(xDir, yDir);

            //Disable the boxCollider so that linecast doesn't hit this object's own collider.
            _boxCollider.enabled = false;

            //Cast a line from start point to end point checking collision on blockingLayer.
            var hit = Physics2D.Linecast(start, end, blockingLayer);

            //Re-enable boxCollider after linecast
            _boxCollider.enabled = true;

            // return the object that hit, maybe null.
            return hit.transform;
        }


        //Co-routine for moving units from one space to next, takes a parameter end to specify where to move to.
        private IEnumerator SmoothMovement(Vector3 end)
        {
            //Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
            //Square magnitude is used instead of magnitude because it's computationally cheaper.
            var sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            //While that distance is greater than a very small amount (Epsilon, almost zero):
            while (sqrRemainingDistance > float.Epsilon)
            {
                //Find a new position proportionally closer to the end, based on the moveTime
                var newPosition = Vector3.MoveTowards(_rb2D.position, end, _inverseMoveTime * Time.deltaTime);

                //Call MovePosition on attached Rigidbody2D and move it to the calculated position.
                _rb2D.MovePosition(newPosition);

                //Recalculate the remaining distance after moving.
                sqrRemainingDistance = (transform.position - end).sqrMagnitude;

                //Return and loop until sqrRemainingDistance is close enough to zero to end the function
                yield return null;
            }

            OnSmoothMoveFinish();
        }

        /// <summary>
        ///     平滑移动结束回调
        /// </summary>
        protected virtual void OnSmoothMoveFinish()
        {
        }


        //The virtual keyword means AttemptMove can be overridden by inheriting classes using the override keyword.
        //AttemptMove takes a generic parameter T to specify the type of component we expect our unit to interact with if blocked (Player for Enemies, Wall for Player).
        protected void AttemptMove(int xDir, int yDir)
        {
            //Hit will store whatever our linecast hits when Move is called.

            //Set canMove to true if Move was successful, false if failed.
            var hitTransform = Move(xDir, yDir);

            //Check if nothing was hit by linecast
            if (hitTransform == null)
            {
                //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
                Vector2 start = transform.position;
                StartCoroutine(SmoothMovement(start + new Vector2(xDir, yDir)));
                return;
            }

            //Get a component reference to the component of type MonoBehaviour attached to the object that was hit
            //Cannot use type Component, for it will get the transform component
            var hitComponent = hitTransform.GetComponent<MonoBehaviour>();

            //If hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
            if (hitComponent == null) return;

            print($"{name} hit {hitComponent.name}");
            //Call the OnCantMove function and pass it hitComponent as a parameter.
            OnCantMove(hitComponent);
        }


        //The abstract modifier indicates that the thing being modified has a missing or incomplete implementation.
        //OnCantMove will be overriden by functions in the inheriting classes.
        protected abstract void OnCantMove([NotNull] MonoBehaviour component);
    }
}