using System;

namespace RobotRex.Skills {
    public class Suppress : SkillBase<Suppress>
    {
        public override string Name => "DIRECTIVE: Suppress";

        public override string Description => "Barrage a target for <style=cIsDamage>90% damage</style> very rapidly. <style=cDeath>Reloads every 40 shots.</style>".AutoFormat();

        public override Type ActivationStateType => typeof(States.Suppress);

        public override string ActivationMachineName => "Weapon";

        public override float Cooldown => 2.5f;

        public override Sprite Icon => Load<Sprite>("suppress.png");
        public override int StockToConsume => 1;
        public override int RechargeStock => 40;
        public override int MaxStock => 40;
        public override bool FullRestockOnAssign => false;
        public override bool BeginCooldownOnSkillEnd => true;
        public override InterruptPriority InterruptPriority => InterruptPriority.Any;

        public override string[] Keywords => new string[] { };
    }
}