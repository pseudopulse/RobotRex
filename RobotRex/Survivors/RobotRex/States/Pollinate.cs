using System;

namespace RobotRex.States {
    public class Pollinate : BaseSkillState {
        public float damageCoeff = 2f;
        public float duration = 0.8f;
        public static LazyAddressable<GameObject> Flash = new(() => Paths.GameObject.MuzzleflashBarrage);

        public override void OnEnter()
        {
            base.OnEnter();

            duration /= base.attackSpeedStat;

            base.StartAimMode(duration * 3f);

            if (base.isAuthority) {
                var info = MiscUtils.GetProjectile(Survivor.PollinateProjectile, damageCoeff, base.characterBody, DamageTypeCombo.GenericSecondary | DamageType.Stun1s);
                ProjectileManager.instance.FireProjectile(info);
            }

            AkSoundEngine.PostEvent(Events.Play_MULT_m1_grenade_launcher_shoot, base.gameObject);
            EffectManager.SimpleMuzzleFlash(Flash, base.gameObject, "MuzzleSyringe", false);
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