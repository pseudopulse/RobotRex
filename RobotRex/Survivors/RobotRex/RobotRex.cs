using System;
using KinematicCharacterController;
using RobotRex.States;
using RobotRex.Utils.Components;
using ThreeEyedGames;
using Survariants;

namespace RobotRex {
    public class Survivor : SurvivorBase<Survivor>
    {
        public override string Name => "REX?";

        public override string Description => 
        """
        Having left the plant behind, REX? salvages the parts it can find to build its defenses and perform repairs.

        < ! > DIRECTIVE: Suppress can struggle to output sustained damage with its limited ammunition. Tinker with Gunner Turrets to increase it's capacity.

        < ! > When under threat, DIRECTIVE: Intercept can temporarily neutralize a focused target. Salvaging Missile Drones will add more missiles to the volley.

        < ! > DIRECTIVE: Pursuit provides great mobility and shreds a wide area, but has poor air control.
        
        < ! > DIRECTIVE: Incinerate is able to dish out incredible area of effect damage from long distances. Decommissioning Incinerator Drones will boost its power.
        """;

        public override string Subtitle => "Aimless Construct";

        public override string Outro => "...and so it left, buried behind amalgam.";

        public override string Failure => "...and so it vanished, having never been complete.";
        public static GameObject LaserSignal;
        public static GameObject OrbitalBeam;
        public static GameObject AcidProjectile;
        public static GameObject PollinateProjectile;
        public static GameObject PollinateBeeMissile;
        public static LazyIndex RobotRex = new("RobotRexBody");
        public static GameObject TargetPainter;
        public static Dictionary<string, Action<RRDroneStats>> DroneMap = new() {
            {"Drone1Master", (x) => {
                x.BonusChaingunSpeed += 0.25f;
                x.BonusArmor += 2;
                x.BonusHealth += 0.05f;
                x.BonusScale += 0.05f;
            }},
            {"Turret1Master", (x) => {
                x.BonusBullets += 10;
                x.BonusArmor += 5;
                x.BonusHealth += 0.05f;
                x.BonusScale += 0.15f;
            }},
            {"Drone2Master", (x) => {
                x.BonusRegen += 0.1f;
                x.BonusArmor += 2;
                x.BonusHealth += 0.1f;
                x.BonusScale += 0.05f;
            }},
            {"DroneMissileMaster", (x) => {
                x.BonusMissiles += 2;
                x.BonusArmor += 5;
                x.BonusHealth += 0.05f;
                x.BonusScale += 0.1f;
            }},
            {"EmergencyDroneMaster", (x) => {
                x.BonusRegen += 0.25f;
                x.BonusArmor += 10;
                x.BonusHealth += 0.2f;
                x.BonusScale += 0.1f;
            }},
            {"FlameDroneMaster", (x) => {
                x.BonusIncinerationDamage += 0.25f;
                x.BonusArmor += 10;
                x.BonusHealth += 0.05f;
                x.BonusScale += 0.15f;
            }},
            {"MegaDroneMaster", (x) => {
                x.BonusArmor += 15;
                x.BonusHealth += 0.2f;
                x.BonusChaingunSpeed += 0.25f;
                x.BonusBullets += 25;
                x.BonusMissiles += 6;
                x.BonusIncinerationDamage += 0.25f;
                x.BonusRegen += 0.2f;
                x.BonusScale += 0.45f;
            }},
        };

        public override void LoadAssets()
        {
            Body = Load<GameObject>("RobotRexBody.prefab");
            Master = PrefabAPI.InstantiateClone(Utils.Assets.GameObject.TreebotMonsterMaster, "RobotRexMaster");

            Body.GetComponentInChildren<Animator>().runtimeAnimatorController = Paths.RuntimeAnimatorController.animTreebot;
            Load<GameObject>("DisplayRobotRex.prefab").GetComponentInChildren<Animator>().runtimeAnimatorController = Paths.RuntimeAnimatorController.animTreebotDisplay;

            SurvivorDef = Load<SurvivorDef>("sdRobotRex.asset");

            Body.GetComponent<CharacterBody>()._defaultCrosshairPrefab = Paths.GameObject.TreebotCrosshair;

            CharacterMaster master = Master.GetComponent<CharacterMaster>();
            master.bodyPrefab = Body;

            SkillLocator locator = Body.GetComponent<SkillLocator>();

            locator.passiveSkill.skillNameToken = "ROBOREX_PASSIVE_NAME";
            locator.passiveSkill.skillDescriptionToken = "ROBOREX_PASSIVE_DESC";
            "ROBOREX_PASSIVE_NAME".Add("Field Modification");
            "ROBOREX_PASSIVE_DESC".Add("Repaired <style=cIsUtility>drones</style> are <style=cIsUtility>merged into you</style> to improve stats and weaponry.");

            ReplaceSkills(locator.primary, Skills.Suppress.instance.skillDef);
            ReplaceSkills(locator.secondary, Skills.Intercept.instance.skillDef);
            ReplaceSkills(locator.utility, Skills.Pursuit.instance.skillDef);
            ReplaceSkills(locator.special, Skills.Incinerate.instance.skillDef);

            ChildLocator cl = Body.GetComponentInChildren<ChildLocator>();
            Transform turret = cl.FindChild("TurretHull");
            turret.GetComponent<Animator>().runtimeAnimatorController = Paths.RuntimeAnimatorController.animTurret1;

            cl.gameObject.AddComponent<RRAnimHandler>().Turret = turret.GetComponent<Animator>();

            Body.GetComponent<KinematicCharacterMotor>().playerCharacter = true;

            ContentAddition.AddNetworkedObject(Body);
            PrefabAPI.RegisterNetworkPrefab(Body);

            Body.GetComponent<CameraTargetParams>().cameraParams = Paths.CharacterCameraParams.ccpTreebot;

            Body.AddComponent<RRMissileTracker>();

            LaserSignal = Load<GameObject>("LaserSignal.prefab");
            OrbitalBeam = Load<GameObject>("OrbitalLaser.prefab");

            var s1 = LaserSignal.AddComponent<DetachLineRendererAndFade>();
            var s2 = OrbitalBeam.AddComponent<DetachLineRendererAndFade>();

            s1.line = LaserSignal.GetComponentInChildren<LineRenderer>();
            s1.decayTime = 0.4f;
            s2.line = OrbitalBeam.GetComponentInChildren<LineRenderer>();
            s2.decayTime = 0.4f;

            /*AcidProjectile = PrefabAPI.InstantiateClone(Paths.GameObject.MolotovProjectileDotZone, "the fucking sludge");

            AcidProjectile.FindComponent<Decal>("Decal").Material = Paths.Material.matRescueshipImpactDecalLarge;
            AcidProjectile.FindComponent<ParticleSystemRenderer>("Fire, Billboard").gameObject.SetActive(false);
            AcidProjectile.transform.Find("FX").Find("ScaledOnImpact").Find("TeamAreaIndicator, GroundOnly").gameObject.SetActive(false);

            AcidProjectile.RemoveComponent<ProjectileDotZone>();
            AcidProjectile.RemoveComponent<ProjectileController>();
            AcidProjectile.RemoveComponent<ProjectileDamage>();
            AcidProjectile.RemoveComponent<TeamFilter>();
            AcidProjectile.RemoveComponent<MeshFilter>();
            AcidProjectile.RemoveComponent<HitBoxGroup>();
            AcidProjectile.RemoveComponent<NetworkIdentity>();*/

            On.RoR2.CharacterBody.Start += OnBodyStart;
            RecalculateStatsAPI.GetStatCoefficients += HandleDroneStats;

            On.RoR2.SummonMasterBehavior.OpenSummon += OpenSummon;

            TargetPainter = PrefabAPI.InstantiateClone(Paths.GameObject.EngiMissileTrackingIndicator, "RRTargetPainter");
            var spr = TargetPainter.FindComponent<SpriteRenderer>("Base Core");
            spr.color = Color.red;
            spr.sprite = Paths.Texture2D.texCaptainCrosshairInner.MakeSprite();
            spr.transform.localScale = new(0.01f, 0.01f, 0.01f);

            PollinateProjectile = Load<GameObject>("PollinateProjectile.prefab");
            PollinateProjectile.gameObject.layer = LayerIndex.projectile.intVal;
            PollinateProjectile.GetComponent<ProjectileImpactExplosion>().explosionEffect = Paths.GameObject.CryoCanisterExplosionPrimary;
            PollinateProjectile.AddComponent<RRBeeUpdater>();

            PollinateBeeMissile = Load<GameObject>("PollinateMissile.prefab");
            PollinateBeeMissile.gameObject.layer = LayerIndex.projectile.intVal;
            PollinateBeeMissile.GetComponent<ProjectileSingleTargetImpact>().impactEffect = Paths.GameObject.ExplosionDroneDeath;

            ContentAddition.AddProjectile(PollinateProjectile);
            ContentAddition.AddProjectile(PollinateBeeMissile);

            SurvivorVariantDef svd = new();
            svd.Color = Body.GetComponent<CharacterBody>().bodyColor;
            svd.DisplayName = "REX?";
            svd.TargetSurvivor = Paths.SurvivorDef.Treebot;
            svd.VariantSurvivor = SurvivorDef;
            svd.Description = "Aimless Construct";
            
            SurvivorDef.hidden = true;
            
            SurvivorVariantCatalog.AddSurvivorVariant(svd);
        }

        private void OpenSummon(On.RoR2.SummonMasterBehavior.orig_OpenSummon orig, SummonMasterBehavior self, Interactor activator)
        {
            if (activator && activator.GetComponent<CharacterBody>()) {
                CharacterBody body = activator.GetComponent<CharacterBody>();

                if (body.bodyIndex == RobotRex) {
                    string name = self.masterPrefab.name;

                    if (DroneMap.ContainsKey(name)) {
                        EffectManager.SpawnEffect(Paths.GameObject.OmniExplosionVFXRoboBallDeath, new EffectData { origin = self.transform.position, scale = 5f}, true);
                        AkSoundEngine.PostEvent(Events.Play_drone_repair, self.gameObject);
                        
                        DroneMap[name](body.master.GetComponent<RRDroneStats>());

                        body.statsDirty = true;
                        return;
                    }
                }
            }

            orig(self, activator);
        }

        private void HandleDroneStats(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.master) {
                RRDroneStats stats = sender.master.GetComponent<RRDroneStats>();

                if (stats) {
                    sender.transform.localScale = Vector3.one * (1f + (stats.BonusScale));
                    sender.GetComponent<Interactor>().maxInteractionDistance = 3f * (1f + stats.BonusScale);
                    args.regenMultAdd += stats.BonusRegen;
                    args.healthMultAdd += stats.BonusHealth;
                    args.armorAdd += stats.BonusArmor;

                    sender.skillLocator.primary.SetBonusStockFromBody(stats.BonusBullets);
                }
            }
        }

        private void OnBodyStart(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);

            if (self.bodyIndex == RobotRex && self.master) {
                if (!self.master.GetComponent<RRDroneStats>()) {
                    self.master.AddComponent<RRDroneStats>();
                }
            }
        }
    }
}