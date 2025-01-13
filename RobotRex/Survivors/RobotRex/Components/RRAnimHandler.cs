using System;

namespace RobotRex {
    public class RRAnimHandler : MonoBehaviour {
        public Animator Turret;
        private Animator us;

        public void Start() {
            us = GetComponent<Animator>();
        }

        public void Update() {
            us.SetBool("isSprinting", false);
            Turret.SetFloat("aimPitchCycle", us.GetFloat("aimPitchCycle") * 0.35f);
        }
    }
}