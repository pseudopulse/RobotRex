using System;

namespace RobotRex {
    public class RRMissileTracker : HurtboxTracker {
        public override void Start()
        {
            base.targetingIndicatorPrefab = Survivor.TargetPainter;
            base.maxSearchAngle = 45f;
            base.maxSearchDistance = 120f;
            base.targetType = TargetType.Enemy;
            base.userIndex = TeamIndex.Player;
            base.Start();
        }
    }
}