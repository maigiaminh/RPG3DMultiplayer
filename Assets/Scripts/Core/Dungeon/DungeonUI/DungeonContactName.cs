using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonContactName : MonoBehaviour
{
    public Image DungeonPortrait;
    public TextMeshProUGUI DungeonName;
    public Animator Anim;
    private Coroutine _coroutine;



    public void PlayDungeonContactName(Sprite portrait, string name)
    {
        DungeonPortrait.sprite = portrait;
        DungeonName.text = name;
        gameObject.SetActive(true);
    }

    public void CloseDungeonContactName()
    {
        Anim.SetTrigger("Disappear");
        if(_coroutine != null)
            StopCoroutine(_coroutine);
        if(gameObject.activeSelf)
            _coroutine = StartCoroutine(DeactiveCoroutine());
    }

    IEnumerator DeactiveCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
        _coroutine = null;
    }
}
