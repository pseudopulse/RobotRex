using System;

namespace RobotRex.Skills {
    public class Pollinate : SkillBase<Pollinate>
    {
        public override string Name => "DIRECTIVE: Pollinate";

        public override string Description => "<style=cIsDamage>Stunning.</style> Fire out a slow cluster missile that explodes into <style=cIsUtility>seeking bombs</style> for <style=cIsDamage>7x200% damage</style>.".AutoFormat();

        public override Type ActivationStateType => typeof(States.Pollinate);

        public override string ActivationMachineName => "Missile";

        public override float Cooldown => 8f;

        public override Sprite Icon => Load<Sprite>("intercept.png");
        public override int StockToConsume => 1;
        public override int MaxStock => 1;
        public override bool FullRestockOnAssign => true;
        public override InterruptPriority InterruptPriority => InterruptPriority.PrioritySkill;

        public override string[] Keywords => new string[] { Utils.Keywords.Stun };
    }
}