using System;

namespace RobotRex.Skills {
    public class Incinerate : SkillBase<Incinerate>
    {
        public override string Name => "DIRECTIVE: Incinerate";

        public override string Description => "<style=cIsDamage>Ignite.</style> Control an <style=cDeath>orbital beam</style> that evaporates targets for <style=cIsDamage>900% damage per second</style>.".AutoFormat();

        public override Type ActivationStateType => typeof(States.Incinerate);

        public override string ActivationMachineName => "Orbital";

        public override float Cooldown => 14f;

        public override Sprite Icon => Load<Sprite>("incinerate.png");
        public override int StockToConsume => 1;
        public override int MaxStock => 1;
        public override bool FullRestockOnAssign => true;
        public override InterruptPriority InterruptPriority => InterruptPriority.PrioritySkill;

        public override string[] Keywords => new string[] { Utils.Keywords.Ignite };
    }
}