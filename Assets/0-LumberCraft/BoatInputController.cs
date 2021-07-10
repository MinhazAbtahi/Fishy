using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LumberCraft
{
    public class BoatInputController : MonoBehaviour
    {
        private PlayerController player;
        private Rigidbody rb;
        public float speed = 6f;
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
        private bool pauseInput;

        void Start()
        {
            this.player = GameManager.instance.playerCon;
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (!GameManager.instance.startGame) return;
            if (player.dead) return;
            if (pauseInput) return;
            HandlePlayerInput();

            
        }

        void FixedUpdate()
        {
            if (move)
            {
                deviation = targetDirection * speed * Time.fixedDeltaTime;
                rb.MovePosition(rb.position + deviation);
                if (targetDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    if (rb.rotation != targetRotation)
                    {
                        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing));
                    }
                }
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
                    move = true;
                    moveDirection = (mouseCurrentPos - mouseStartPos).normalized;
                    targetDirection = new Vector3(moveDirection.x, 0, moveDirection.y);
                }
                else
                {

                }

                
            }
            else if (Input.GetMouseButtonUp(0))
            {
                move = false;
            }
        }
    }
}