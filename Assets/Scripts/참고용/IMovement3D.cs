using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement3D
{
    bool IsMoving();
    bool IsGrounded();
    float GetDistanceFromGround();

    void SetMovement(in Vector3 worldMoveDirection, bool isRunning);

    bool SetJump();
    void StopMoving();
    void KnockBack(in Vector3 force, float time);
}
