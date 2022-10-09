using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public FadeScreen fadeScreen;
    public void GoToScene(int sceneIndex)
    {
        StartCoroutine(GoToSceneRoutine(sceneIndex));
    }

    public IEnumerator GoToSceneRoutine(int sceneIndex)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        SceneManager.LoadScene(sceneIndex);
    }

    public void GoToSceneAsync(int sceneIndex)
    {
        StartCoroutine(GoToSceneAsyncRoutine(sceneIndex));
    }

    public IEnumerator GoToSceneAsyncRoutine(int sceneIndex)
    {
        fadeScreen.FadeOut();

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        operation.allowSceneActivation = false;

        float timer = 0;
        while (!operation.isDone) //timer <= fadeScreen.fadeDuration &&
        {
            timer += Time.deltaTime;
            yield return null;
        }
        operation.allowSceneActivation = true;
    }

    public void QuitGame() {
        Application.Quit();
    }

}
