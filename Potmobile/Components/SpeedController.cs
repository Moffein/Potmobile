using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Potmobile.Components
{
    public class SpeedController : MonoBehaviour
    {
        public static bool allowReverse = true;

        public float reverseSpeedCoefficient = 0.5f;
        private NetworkIdentity networkIdentity;
        public Rigidbody rigidbody;
        public CharacterBody body;
        public HoverVehicleMotor motor;
        private InputBankTest inputBank;
        private float baseSpeed;

        public float speedMult = 1f;

        public void Awake()
        {
            networkIdentity = base.GetComponent<NetworkIdentity>();
            motor = base.GetComponent<HoverVehicleMotor>();
            body = base.GetComponent<CharacterBody>();
            rigidbody = base.GetComponent<Rigidbody>();
            inputBank = base.GetComponent<InputBankTest>();

            if (motor)
            {
                baseSpeed = motor.motorForce;
            }
        }

        //Apply upwards force on spawn to reduce chances of falling through the floor
        public void Start()
        {
            if (rigidbody)
            {
                Vector3 newVelocity = rigidbody.velocity;
                if (newVelocity.y < 0f) newVelocity.y = 0f;
                newVelocity.y += 32f;
                rigidbody.velocity = newVelocity;
            }
        }

        public void FixedUpdate()
        {
            if (motor)
            {
                bool reverse = false;
                if (allowReverse && inputBank)
                {
                    Ray aimRay = inputBank.GetAimRay();
                    Vector2 moveDirectionFlat = new Vector2(inputBank.moveVector.x, inputBank.moveVector.z);
                    Vector2 forwardDirectionFlat = new Vector2(aimRay.direction.x, aimRay.direction.z);

                    float angle = Vector2.Angle(moveDirectionFlat, forwardDirectionFlat);
                    if (angle > 120f)
                    {
                        reverse = true;
                    }
                }

                float calcSpeed = baseSpeed;

                float bodySpeedMult = 1f;
                if (body)
                {
                    bodySpeedMult = body.moveSpeed / body.baseMoveSpeed;
                }
                calcSpeed *= bodySpeedMult * speedMult;

                if (reverse)
                {
                    calcSpeed = Mathf.Abs(calcSpeed);
                    calcSpeed *= reverseSpeedCoefficient * -1f;
                }
                motor.motorForce = calcSpeed;
            }
        }

        //Copied from RoR2.Util to skip the getcomponent check
        public bool HasEffectiveAuthority()
        {
            return networkIdentity && (networkIdentity.hasAuthority || (NetworkServer.active && networkIdentity.clientAuthorityOwner == null));
        }
    }
}
