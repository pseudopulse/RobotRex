using System;

namespace RobotRex.States {
    public class Incinerate : BaseSkillState {
        public float duration = 4f;
        public const float damageCoeffPerSecond = 9f;
        public const float tickRate = 10f;
        public float delay = 1f / tickRate;
        public GameObject signalFlare;
        public GameObject orbital;
        public LineRenderer lrSignal;
        public LineRenderer lrOrbital;
        public float stopwatch = 0f;
        public Vector3 targetPosition;
        public Vector3 signalTarget;
        public GameObject laserEndpoint;
        public static LazyAddressable<GameObject> IMPACT = new(() => Paths.GameObject.CaptainAirstrikeImpact1);
        public int ticksSinceSpawnedPool = 0;
        private float damageCoeffPerTick;

        public override void OnEnter()
        {
            base.OnEnter();
            float ticks = tickRate * base.attackSpeedStat;
            delay = 1f / ticks;

            damageCoeffPerTick = damageCoeffPerSecond * delay * (1f + base.characterBody.master.GetComponent<RRDroneStats>().BonusIncinerationDamage);

            signalFlare = GameObject.Instantiate(Survivor.LaserSignal, base.FindModelChild("MuzzleSyringe"));

            signalTarget = inputBank.GetAimRay().GetPoint(60f);

            if (Util.CharacterRaycast(base.gameObject, base.GetAimRay(), out RaycastHit hit, 4000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore)) {
                signalTarget = hit.point;
            }

            Vector3? grounded = MiscUtils.GroundPoint(signalTarget + Vector3.up * 5f);

            if (grounded.HasValue) {
                targetPosition = grounded.Value;
            }
            else {
                targetPosition = signalTarget - (Vector3.up * 90f);
            }

            orbital = GameObject.Instantiate(Survivor.OrbitalBeam, targetPosition, Quaternion.identity);

            lrSignal = signalFlare.GetComponentInChildren<LineRenderer>();
            lrOrbital = orbital.GetComponentInChildren<LineRenderer>();

            AkSoundEngine.PostEvent(Events.Play_majorConstruct_m1_laser_loop, orbital.gameObject);
            AkSoundEngine.PostEvent(Events.Play_titanboss_R_laser_loop, orbital.gameObject);

            AkSoundEngine.PostEvent(Events.Play_majorConstruct_m1_laser_chargeShoot, orbital.gameObject);
            AkSoundEngine.PostEvent(Events.Play_titanboss_R_laser_shoot, orbital.gameObject);
            AkSoundEngine.PostEvent(Events.Play_vagrant_R_explode, orbital.gameObject);

            laserEndpoint = signalFlare.transform.Find("endpoint").gameObject;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.StartAimMode(0.2f);

            stopwatch += Time.fixedDeltaTime;

            if (stopwatch >= delay) {
                stopwatch = 0f;

                FireAuthority();
            }

            signalTarget = inputBank.GetAimRay().GetPoint(60f);

            if (Util.CharacterRaycast(base.gameObject, base.GetAimRay(), out RaycastHit hit, 4000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore)) {
                signalTarget = hit.point;
            }

            Vector3? grounded = MiscUtils.GroundPoint(signalTarget + Vector3.up * 5f);

            if (grounded.HasValue) {
                targetPosition = grounded.Value;
            }
            else {
                targetPosition = signalTarget - (Vector3.up * 90f);
            }

            if (base.fixedAge >= duration) {
                outer.SetNextStateToMain();
            }
        }

        public void FireAuthority() {
            // AkSoundEngine.PostEvent(Events.Play_titanboss_R_laser_shoot, orbital.gameObject);
            ticksSinceSpawnedPool--;

            if (ticksSinceSpawnedPool <= 0) {
                // ticksSinceSpawnedPool = 2;
                // GameObject.Instantiate(Survivor.AcidProjectile, orbital.transform.position + Vector3.up, Quaternion.identity);
                // couldnt get this to work
            }

            if (!base.isAuthority) return;

            BulletAttack attack = new();

            attack.owner = base.gameObject;
            attack.origin = orbital.transform.position;
            attack.aimVector = Vector3.up;
            attack.maxDistance = 9000f;
            attack.stopperMask = LayerIndex.noCollision.mask;
            attack.damage = base.damageStat * damageCoeffPerTick;
            attack.damageType = DamageTypeCombo.GenericSpecial | DamageType.IgniteOnHit;
            attack.isCrit = base.RollCrit();
            attack.radius = 15f;

            attack.Fire();
        }   

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void OnExit()
        {
            base.OnExit();

            AkSoundEngine.PostEvent(Events.Stop_majorConstruct_m1_laser_loop, orbital.gameObject);
            AkSoundEngine.PostEvent(Events.Stop_titanboss_R_laser_loop, orbital.gameObject);

            Destroy(signalFlare);
            Destroy(orbital);
        }

        public override void Update()
        {
            base.Update();

            lrSignal.SetPosition(0, signalFlare.transform.position);
            lrOrbital.SetPosition(0, orbital.transform.position);

            lrSignal.SetPosition(1, signalTarget);
            laserEndpoint.transform.position = signalTarget;

            orbital.transform.position = Vector3.MoveTowards(orbital.transform.position, targetPosition, 120f * Time.deltaTime);
            if (orbital.transform.position.y > targetPosition.y) {
                orbital.transform.position = new(orbital.transform.position.x, targetPosition.y, orbital.transform.position.z);
            }
            orbital.transform.up = Vector3.upVector;

            lrOrbital.SetPosition(1, Vector3.up * 5000f);
        }
    }
}