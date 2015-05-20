using LeagueSharp;
using LeagueSharp.Common;
using System;
using System.Collections.Generic;

namespace GoldTracker
{
    class Program
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }
        private static Menu menu;
        private static Dictionary<int, float> heroValues;
        private static float allyTeamValue, enemyTeamValue;

        static void Main(string[] args)
        {
 //           CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            if (Game.Mode == GameMode.Running)
            {
                Game_OnGameStart(new EventArgs());
            }

            Game.OnStart += Game_OnGameStart;
        }

        private static void Game_OnGameStart(EventArgs args)
        {
            Game.PrintChat("GoldValue loaded");
   //       Game.PrintChat(Player.FlatArmorPenetrationMod.ToString());
   //       Game.PrintChat(Player.PercentArmorPenetrationMod.ToString());
   //       Game.PrintChat(Player.PercentBonusArmorPenetrationMod.ToString());

            menu = new Menu("GoldTracker", "goldTracker", true);
            menu.AddItem(new MenuItem("displayHeroValues", "Display champion gold values").SetValue(true));
            menu.AddItem(new MenuItem("displayTeamValues", "Display team gold values").SetValue(true));
            menu.AddItem(new MenuItem("includeNonCombatStats", "Include non combat stats (MoveSpeed, Mana, ...)").SetValue(true));
            menu.AddToMainMenu();

            heroValues = new Dictionary<int, float>();
            allyTeamValue = 0;
            enemyTeamValue = 0;



            // Player network id for comparison
            var playerId = ObjectManager.Player.NetworkId;

            // Draw champion gold values
            var heroes = ObjectManager.Get<Obj_AI_Hero>();
            foreach (var hero in heroes)
            {
                heroValues.Add(hero.NetworkId, GetHeroValue(hero));

                var heroCopy = hero; // Prevent access of foreach variable in closure
                var heroText = new Render.Text(0, 0, String.Empty, 20, SharpDX.Color.Gold)
                {
                    OutLined = true,

                    PositionUpdate = () => heroCopy.HPBarPosition + new SharpDX.Vector2(10, -10),

                    TextUpdate = delegate
                    {
                        var heroValue = heroValues[heroCopy.NetworkId];

                        // Display '✓' if the enemy is weaker than the player
                        //         '✖' if the enemy is stronger
                        var valueDiffSymbol = String.Empty;
                        if (heroCopy.IsEnemy)
                            valueDiffSymbol = (heroValue > heroValues[playerId]) ? " ✖" : " ✓";

                        return String.Format("{0:0.0}k" + valueDiffSymbol, heroValue / 1000);
                    },

                    VisibleCondition = sender => menu.Item("displayHeroValues").GetValue<bool>() && !heroCopy.IsDead
                };

                heroText.Add();
            }

            // Draw team gold values
            var teamText = new Render.Text(0, 0, String.Empty, 30, SharpDX.Color.Gold)
            {
                OutLined = true,

                // Display '✓' if the enemy team is weaker than the ally team
                //         '✖' if the enemy team is stronger
                TextUpdate = () => String.Format("Allies {0:0.0}k " + ((enemyTeamValue > allyTeamValue) ? "✖" : "✓") + " {1:0.0}k Enemies", allyTeamValue / 1000, enemyTeamValue / 1000),

                VisibleCondition = sender => menu.Item("displayTeamValues").GetValue<bool>()
            };

            teamText.Add();

            Game.OnUpdate += Game_OnGameUpdate;
        }

/*
        static void Game_OnGameLoad(EventArgs args)
        {   
            Game.PrintChat("GoldValue loaded!");

            menu = new Menu("GoldTracker", "goldTracker", true);
            menu.AddItem(new MenuItem("displayHeroValues", "Display champion gold values").SetValue(true));
            menu.AddItem(new MenuItem("displayTeamValues", "Display team gold values").SetValue(true));
            menu.AddItem(new MenuItem("includeNonCombatStats", "Include non combat stats (MoveSpeed, Mana, ...)").SetValue(true));
            menu.AddToMainMenu();

            heroValues = new Dictionary<int, float>();
            allyTeamValue = 0;
            enemyTeamValue = 0;

            

            // Player network id for comparison
            var playerId = ObjectManager.Player.NetworkId;

            // Draw champion gold values
            var heroes = ObjectManager.Get<Obj_AI_Hero>();
            foreach (var hero in heroes)
            {
                heroValues.Add(hero.NetworkId, GetHeroValue(hero));

                var heroCopy = hero; // Prevent access of foreach variable in closure
                var heroText = new Render.Text(0, 0, String.Empty, 20, SharpDX.Color.Gold)
                {
                    OutLined = true,

                    PositionUpdate = () => heroCopy.HPBarPosition + new SharpDX.Vector2(10, -10),

                    TextUpdate = delegate
                    {
                        var heroValue = heroValues[heroCopy.NetworkId];

                        // Display '✓' if the enemy is weaker than the player
                        //         '✖' if the enemy is stronger
                        var valueDiffSymbol = String.Empty;
                        if (heroCopy.IsEnemy)
                            valueDiffSymbol = (heroValue > heroValues[playerId]) ? " ✖" : " ✓";

                        return String.Format("{0:0.0}k" + valueDiffSymbol, heroValue / 1000);
                    },

                    VisibleCondition = sender => menu.Item("displayHeroValues").GetValue<bool>() && !heroCopy.IsDead
                };

                heroText.Add();
            }

            // Draw team gold values
            var teamText = new Render.Text(0, 0, String.Empty, 30, SharpDX.Color.Gold)
            {
                OutLined = true,

                // Display '✓' if the enemy team is weaker than the ally team
                //         '✖' if the enemy team is stronger
                TextUpdate = () => String.Format("Allies {0:0.0}k " + ((enemyTeamValue > allyTeamValue) ? "✖" : "✓") + " {1:0.0}k Enemies", allyTeamValue / 1000, enemyTeamValue / 1000),

                VisibleCondition = sender => menu.Item("displayTeamValues").GetValue<bool>()
            };

            teamText.Add();

            Game.OnUpdate += Game_OnGameUpdate;
            
        }
*/
        static void Game_OnGameUpdate(EventArgs args)
        {
            float allyTeamRecalculated = 0, enemyTeamRecalculated = 0;
            var heroes = ObjectManager.Get<Obj_AI_Hero>();

            foreach (var hero in heroes)
            {
                // Update champion gold values
                var heroValue = GetHeroValue(hero);
                heroValues[hero.NetworkId] = heroValue;

                // Update team gold values
                if (hero.IsAlly)
                    allyTeamRecalculated += heroValue;
                else if (hero.IsEnemy)
                    enemyTeamRecalculated += heroValue;
            }

            allyTeamValue = allyTeamRecalculated;
            enemyTeamValue = enemyTeamRecalculated;
        }

        // Calculates an estimated gold value of combat stats
        // TODO: Include percent penetration Values
        static float GetHeroValue(Obj_AI_Hero hero)
        {
            var attackDamage = hero.BaseAttackDamage + hero.FlatPhysicalDamageMod;
            var abilityPower = hero.BaseAbilityDamage + hero.FlatMagicDamageMod;
            var armor = hero.Armor;
            var magicResistance = hero.SpellBlock;
            var health = hero.MaxHealth;
            var attackSpeed = 1 / hero.AttackDelay;
            var critChance = hero.Crit;
            var lifeSteal = hero.PercentLifeStealMod;
            var spellVamp = hero.PercentSpellVampMod;
            var cooldownReduction = hero.PercentCooldownMod;
            var armorPen = hero.FlatArmorPenetrationMod;
            var magicPen = hero.FlatMagicPenetrationMod;
            
   //         Game.PrintChat(armorPen.ToString());

            float value = 0;
            value += attackDamage * GoldValue.AttackDamage;
            value += abilityPower * GoldValue.AbilityPower;
            value += armor * GoldValue.Armor;
            value += magicResistance * GoldValue.MagicResistance;
            value += health * GoldValue.Health;
            value += attackSpeed * GoldValue.AttackSpeed;
            value += critChance * 100 * GoldValue.CritChance;
            value += lifeSteal * 100 * GoldValue.LifeSteal;
            value += spellVamp * 100 * GoldValue.SpellVamp;
            value += cooldownReduction * -1 * 100 * GoldValue.CooldownReduction;
            value += armorPen * GoldValue.ArmorPen;
            value += magicPen * GoldValue.MagicPen;

            if (menu.Item("includeNonCombatStats").GetValue<bool>())
            {
                // Include non combat stats
                var moveSpeed = hero.MoveSpeed;
                var mana = hero.MaxMana;
                var healthRegeneration = hero.HPRegenRate;
                var manaRegeneration = hero.PARRegenRate;

                value += moveSpeed * GoldValue.MoveSpeed;
                value += mana * GoldValue.Mana;
                value += healthRegeneration * 5 * GoldValue.HealthRegeneration;
                value += manaRegeneration * 5 * GoldValue.ManaRegeneration;
            }

            return value;
        }
    }
}
