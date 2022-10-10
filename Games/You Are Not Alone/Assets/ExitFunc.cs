using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitFunc : MonoBehaviour
{

    public void ExitGame()
    {
        StartCoroutine(Exit());
    }
    public IEnumerator Exit()
    {
        yield return new WaitForSeconds(1.25f);
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        yield return null;
    }
}
