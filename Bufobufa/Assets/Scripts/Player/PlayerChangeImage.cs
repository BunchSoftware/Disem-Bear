using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.LPlayer
{
    public class PlayerChangeImage
    {
        private Animator animator;
        private ParticleSystem playerParticleSystem;
        private Player player;

        public void Init(Animator animator, Player player)
        {
            this.animator = animator;
            this.player = player;
            playerParticleSystem = player.PlayerParticleSystem;
        }


        public void Update(Move move)
        {
            switch (move.directionMove)
            {
                case DirectionMove.State:
                    animator.SetInteger("State", 0);
                    if (playerParticleSystem.isPlaying)
                        playerParticleSystem.Stop();

                    FlipPickItem(player.PointItemForward.transform.position);

                    break;
                case DirectionMove.Right:
                    animator.SetInteger("State", 1);

                    if (!playerParticleSystem.isPlaying)
                        playerParticleSystem.Play();
                    playerParticleSystem.transform.eulerAngles = new Vector3(15, 270, 0);

                    FlipPickItem(player.PointItemRight.transform.position);

                    break;
                case DirectionMove.Left:
                    animator.SetInteger("State", 2);

                    if (!playerParticleSystem.isPlaying)
                        playerParticleSystem.Play();
                    playerParticleSystem.transform.eulerAngles = new Vector3(15, 90, 0);

                    FlipPickItem(player.PointItemLeft.transform.position);

                    break;
                case DirectionMove.Forward:
                    animator.SetInteger("State", 3);

                    if (!playerParticleSystem.isPlaying)
                        playerParticleSystem.Play();
                    playerParticleSystem.transform.eulerAngles = new Vector3(15, 180, 0);

                    FlipPickItem(player.PointItemForward.transform.position);

                    break;
                case DirectionMove.Back:
                    animator.SetInteger("State", 4);

                    if (!playerParticleSystem.isPlaying)
                        playerParticleSystem.Play();
                    playerParticleSystem.transform.eulerAngles = new Vector3(15, 0, 0);

                    FlipPickItem(player.PointItemBack.transform.position);

                    break;
            }
        }

        private void FlipPickItem(Vector3 position)
        {
            if (player.currentPickObject != null)
            {
                player.currentPickObject.transform.localEulerAngles = Vector3.zero;
                player.currentPickObject.transform.position = position;
            }
        }
    }
}
