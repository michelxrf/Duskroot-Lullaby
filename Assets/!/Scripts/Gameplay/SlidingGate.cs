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

        private ChangeDetector _changeDetector;

        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            if (slidingPart != null)
            {
                _closedLocalPosition = slidingPart.localPosition;
                _openLocalPosition = _closedLocalPosition + Vector3.down * slideDistance;
            }

            // Ensure correct state if joining late
            if (IsOpen && animator != null)
            {
                // Note: Triggers are fire-and-forget. For late joiners, 
                // we just fire it again. If the animator is already in the 'Open' state
                // it should handle it or we can check the state.
                animator.SetTrigger("Open");
            }

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
                    // Logic is handled by animator visuals, but we still need to 
                    // finish the logical "opening" state.
                    OpenProgress = 1f;
                    if (physicalCollider != null) physicalCollider.enabled = false;
                }
            }
        }

        public override void Render()
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                if (change == nameof(IsOpen))
                {
                    if (IsOpen && animator != null)
                    {
                        animator.SetTrigger("Open");
                    }
                }
            }

            if (slidingPart == null || animator != null) return;
            slidingPart.localPosition = Vector3.Lerp(_closedLocalPosition, _openLocalPosition, OpenProgress);
        }
        }
        }
