using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordsMan : StickManEnemy
{
   // [SerializeField] private Rigidbody rigidbody;
    [SerializeField] float runningSpeed;

    private void FixedUpdate()
    {
        if (isAlive)
        {
            if (lookAtPlayer)
            {
                Vector3 playerPosition = GameManager.playerPosition;
                Vector3 myDirection = playerPosition - myTransform.position;
                myDirection.y = 0;
                myTransform.rotation = Quaternion.LookRotation(myDirection);
            }
            if (isAwake)
            {
                Vector3 newPosition = myTransform.position +
                       (myTransform.forward * runningSpeed * Time.fixedDeltaTime);
                myTransform.position = newPosition;
            }

          //  rigidbody.MovePosition(newPosition);
        }

    }

    public override void Awaken()
    {
        base.Awaken();
        animator.SetBool("IsRunning", true);
    }


}
