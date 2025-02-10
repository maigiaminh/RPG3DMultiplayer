using UnityEngine;

public class ChooseOutfitPlace : MonoBehaviour
{
    public GameObject ChooseOutfitCanvas;
    public GameObject Model;
    public void EnableCanvas()
    {
        ChooseOutfitCanvas.SetActive(true);
    }

    public void ChangeModelAnimState()
    {
        Model.GetComponent<Animator>().SetTrigger("Idle");
    }
}
