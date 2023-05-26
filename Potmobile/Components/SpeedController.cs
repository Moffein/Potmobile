using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Potmobile.Components
{
    public class SpeedController : MonoBehaviour
    {
        private NetworkIdentity networkIdentity;
        public Rigidbody rigidbody;
        public CharacterBody body;
        public HoverVehicleMotor motor;
        private float baseSpeed;

        public float speedMult = 1f;

        public void Awake()
        {
            networkIdentity = base.GetComponent<NetworkIdentity>();
            motor = base.GetComponent<HoverVehicleMotor>();
            body = base.GetComponent<CharacterBody>();
            rigidbody = base.GetComponent<Rigidbody>();

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
                float calcSpeed = baseSpeed;

                float bodySpeedMult = 1f;
                if (body)
                {
                    bodySpeedMult = body.moveSpeed / body.baseMoveSpeed;
                }
                calcSpeed *= bodySpeedMult * speedMult;

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
