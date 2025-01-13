using System;
using System.Collections;

namespace RobotRex.States {
    public class Intercept : BaseSkillState {
        public float damageCoeff = 4.5f;
        public float duration = 0.8f;
        public static LazyAddressable<GameObject> Projectile = new(() => Paths.GameObject.MissileProjectile);
        public static LazyAddressable<GameObject> Flash = new(() => Paths.GameObject.MuzzleflashMageFireLarge);
        public RRMissileTracker tracker;

        public override void OnEnter()
        {
            base.OnEnter();
            tracker = GetComponent<RRMissileTracker>();

            duration /= base.attackSpeedStat;

            int missiles = 2 + (base.characterBody.master.GetComponent<RRDroneStats>().BonusMissiles);

            for (int i = 0; i < missiles; i++) {
                string name = i % 2 == 0 ? "MuzzleLMissile" : "MuzzleRMissile";
                int h = i + 1;
                
                if (h > 2) {
                    h -= 2;
                    base.characterBody.StartCoroutine(SpawnAfterDelay((duration / missiles) * h, name));
                }
                else {
                    FireMissile(name);
                }
            }
        }

        public IEnumerator SpawnAfterDelay(float delay, string name) {
            yield return new WaitForSeconds(delay);
            FireMissile(name);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= duration) {
                outer.SetNextStateToMain();
            }
        }

        public void FireMissile(string muzzle) {
            EffectManager.SimpleMuzzleFlash(Flash, base.gameObject, muzzle, false);
            AkSoundEngine.PostEvent(Events.Play_item_proc_missile_fire, base.gameObject);

            if (base.isAuthority) {
                FireProjectileInfo info = MiscUtils.GetProjectile(Projectile, damageCoeff, base.characterBody, DamageTypeCombo.GenericSecondary | DamageType.Stun1s);
                if (tracker.target) {
                    info.target = tracker.target.gameObject;
                }
                info.rotation = Quaternion.LookRotation(FindModelChild(muzzle).forward);
                info.speedOverride = 220f;
                info.position = FindModelChild(muzzle).transform.position;
                ProjectileManager.instance.FireProjectile(info);
            }
        }
    }
}