using System;

namespace RobotRex {
    public class RRBeeUpdater : MonoBehaviour {
        public void Start() {
            var impact = GetComponent<ProjectileImpactExplosion>();
            var projectileController = GetComponent<ProjectileController>();

            if (projectileController.owner) {
                CharacterBody body = projectileController.owner.GetComponent<CharacterBody>();

                if (body && body.master) {
                    RRDroneStats stats = body.master.GetComponent<RRDroneStats>();

                    if (stats) {
                        impact.childrenCount += stats.BonusMissiles;
                    }
                }
            }
        }
    }
}