using UnityEngine;

namespace Assets.Scripts.CustomBehaviour
{
    public class Wheel : MonoBehaviour
    {
        public enum WheelType { FRONT_L, FRONT_R, REAR_L, REAR_R }

        [HideInInspector] public float steerAngle;
        [HideInInspector] public float motorPower;
        [HideInInspector] public float steeringPower;
        [HideInInspector] public float throttle;
        [SerializeField] private float springTravel;
        [SerializeField] private float springStiffness;
        [SerializeField] private float damperStiffness;
        [Header("Wheel")]
        [SerializeField] private float wheelRadius;
        [SerializeField] private WheelType type;
        [SerializeField] private float steerTime;
        [Header("Suspension")]
        [SerializeField] private float restLength;
        private new Rigidbody rigidbody;
        private float minLength;
        private float maxLength;
        private float lastLength;
        private float springLength;
        private float springForce;
        private float damperForce;
        private float springVelocity;
        private float wheelAngle = 0;
        private Vector3 suspensionForce;
        private Vector3 wheelVelocity;
        private float forwardForce;
        private float rightForce;
        private Vector3 applicationOffset;

        public WheelType Type => type;

        private void Start()
        {
            rigidbody = transform.root.GetComponent<Rigidbody>();
            minLength = restLength - springTravel;
            maxLength = restLength + springTravel;
            applicationOffset = new Vector3(0F, 0.8F * wheelRadius, 0F);
        }

        private void Update()
        {
            wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, steerTime * Time.deltaTime);
            transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);

            Debug.DrawRay(transform.position, -transform.up * (springLength + wheelRadius), Color.green);
        }

        private void FixedUpdate()
        {
            if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius, LayerMask.GetMask("Floor")))
            {
                lastLength = springLength;
                springLength = hit.distance - wheelRadius;
                springLength = Mathf.Clamp(springLength, minLength, maxLength);
                springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
                springForce = springStiffness * (restLength - springLength);
                damperForce = damperStiffness * springVelocity;

                wheelVelocity = transform.InverseTransformDirection(rigidbody.GetPointVelocity(hit.point));

                forwardForce = throttle * motorPower;
                rightForce = wheelVelocity.x * steeringPower;

                suspensionForce = (springForce + damperForce) * transform.up;

                rigidbody.AddForceAtPosition(suspensionForce + (forwardForce * transform.forward) + (rightForce * -transform.right), hit.point + applicationOffset);
            }
        }
    }
}