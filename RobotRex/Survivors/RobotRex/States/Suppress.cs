using System;

namespace RobotRex.States {
    public class Suppress : BaseSkillState {
        public float damageCoeff = 0.9f;
        public float duration = 1f / 10f;
        public static LazyAddressable<GameObject> Tracer = new(() => Paths.GameObject.TracerCommandoBoost);
        public static LazyAddressable<GameObject> Flash = new(() => Paths.GameObject.MuzzleflashBarrage);
        public static LazyAddressable<GameObject> Impact = new(() => Paths.GameObject.ImpactNailgun);

        public override void OnEnter()
        {
            base.OnEnter();

            duration /= base.attackSpeedStat + base.characterBody.master.GetComponent<RRDroneStats>().BonusChaingunSpeed;

            base.StartAimMode(duration * 3f);

            if (base.isAuthority) {
                BulletAttack attack = new();
                attack.origin = base.inputBank.aimOrigin;
                attack.aimVector = base.inputBank.aimDirection;
                attack.damage = base.damageStat * damageCoeff;
                attack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                attack.maxDistance = 240f;
                attack.radius = 0.3f;
                attack.smartCollision = true;
                attack.minSpread = 0;
                attack.maxSpread = 2;
                attack.isCrit = base.RollCrit();
                attack.owner = base.gameObject;
                attack.muzzleName = "MuzzleTurret";
                attack.tracerEffectPrefab = Tracer;
                attack.hitEffectPrefab = Impact;
                attack.Fire();
            }

            AkSoundEngine.PostEvent(Events.Play_drone_attack, base.gameObject);
            EffectManager.SimpleMuzzleFlash(Flash, base.gameObject, "MuzzleTurret", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= duration) {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}