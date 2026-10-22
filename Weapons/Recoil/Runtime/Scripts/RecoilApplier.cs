using System.Collections;
using UnityEngine;

namespace EggCentric.Weapons
{
    public class RecoilApplier : RecoilProvider
    {
        [SerializeField] private Vector3 _recoilApplicationPoint;
        [SerializeField] private Vector2 _minMaxLongitudinalRecoilForce;
        [SerializeField] private Vector2 _minMaxLateralRecoilForce;
        [SerializeField] private Vector2 _minMaxRecoilAngle;
        [SerializeField] private DistributionParameters _distributionParameters;

        private DistributedValue _distributedValue;

        public override void ApplyRecoil()
        {
            Vector3 recoil = GetRecoil();
            StartCoroutine(ApplyContiniousForce(_distributedValue, recoil));
        }

        private void Awake() => _distributedValue = new DistributedValue(_distributionParameters);

        private Vector3 GetRecoil()
        {
            float angle = GetRecoilAngle();
            float longitudinalForce = GetLongitudinalForce();
            float lateralForce = GetLateralRecoilForce();

            Vector3 lateralRecoilDirection = Quaternion.AngleAxis(angle, Vector3.forward) * _recoilBody.transform.up;
            Vector3 lateralRecoil = lateralRecoilDirection * lateralForce;
            Vector3 longitudinalRecoil = _recoilBody.transform.forward * longitudinalForce * -1f;

            return lateralRecoil + longitudinalRecoil;
        }

        private float GetLongitudinalForce() => Random.Range(_minMaxLongitudinalRecoilForce.x, _minMaxLongitudinalRecoilForce.y);
        private float GetLateralRecoilForce() => Random.Range(_minMaxLateralRecoilForce.x, _minMaxLateralRecoilForce.y);
        private float GetRecoilAngle() => Random.Range(_minMaxRecoilAngle.x, _minMaxRecoilAngle.y);

        private IEnumerator ApplyContiniousForce(DistributedValue distributedValue, Vector3 recoil)
        {
            var elapsedTime = 0f;

            while (elapsedTime < distributedValue.DistributionTime)
            {
                var timeStep = Time.fixedDeltaTime;
                var value = distributedValue.GetDistributedValue(elapsedTime, timeStep);
                _recoilBody.AddForceAtPosition(recoil * value, _recoilBody.transform.TransformPoint(_recoilApplicationPoint));

                elapsedTime += timeStep;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}