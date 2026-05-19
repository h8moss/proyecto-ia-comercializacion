using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField] private float returnToMenuAfter;

    void Start()
    {
        StartCoroutine(LeaveRoutine());
    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            SceneManager.LoadScene(0);
        }
    }

    IEnumerator LeaveRoutine()
    {
        yield return new WaitForSeconds(returnToMenuAfter);

        SceneManager.LoadScene(0);
    }
}
