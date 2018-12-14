using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;



// This class doesn't work.
// Cool idea though
namespace BeatSaberTweaks
{
    public class SceneEvents : MonoBehaviour
    {
        private bool loaded = false;

        public delegate void MyFunction();

        public void WaitThenRunFunction(MyFunction functionToRun)
        {
            StartCoroutine(WaitForLoad(functionToRun));
        }

        private IEnumerator WaitForLoad(MyFunction functionToRun)
        {
            while (!loaded)
            {
                var resultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();

                if (resultsViewController == null)
                {
                    Plugin.Log("resultsViewController is null!", Plugin.LogLevel.DebugOnly);
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    Plugin.Log("Loaded the energy bar mover", Plugin.LogLevel.DebugOnly);
                    loaded = true;
                }
            }
            functionToRun();
        }
    }
}