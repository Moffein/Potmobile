using RoR2;
using UnityEngine;

namespace Potmobile.Components
{
    public class SpeedController : MonoBehaviour
    {
        private CharacterBody body;
        private HoverVehicleMotor motor;
        private float baseSpeed;

        public float speedMult = 1f;

        public void Awake()
        {
            motor = base.GetComponent<HoverVehicleMotor>();
            body = base.GetComponent<CharacterBody>();

            if (motor)
            {
                baseSpeed = motor.motorForce;
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
    }
}
