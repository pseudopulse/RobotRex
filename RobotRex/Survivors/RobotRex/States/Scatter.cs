using System;

namespace RobotRex.States {
    public class Scatter : BaseSkillState {
        public float damageCoeff = 0.6f;
        public float duration = 0.5f;
        public float count = 10;
        public static LazyAddressable<GameObject> Tracer = new(() => Paths.GameObject.TracerCommandoBoost);
        public static LazyAddressable<GameObject> Flash = new(() => Paths.GameObject.MuzzleflashBarrage);
        public static LazyAddressable<GameObject> Impact = new(() => Paths.GameObject.ImpactNailgun);

        public override void OnEnter()
        {
            base.OnEnter();

            duration /= base.attackSpeedStat;
            count *= 1f + base.characterBody.master.GetComponent<RRDroneStats>().BonusChaingunSpeed;

            base.StartAimMode(duration * 3f);

            if (base.isAuthority) {
                bool crit = base.RollCrit();
                for (int i = 0; i < count; i++) {
                    BulletAttack attack = new();
                    attack.origin = base.inputBank.aimOrigin;
                    attack.aimVector = base.inputBank.aimDirection;
                    attack.damage = base.damageStat * damageCoeff;
                    attack.falloffModel = BulletAttack.FalloffModel.Buckshot;
                    attack.maxDistance = 90f;
                    attack.radius = 0.3f;
                    attack.smartCollision = true;
                    attack.minSpread = 0;
                    attack.maxSpread = 8;
                    attack.isCrit = crit;
                    attack.owner = base.gameObject;
                    attack.muzzleName = "MuzzleTurret";
                    attack.tracerEffectPrefab = Tracer;
                    attack.hitEffectPrefab = Impact;
                    attack.Fire();
                }
            }

            AkSoundEngine.PostEvent(Events.Play_captain_m1_shotgun_shootTight, base.gameObject);
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