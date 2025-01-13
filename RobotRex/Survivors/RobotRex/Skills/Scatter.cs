using System;

namespace RobotRex.Skills {
    public class Scatter : SkillBase<Scatter>
    {
        public override string Name => "DIRECTIVE: Scatter";

        public override string Description => "Shoot a burst of inaccurate bullets for 10x70% damage. <style=cDeath>Reloads every 4 shots.".AutoFormat();

        public override Type ActivationStateType => typeof(States.Scatter);

        public override string ActivationMachineName => "Weapon";

        public override float Cooldown => 0.8f;

        public override Sprite Icon => Load<Sprite>("suppress.png");
        public override int StockToConsume => 10;
        public override int RechargeStock => 20;
        public override int MaxStock => 40;
        public override bool FullRestockOnAssign => false;
        public override bool BeginCooldownOnSkillEnd => true;
        public override InterruptPriority InterruptPriority => InterruptPriority.Any;

        public override string[] Keywords => new string[] { };
    }
}