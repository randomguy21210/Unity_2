using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public void ChangeScene()
    {
        SceneManager.LoadScene(0);
    }
}
