using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.UI
{
    public class UIDemo : MonoBehaviour
    {

        IEnumerator LoadScene(string name)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(name);
            while (!op.isDone)
            {
                yield return null;
            }
        }
        public void ResetScene()
        {
            StartCoroutine(LoadScene("SampleScene"));
        }
        public void Exit()
        {
            Application.Quit();
        }
    }
}