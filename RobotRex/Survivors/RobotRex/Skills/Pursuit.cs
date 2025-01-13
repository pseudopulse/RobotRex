using System;

namespace RobotRex.Skills {
    public class Pursuit : SkillBase<Pursuit>
    {
        public override string Name => "DIRECTIVE: Pursuit";

        public override string Description => "Take flight, increasing agility and shredding targets nearby for <style=cIsDamage>350% damage per second</style>.".AutoFormat();

        public override Type ActivationStateType => typeof(States.PursuitMain);

        public override string ActivationMachineName => "Body";

        public override float Cooldown => 15f;

        public override Sprite Icon => Load<Sprite>("pursuit.png");
        public override int StockToConsume => 1;
        public override int MaxStock => 1;
        public override bool FullRestockOnAssign => false;
        public override InterruptPriority InterruptPriority => InterruptPriority.PrioritySkill;

        public override string[] Keywords => new string[] { };
    }
}