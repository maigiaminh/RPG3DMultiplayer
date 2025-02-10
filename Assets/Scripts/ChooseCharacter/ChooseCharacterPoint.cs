using UnityEngine;

public class ChooseCharacterPoint : MonoBehaviour
{
    public CharacterDescript characterDescription;


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            ChooseCharacterManager.Instance.UpdateDescription(characterDescription);
            //ChooseCharacterManager.Instance.ActivateCharacterDescriptionPanel(true);
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            ChooseCharacterManager.Instance.ActivateCharacterDescriptionPanel(false);
        }
    }
}
