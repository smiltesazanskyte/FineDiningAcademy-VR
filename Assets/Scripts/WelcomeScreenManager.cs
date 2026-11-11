using UnityEngine;

public class WelcomeScreenManager : MonoBehaviour
{
    public GameObject welcomeCanvas;

    public void StartTraining()
    {
        welcomeCanvas.SetActive(false);
    }
}