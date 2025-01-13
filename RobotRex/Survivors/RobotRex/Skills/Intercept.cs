using System;

namespace RobotRex.Skills {
    public class Intercept : SkillBase<Intercept>
    {
        public override string Name => "DIRECTIVE: Intercept";

        public override string Description => "<style=cIsDamage>Stunning.</style> Fire out twin missiles for <style=cIsDamage>2x450% damage</style> that <style=cIsUtility>seek onto a locked target</style>.".AutoFormat();

        public override Type ActivationStateType => typeof(States.Intercept);

        public override string ActivationMachineName => "Missile";

        public override float Cooldown => 5f;

        public override Sprite Icon => Load<Sprite>("intercept.png");
        public override int StockToConsume => 1;
        public override int MaxStock => 1;
        public override bool FullRestockOnAssign => true;
        public override InterruptPriority InterruptPriority => InterruptPriority.PrioritySkill;

        public override string[] Keywords => new string[] { Utils.Keywords.Stun };
    }
}