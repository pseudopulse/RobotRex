using System;
using Rewired.ComponentControls.Effects;

namespace RobotRex.States {
    public class PursuitMain : GenericCharacterMain {
        public float duration = 7f;
        public Animator anim;
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.characterMotor.Motor.ForceUnground(0.1f);

            if (base.fixedAge >= duration) {
                outer.SetNextStateToMain();
            }

            float dist = GetDownwardDistance();

            if (dist < 10) {
                if (base.inputBank.jump.down) {
                    base.characterMotor.velocity.y += 35f * Time.fixedDeltaTime;
                    base.characterMotor.velocity.y = Mathf.Clamp(base.characterMotor.velocity.y, -46.82f, 35f);
                }

                else if (dist < 3.5f) {
                    if (base.characterMotor.velocity.y < 0f) {
                        base.characterMotor.velocity.y = 0f;
                        return;
                    }

                    base.characterMotor.velocity.y += 15f * Time.fixedDeltaTime;
                    base.characterMotor.velocity.y = Mathf.Clamp(base.characterMotor.velocity.y, -46.82f, 15f);
                    return;
                }
            }
            else {
                base.characterMotor.velocity.y += -16.82f * Time.fixedDeltaTime;
                base.characterMotor.velocity.y = Mathf.Clamp(base.characterMotor.velocity.y, -46.82f, 10f);
            }
        }
        public override void UpdateAnimationParameters()
        {
            base.UpdateAnimationParameters();
            anim.SetBool("isGrounded", true);
            anim.SetFloat("walkSpeed", 0f);
            anim.SetBool("isMoving", false);
        }
        public override void OnEnter()
        {
            base.OnEnter();

            anim = GetModelAnimator();

            // base.characterMotor.velocity += Vector3.up * 10f;
            base.characterMotor.Motor.ForceUnground(0.1f);
            base.GetComponent<ContactDamage>().enabled = true;
            base.GetComponent<ContactDamage>().Start(); // jank but needed
            base.GetComponent<ContactDamage>().InitOverlapAttack();
            base.GetComponent<ContactDamage>().overlapAttack.hitEffectPrefab = Paths.GameObject.OmniImpactVFXSawmerang;
            base.GetComponent<ContactDamage>().overlapAttack.damageType = Util.CheckRoll(20f) ? DamageType.BleedOnHit : DamageType.Generic;
            base.characterBody.baseAcceleration = 140f;
            base.characterBody.baseMoveSpeed = 25f;
            base.characterMotor.airControl = 0.1f;

            base.characterMotor.useGravity = false;
            base.characterMotor._gravityParameters.environmentalAntiGravityGranterCount++;

            base.characterBody.statsDirty = true;

            duration += 2f * (base.characterBody.maxJumpCount - base.characterBody.baseJumpCount);

            base.skillLocator.utility.SetSkillOverride(base.gameObject, Skills.PursuitExit.instance.skillDef, GenericSkill.SkillOverridePriority.Contextual);

            ToggleSaws(true);
        }
        public override void OnExit()
        {
            base.OnExit();

            base.GetComponent<ContactDamage>().enabled = false;
            base.characterBody.baseAcceleration = 80f;
            base.characterBody.baseMoveSpeed = 7f;
            base.characterMotor.airControl = 0.25f;

            base.characterMotor.useGravity = true;
            base.characterMotor._gravityParameters.environmentalAntiGravityGranterCount--;

            base.characterBody.statsDirty = true;

            base.skillLocator.utility.UnsetSkillOverride(base.gameObject, Skills.PursuitExit.instance.skillDef, GenericSkill.SkillOverridePriority.Contextual);

            ToggleSaws(false);
        }
        public override void ProcessJump()
        {
            return;
        }
        public void ToggleSaws(bool val) {
            AkSoundEngine.PostEvent(val ? Events.Play_MULT_m1_sawblade_active_loop : Events.Stop_MULT_m1_sawblade_active_loop, base.gameObject);
            
            for (int i = 0; i < 4; i++) {
                string name = $"Saw{i+1}";
                Transform model = FindModelChild(name);
                model.GetComponent<RotateAroundAxis>().enabled = val;
                model.Find("Swirls").gameObject.SetActive(val);

                AkSoundEngine.PostEvent(val ? Events.Play_MULT_m1_sawblade_start : Events.Play_MULT_m1_sawblade_stop, model.gameObject);
            }
        }
        public float GetDownwardDistance() {
            Vector3? ground = MiscUtils.GroundPoint(base.transform.position);

            if (!ground.HasValue) {
                return float.MaxValue;
            }

            return Vector3.Distance(ground.Value, base.transform.position);
        }
    }
}