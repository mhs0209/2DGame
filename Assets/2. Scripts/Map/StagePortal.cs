using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePortal : MonoBehaviour
{
    private string stageName;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(stageName);
        }
    }

    public void NextStage(string stagePortalName)
    {
        stageName = stagePortalName;
    }
}