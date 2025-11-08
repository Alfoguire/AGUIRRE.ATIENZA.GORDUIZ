using UnityEngine;
using TMPro; 
using System.Collections;

public class HintManager : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public float displayTime = 3f;

    public static HintManager instance;

    private Coroutine hideCoroutine;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if(hintText != null)
            hintText.text = "";
    }
    
    public void ShowHint(string message)
    {
        if (hintText == null) return;

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hintText.text = message;

        hideCoroutine = StartCoroutine(HideHintAfterDelay());
    }

    private IEnumerator HideHintAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);

        hintText.text = "";
        hideCoroutine = null;
    }
}