using System.Collections;
using UnityEngine;

namespace Bee
{
    public class BeehiveSpawner : MonoBehaviour
    {
        [SerializeField] private ProjectContext projectContext;

        [SerializeField] private BeeView beePrefab;
        [SerializeField] private int totalBees = 5;
        [SerializeField] private float spawnDelay = 0.25f;

        private Coroutine _spawner;
        private Transform _transform;

        private void Awake()
        {
            projectContext.GameStarted += ActivateSpawner;
            _transform = transform;
        }

        private void ActivateSpawner()
        {
            if (_spawner != null)
            {
                StopCoroutine(_spawner);
            }
            _spawner = StartCoroutine(Spawner());
        }

        private IEnumerator Spawner()
        {
            for (int i = 0; i < totalBees; i++)
            {
                BeeView bee = Instantiate(beePrefab, _transform.position, Quaternion.identity);
                bee.transform.SetParent(_transform);
                bee.SetTarget(projectContext.GetBeesTarget());
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
}