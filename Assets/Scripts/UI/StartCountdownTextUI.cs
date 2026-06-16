using System;
using UnityEngine;

public class StartCountdownTextUI : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        animator.enabled = true;
    }
}
