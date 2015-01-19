using System.Linq;
using UnityEngine;
using System.Collections;

public class PowerUpController : MonoBehaviour
{
	private GameObject[] _powerUps;

	public Vector3 holdingPosition;
	public int minPlatform = 1;
    public float powerUpSpawnInterval = 1f;
    [Range(-4f, 0)]
    public float minXPosition = -1f;
    [Range(0, 4f)]
    public float maxXPosition = 1f;

    public delegate void NewPowerUpLocation(Vector3 location, string name);
    public static event NewPowerUpLocation OnNewPowerUpLocation;

    // Use this for initialization
    IEnumerator Start()
    {
		_powerUps = GameObject.FindGameObjectsWithTag ("PowerUp");
		while (true)
        {
            yield return StartCoroutine("SpawnPowerUps");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator SpawnPowerUps()
    {
        var levelPlatforms = PlatformController.Instance.levelPlatforms;

        if (levelPlatforms == null || levelPlatforms.Count <= 0) yield return new WaitForSeconds(powerUpSpawnInterval);

        if (PlatformController.Instance.GetCurrentPlatformNumber() < minPlatform)
        {
            //Debug.Log("Hasn't reached min platform for pick-ups");
            yield return new WaitForSeconds(powerUpSpawnInterval);
        }
        else
        {
            var bottomPlatform = levelPlatforms.Keys.Min();
            var topPlatform = levelPlatforms.Keys.Max();


            //var screenLeftToWorld = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f));

            var newPosition = new Vector3(Random.Range(minXPosition, maxXPosition),
                Random.Range(levelPlatforms[bottomPlatform].transform.position.y,
                    levelPlatforms[topPlatform].transform.position.y),
                    GameController.Instance.playerZPosition);

			var randomPowerUp = _powerUps[Random.Range(0, 3)].transform;

			Debug.Log("New pick-up " + randomPowerUp + " position: " + newPosition);

            if (OnNewPowerUpLocation != null)
            {
                OnNewPowerUpLocation(newPosition, randomPowerUp.name);
            }

            yield return new WaitForSeconds(powerUpSpawnInterval);
        }

    }
}
