using UnityEngine;
using System.Collections;

public class EveryplayEarlyInitializer : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(InitializeEveryplay());
    }

    IEnumerator InitializeEveryplay() {
        yield return 0;
        Everyplay.Initialize();
        Destroy(gameObject);
    }
}
