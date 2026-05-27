using Fusion;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(NetworkObject))]
    public class SlidingGate : NetworkBehaviour
    {
        [Header("Visuals")]
        [Tooltip("The part of the gate that will move.")]
        [SerializeField] private Transform slidingPart;
        [Tooltip("How far down the gate should slide into the ground.")]
        [SerializeField] private float slideDistance = 3.5f;
        [Tooltip("How long the sliding animation takes.")]
        [SerializeField] private float slideDuration = 2.0f;
        [Tooltip("Will slide into ground if null")]
        [SerializeField] private Animator animator;


        [Header("Physics")]
        [Tooltip("The collider that blocks the path.")]
        [SerializeField] private Collider physicalCollider;

        [Networked]
        private NetworkBool IsOpen { get; set; }

        [Networked]
        private float OpenProgress { get; set; }

        private Vector3 _closedLocalPosition;
        private Vector3 _openLocalPosition;

        public override void Spawned()
        {
            if (animator != null) return;

            if (slidingPart != null)
            {
                _closedLocalPosition = slidingPart.localPosition;
                _openLocalPosition = _closedLocalPosition + Vector3.down * slideDistance;
            }

            // Ensure correct state if joining late
            if (IsOpen && OpenProgress >= 1f)
            {
                if (physicalCollider != null) physicalCollider.enabled = false;
                if (slidingPart != null) slidingPart.localPosition = _openLocalPosition;
            }
        }

        public void Open()
        {
            if (IsOpen) return;
            RPC_RequestOpen();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_RequestOpen()
        {
            IsOpen = true;
        }

        public override void FixedUpdateNetwork()
        {
            if (IsOpen && OpenProgress < 1f)
            {
                if (animator == null)
                {
                    OpenProgress = Mathf.MoveTowards(OpenProgress, 1f, Runner.DeltaTime / slideDuration);

                    if (OpenProgress >= 1f && physicalCollider != null)
                    {
                        physicalCollider.enabled = false;
                    }
                }
                else
                {
                    animator.SetTrigger("Open");
                   // physicalCollider.enabled = false;
                   // OpenProgress = 1f;
                }
            }
        }

        public override void Render()
        {
            if (slidingPart == null) return;
            slidingPart.localPosition = Vector3.Lerp(_closedLocalPosition, _openLocalPosition, OpenProgress);
        }
    }
}
