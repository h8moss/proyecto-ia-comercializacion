using System.Collections;
using UnityEngine;

public class StartGameUIAdapter : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ShowStartGame());
    }

    void Update()
    {
        
    }

    IEnumerator ShowStartGame()
    {
        var endgameCanvas = GetComponent<EndGameUI>();
        endgameCanvas.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        endgameCanvas.GetComponent<EndGameUI>().fadeDuration = 10f;
        endgameCanvas.GetComponent<EndGameUI>().StartFade();
        yield return new WaitForSeconds(12);
        endgameCanvas.gameObject.SetActive(false);
    }
}
