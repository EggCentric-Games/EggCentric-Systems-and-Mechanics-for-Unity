using EggCentric.LifeCycleHandling;
using UnityEngine;

namespace EggCentric.Stabbing
{
    public class StabbingSystemSetup : MonoBehaviour
    {
        private void Awake() => SetupStabbingSystem();

        private void SetupStabbingSystem()
        {
            LifeCycleProvider lifeCycleProvider = PrepareLifecycleProvider();
            new StabbingSystem(lifeCycleProvider);
        }

        private LifeCycleProvider PrepareLifecycleProvider()
        {
            if (LifeCycleProvider.Instance == null)
                gameObject.AddComponent<LifeCycleProvider>();

            return LifeCycleProvider.Instance;
        }
    }
}