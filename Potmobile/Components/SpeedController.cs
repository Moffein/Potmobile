using R2API;
using RoR2;
using System;
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
        private ModelLocator modelLocator;
        private float baseSpeed;
        public Vector3 ramHitboxSize = Vector3.one;

        private GameObject hitboxObject;
        private HitBoxGroup hitBoxGroup;
        private OverlapAttack overlapAttack;
        public float speedMult = 1f;

        public float minOverlapDamageCoefficient = 3f;
        public float minOverlapSpeed = 20f;
        public float doubleDamageOverlapSpeed = 40f;

        public Vector3 overlapForce = 2500f * Vector3.down;
        public float overlapResetInterval = 1f;
        private float overlapResetStopwatch;

        public void Awake()
        {
            networkIdentity = base.GetComponent<NetworkIdentity>();
            motor = base.GetComponent<HoverVehicleMotor>();
            body = base.GetComponent<CharacterBody>();
            rigidbody = base.GetComponent<Rigidbody>();
            inputBank = base.GetComponent<InputBankTest>();
            modelLocator = base.GetComponent<ModelLocator>();

            if (motor)
            {
                baseSpeed = motor.motorForce;
            }

            if (modelLocator)
            {
                BuildHitbox();
                InitOverlapAttack();
            }
        }

        //SUPER CURSED, should do this in Unity if possible (not an option here)
        private void BuildHitbox()
        {
            if (modelLocator.modelTransform)
            {
                GameObject ramHitbox = new GameObject();
                hitboxObject = ramHitbox;
                HurtBoxGroup hbg = modelLocator.modelTransform.gameObject.GetComponent<HurtBoxGroup>();
                if (hbg && hbg.mainHurtBox)
                {
                    BoxCollider bc = hbg.mainHurtBox.GetComponent<BoxCollider>();
                    if (bc)
                    {
                        ramHitbox.transform.parent = base.transform;
                        ramHitbox.transform.localScale = ramHitboxSize;
                        ramHitbox.transform.localPosition = bc.center;
                        ramHitbox.transform.localRotation = bc.transform.localRotation;
                        ramHitbox.name = "RamHitbox";
                        Potmobile.SetupHitbox(base.gameObject, "RamHitbox", new Transform[] { ramHitbox.transform });
                    }
                }
                hitBoxGroup = base.GetComponent<HitBoxGroup>();
            }
        }

        private void InitOverlapAttack()
        {
            if (!hitBoxGroup) return;
            overlapResetStopwatch = 0f;

            overlapAttack = new OverlapAttack();
            overlapAttack.damageType = DamageType.Generic;
            overlapAttack.attacker = base.gameObject;
            overlapAttack.inflictor = base.gameObject;
            overlapAttack.teamIndex = (body && body.teamComponent) ? body.teamComponent.teamIndex : TeamIndex.None;
            overlapAttack.damage = 0f;
            overlapAttack.forceVector = overlapForce;
            overlapAttack.pushAwayForce = 0f;
            overlapAttack.hitBoxGroup = hitBoxGroup;
            overlapAttack.isCrit = body ? body.RollCrit(): false;
            overlapAttack.AddModdedDamageType(PotmobileContent.ModdedDamageTypes.SquashOnKill);
            overlapAttack.AddModdedDamageType(PotmobileContent.ModdedDamageTypes.PotmobileRam);
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

            if (HasEffectiveAuthority())
            {
                if (body)
                {
                    body.isSprinting = speedMult > 1f;
                }
                ProcessOverlapAttack();
            }
        }

        public void ProcessOverlapAttack()
        {
            bool hitEnemy = false;
            bool isPlayer = body && (body.isPlayerControlled || (body.teamComponent && body.teamComponent.teamIndex == TeamIndex.Player));
            if (!Potmobile.ramEnabled || !hitBoxGroup || (!isPlayer && Potmobile.ramDisableOnEnemies)) return;
            overlapResetStopwatch += Time.fixedDeltaTime;
            if (overlapResetStopwatch >= overlapResetInterval)
            {
                overlapResetStopwatch -= overlapResetInterval;
                overlapAttack.ResetIgnoredHealthComponents();
                overlapAttack.isCrit = body ? body.RollCrit() : false;
            }

            float currentSpeed = 0f;
            if (rigidbody)
            {
                currentSpeed = rigidbody.velocity.magnitude;
            }

            if (currentSpeed >= minOverlapSpeed)
            {
                //InverseLerp is clamped, so calc manually
                float invLerp = (currentSpeed - minOverlapSpeed) / (doubleDamageOverlapSpeed - minOverlapSpeed);

                float damageCoefficient = 1f + invLerp;

                overlapAttack.damage = damageCoefficient * minOverlapDamageCoefficient * (body ? body.damage : 1f);

                hitEnemy = overlapAttack.Fire();
            }
            if (hitEnemy)
            {
                Util.PlaySound("Play_MULT_shift_hit", base.gameObject);
            }
        }

        //Copied from RoR2.Util to skip the getcomponent check
        public bool HasEffectiveAuthority()
        {
            return networkIdentity && (networkIdentity.hasAuthority || (NetworkServer.active && networkIdentity.clientAuthorityOwner == null));
        }


        private void OnDestroy()
        {
            if (hitboxObject)
            {
                Destroy(hitboxObject);
            }
        }

        public void Update()
        {
            if (HasEffectiveAuthority() && Potmobile.GetKeyPressed(Potmobile.honkButton))
            {

            }
        }
    }
}
