using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerChangeImage 
{
    private Animator animator;
    private ParticleSystem playerParticleSystem;
    private PlayerInfo playerInfo;

    public void Init(Animator animator, PlayerInfo playerInfo)
    {
        this.animator = animator;
        this.playerInfo = playerInfo;
        playerParticleSystem = playerInfo.PlayerParticleSystem;
    }


    public void Update(Move move)
    {
        playerInfo.currentPickObject.transform.localEulerAngles = Vector3.zero;

        switch (move.directionMove)
        {
            case DirectionMove.State:
                animator.SetInteger("State", 0);
                if (playerParticleSystem.isPlaying)
                    playerParticleSystem.Stop();

                FlipPickItem(playerInfo.PointItemForward.transform.position);

                break;
            case DirectionMove.Right:
                animator.SetInteger("State", 1);

                if (!playerParticleSystem.isPlaying)
                    playerParticleSystem.Play();
                playerParticleSystem.transform.eulerAngles = new Vector3(15, 270, 0);

                FlipPickItem(playerInfo.PointItemRight.transform.position);

                break;
            case DirectionMove.Left:
                animator.SetInteger("State", 2);

                if (!playerParticleSystem.isPlaying)
                    playerParticleSystem.Play();
                playerParticleSystem.transform.eulerAngles = new Vector3(15, 90, 0);

                FlipPickItem(playerInfo.PointItemLeft.transform.position);

                break;
            case DirectionMove.Forward:
                animator.SetInteger("State", 3);

                if (!playerParticleSystem.isPlaying)
                    playerParticleSystem.Play();
                playerParticleSystem.transform.eulerAngles = new Vector3(15, 180, 0);

                FlipPickItem(playerInfo.PointItemForward.transform.position);

                break;
            case DirectionMove.Back:
                animator.SetInteger("State", 4);

                if (!playerParticleSystem.isPlaying)
                    playerParticleSystem.Play();
                playerParticleSystem.transform.eulerAngles = new Vector3(15, 0, 0);

                FlipPickItem(playerInfo.PointItemBack.transform.position);

                break;
        }
    }

    private void FlipPickItem(Vector3 position)
    {
        if(playerInfo.currentPickObject != null)
            playerInfo.currentPickObject.transform.position = position;
    }
}
