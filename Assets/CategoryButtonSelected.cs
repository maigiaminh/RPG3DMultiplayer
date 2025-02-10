using UnityEngine;

public class CategoryButtonSelected : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void CategorySelected(){
        animator.Play("CategorySelected");
    }
}
