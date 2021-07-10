using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LumberCraft
{
    public class PlayerInputController : MonoBehaviour
    {
        private PlayerController player;
        public float speed = 6f;
        public float walkSpeed = 3f;
        public float runSpeed = 8f;
        [Range(0f, 1f)]
        public float movemenetSmoothing = .5f;
        [Range(0f, 1f)]
        public float rotationSmoothing = .25f;
        private Vector3 mouseCurrentPos;
        private Vector3 mouseStartPos;
        private Vector3 moveDirection;
        private Vector3 targetDirection;
        private Vector3 deviation;
        private float currentDragDistance;
        public float maxDragDistance = 10f;
        public bool move;

        void Start()
        {
            this.player = GetComponent<PlayerController>();
        }

        void Update()
        {
            if (!GameManager.instance.startGame) return;
            if (player.dead) return;

            HandlePlayerInput();
        }

        void FixedUpdate()
        {
            if (move)
            {
                deviation = targetDirection * speed * Time.fixedDeltaTime;
                player.rb.MovePosition(player.rb.position + deviation);
                //if (!player.enemyContact)
                //{
                    if (targetDirection != Vector3.zero)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                        if (transform.rotation != targetRotation)
                        {
                            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing);
                            //player.rb.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing);
                            player.rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing));
                        }
                    }
                //}
            }
        }

        private void HandlePlayerInput()
        {
            mouseCurrentPos = Input.mousePosition;
            if (Input.GetMouseButtonDown(0))
            {
                mouseStartPos = mouseCurrentPos;
                if (!UIManager.instance.howtoPlayTapped)
                {
                    GameManager.instance.startGame = true;
                    UIManager.instance.tutorial.SetActive(false);
                    UIManager.instance.howtoPlayTapped = true;
                }
            }
            else if (Input.GetMouseButton(0) && !player.dead)
            {
                currentDragDistance = (mouseCurrentPos - mouseStartPos).magnitude;

                if (currentDragDistance > maxDragDistance)
                {
                    //mouseStartPos = mouseCurrentPos - moveDirection * maxDragDistance;
                    speed = runSpeed;
                    player.anim.SetBool("walk", false);
                    player.anim.SetBool("run", true);
                    player.dirtTrailFX.SetActive(true);
                    player.footstepAudioThresold = 0.3f;
                }
                else
                {
                    speed = walkSpeed;
                    player.anim.SetBool("run", false);
                    player.anim.SetBool("walk", true);

                    player.footstepAudioThresold = 0.7f;
                }

                move = true;
                moveDirection = (mouseCurrentPos - mouseStartPos).normalized;
                targetDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
                //transform.position += deviation;          
                //player.anim.SetBool("walk", true);
                //player.SimulateFootsteps();
                player.footstepDelay += Time.deltaTime;
                if (player.footstepDelay >= player.footstepAudioThresold)
                {
                    SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.footstepSFX);
                    FPG.HapticController.PlayWalkHaptic(SoundManager.sharedInstance.footstepSFX.length);
                    player.footstepDelay = 0;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                move = false;
                player.anim.SetBool("walk", false);
                player.anim.SetBool("run", false);
                player.dirtTrailFX.SetActive(false);
            }
        }
    }
}