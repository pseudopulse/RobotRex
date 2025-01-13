using System;

namespace RobotRex.Skills {
    public class PursuitExit : SkillBase<PursuitExit>
    {
        public override string Name => "DIRECTIVE: Pursuit";

        public override string Description => "Exit flight.".AutoFormat();

        public override Type ActivationStateType => typeof(GenericCharacterMain);

        public override string ActivationMachineName => "Body";

        public override float Cooldown => 2f;

        public override Sprite Icon => Load<Sprite>("pursuit.png");
        public override int StockToConsume => 1;
        public override int MaxStock => 1;
        public override bool FullRestockOnAssign => false;
        public override InterruptPriority InterruptPriority => InterruptPriority.PrioritySkill;

        public override string[] Keywords => new string[] { };
    }
}