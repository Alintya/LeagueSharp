using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using ChampAssembly.Manager;
using ChampAssembly.Utilities;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace ChampAssembly
{
    class Champion
    {

        // declare shorthandle to access the player object
        // Properties http://msdn.microsoft.com/en-us/library/aa288470%28v=vs.71%29.aspx 
        private readonly Obj_AI_Hero Player = ObjectManager.Player;


        // declare orbwalker class
        private static Orbwalking.Orbwalker Orbwalker;

        //Menu
        public static Menu menu;
        private static readonly Menu OrbwalkerMenu = new Menu("Orbwalker", "Orbwalker");

        // declare  list of spells
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;

        // declare list of items
//        private static Items.Item Dfg;

        // declare Laugh
        private static int _lastLaugh;

        private static int _lastFlash;


        //champion constructor
        public Champion()
        {
            // Events http://msdn.microsoft.com/en-us/library/edzehd2t%28v=vs.110%29.aspx
            // OnGameLoad event, gets fired after loading screen is over
 //           CustomEvents.Game.OnGameLoad += Game_OnGameLoad;

            CustomEvents.Game.OnGameLoad += OnGameLoad;

            Game.PrintChat("Champion Constructor");
            Notifications.AddNotification("Champion Constructor", 1000);

        }





        /// <summary>
        /// Game Loaded Method
        /// </summary>
        private void OnGameLoad(EventArgs args)
        {
            //Load reached
            Game.PrintChat("<font color = \"#FFB6C1\"> Test </font> Test2 <font color = \"#00FFFF\"> Test3 </font>");
            Notifications.AddNotification("Test3", 2000);


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (Player.ChampionName.ToLower() != "veigar") // check if the current champion is supported
                return; // stop programm


            InitializeSpells();

            InitializeMenu();
            LoadChampionMenu();


            

            // set spells prediction values, not used on Nunu
            // Method Spell.SetSkillshot(float delay, float width, float speed, bool collision, SkillshotType type)
            // Q.SetSkillshot(0.25f, 80f, 1800f, false, SkillshotType.SkillshotLine);
 /*           Q = new Spell(SpellSlot.Q, 125); // create Q spell with a range of 125 units
            W = new Spell(SpellSlot.W, 700); // create W spell with a range of 700 units
            E = new Spell(SpellSlot.E, 550); // create E spell with a range of 550 units
            R = new Spell(SpellSlot.R, 650); // create R spell with a range of 650 units
*/
            // create Dfg item id 3128 and range of 750 units
            // Constructor Items.Item(int id, float range)
            //            Dfg = new Items.Item(3128, 750); // or use ItemId enum
            //            Dfg = new Items.Item((int)ItemId.Deathfire_Grasp, 750);




            // Menu.AddItem(MenuItem item) returns added MenuItem
            // Constructor MenuItem(string name, string displayName)
            // .SetValue(true) on/off button
//            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
//            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
//            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
//            spellMenu.AddItem(new MenuItem("useR", "Use R to Farm Stacks").SetValue(false));



            // create MenuItem 'ConsumeHealth' as Slider
            // Constructor Slider(int value, int min, int max)
//            spellMenu.AddItem(new MenuItem("ConsumeHealth", "Consume below HP").SetValue(new Slider(40, 1, 100)));

            // subscribe to Drawing event
            Drawing.OnDraw += Drawing_OnDraw;

            // subscribe to Update event gets called every game update around 10ms
            Game.OnUpdate += Game_OnGameUpdate;



            // print text in local chat
            Game.PrintChat("OnLoad complete");
            Notifications.AddNotification("OnLoad complete", 2000);

        }

        private static void InitializeSpells()
        {
            // the Spell class provides methods to check and cast Spells
            // Constructor Spell(SpellSlot slot, float range)
            SpellManager.Q = new Spell(SpellSlot.Q, 950);
            SpellManager.W = new Spell(SpellSlot.W, 900);
            SpellManager.E = new Spell(SpellSlot.E, 725);
            SpellManager.R = new Spell(SpellSlot.R, 650);

            // set spells prediction values
            SpellManager.Q.SetSkillshot(.1f, 70f, 2000f, false, SkillshotType.SkillshotLine);
            SpellManager.W.SetSkillshot(1.25f, 230f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            SpellManager.E.SetSkillshot(.5f, 425f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            //deprecated
/*          QMana = new[] { 60, 60, 65, 70, 75, 80 };
            WMana = new[] { 70, 70, 75, 80, 85, 90 };
            EMana = new[] { 80, 80, 85, 90, 95, 100 };
            RMana = new[] { 120, 120, 100, 80 };
*/
        }

        private void InitializeMenu()
        {

            //Champmenu
            // create root menu
            // Constructor Menu(string displayName, string name, bool root)
            menu = new Menu(Player.ChampionName, Player.ChampionName, true);

            //Target selector
            // create submenu for TargetSelector used by Orbwalker
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            menu.AddSubMenu(targetSelectorMenu);

            //Orbwalkermenu
            // create and add submenu 'Orbwalker'
            // Menu.AddSubMenu(Menu menu) returns added Menu
            menu.AddSubMenu(OrbwalkerMenu);
            Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);

            //Item Menu
            var itemMenu = new Menu("Items and Summoners", "Items");
            ItemManager.AddToMenu(itemMenu);
            menu.AddSubMenu(itemMenu);

            //Spells menu
            Menu spellMenu = new Menu("Spells", "Spells");
            menu.AddSubMenu(spellMenu);

            // create MenuItem 'LaughButton' as Keybind
            // Constructor KeyBind(int keyCode, KeyBindType type)
            spellMenu.AddItem(new MenuItem("LaughButton", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));



            // attach to 'Shift/F9' Menu
            menu.AddToMainMenu();
            
        }
 
        private void LoadChampionMenu()
        {
            //Hotkeys
            var key = new Menu("Key", "Key");
            {
                key.AddItem(new MenuItem("ComboActive", "Combo!", true).SetValue(new KeyBind(32, KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActive", "Harass!", true).SetValue(new KeyBind("C".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("HarassActiveT", "Harass (toggle)!", true).SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("LaneClearActive", "Farm!", true).SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
                key.AddItem(new MenuItem("LastHitQQ", "Use Q only to cs", true).SetValue(new KeyBind("A".ToCharArray()[0], KeyBindType.Press)));
//                key.AddItem(new MenuItem("wPoke", "Cast W only after Stun", true).SetValue(new KeyBind("N".ToCharArray()[0], KeyBindType.Toggle)));
                key.AddItem(new MenuItem("escape", "Escape", true).SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));
                //add to menu
                menu.AddSubMenu(key);
            }
            //Combo
            var combo = new Menu("Combo", "Combo");
            {
                combo.AddSubMenu(HitChanceManager.AddHitChanceMenuCombo(true, true, true, false));

                combo.AddItem(new MenuItem("selected", "Focus Selected Target", true).SetValue(true));
                combo.AddItem(new MenuItem("UseQCombo", "Use Q", true).SetValue(true));
                combo.AddItem(new MenuItem("UseWCombo", "Use W", true).SetValue(true));
                combo.AddItem(new MenuItem("UseECombo", "Use E", true).SetValue(true));
                combo.AddItem(new MenuItem("waitW", "Wait for W to E", true).SetValue(true));
                combo.AddItem(new MenuItem("castWonTopOfE", "Cast W immediately after E", true).SetValue(false));
                combo.AddItem(new MenuItem("UseRCombo", "Use R", true).SetValue(true));
                //add to menu
                menu.AddSubMenu(combo);
            }
            //Harass
            var harass = new Menu("Harass", "Harass");
            {
                harass.AddItem(new MenuItem("UseQHarass", "Use Q", true).SetValue(true));
                harass.AddItem(new MenuItem("UseWHarass", "Use W", true).SetValue(true));
                harass.AddItem(new MenuItem("UseEHarass", "Use E", true).SetValue(true));
                harass.AddSubMenu(HitChanceManager.AddHitChanceMenuHarass(true, true, true, false));
                ManaManager.AddManaManagertoMenu(harass, "Harass", 60);
                //add to menu
                menu.AddSubMenu(harass);
            }
            //LClear
            var farm = new Menu("LaneClear", "LaneClear");
            {
                farm.AddItem(new MenuItem("UseQFarm", "Use Q", true).SetValue(true));
                farm.AddItem(new MenuItem("UseWFarm", "Use W", true).SetValue(true));
                ManaManager.AddManaManagertoMenu(farm, "Farm", 50);
                //add to menu
                menu.AddSubMenu(farm);
            }

            //Misc Menu:
            var misc = new Menu("Misc", "Misc"); { 
                misc.AddItem(new MenuItem("UseInt", "Use R to Interrupt", true).SetValue(true));
                misc.AddItem(new MenuItem("UseGap", "Use W for GapCloser", true).SetValue(true));
                misc.AddItem(new MenuItem("overKill", "Over Kill Check", true).SetValue(true));
//                misc.AddItem(new MenuItem("smartKS", "Use Smart KS System", true).SetValue(true));

                misc.AddSubMenu(new Menu("Dont use R on", "DontUlt"));

                foreach (Obj_AI_Hero enemy in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => enemy.Team != Player.Team))
                    misc
                        .SubMenu("DontUlt")
                        .AddItem(new MenuItem("DontUlt" + enemy.BaseSkinName, enemy.BaseSkinName, true).SetValue(false));

                menu.AddSubMenu(misc);
            }

            var drawMenu = new Menu("Drawing", "Drawing");
            {
                drawMenu.AddItem(new MenuItem("Draw_Disabled", "Disable All", true).SetValue(false));
                drawMenu.AddItem(new MenuItem("Draw_Q", "Draw Q", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("Draw_W", "Draw W", true).SetValue(true));
                drawMenu.AddItem(new MenuItem("ERange", "E range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                drawMenu.AddItem(new MenuItem("Draw_R", "Draw R", true).SetValue(true));

                MenuItem drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                MenuItem drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                drawMenu.AddItem(drawComboDamageMenu);
                drawMenu.AddItem(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<bool>();
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };
                //add to menu
                menu.AddSubMenu(drawMenu);
            }

        }




        /// <summary>
        /// Main Update Method
        /// </summary> 
        private void Game_OnGameUpdate(EventArgs args)
        {
            
            //check if player is dead
            if (Player.IsDead) return;

            if (menu.Item("ComboActive", true).GetValue<KeyBind>().Active)
            {
 //               Notifications.AddNotification("asdgfdsafgsdfg", 1);
                Combo();
                
            }
            else
            {
                if (menu.Item("LastHitQQ", true).GetValue<KeyBind>().Active)
                {
                    LastHit();
                }

                if (menu.Item("HarassActive", true).GetValue<KeyBind>().Active)
                    Harass();

                if (menu.Item("HarassActiveT", true).GetValue<KeyBind>().Active)
                    Harass();
            }

        }
/*
        protected virtual void Game_OnGameUpdate(EventArgs args)
        {
            // dont do stuff while dead
            if (Player.IsDead)
                return;

            // checks the current Orbwalker mode Combo/Mixed/LaneClear/LastHit
            if (Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                // combo to kill the enemy
                Consume();
                Bloodboil();
                Iceblast();
            }

            if (Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                // farm and harass
                Consume();
                Iceblast();
            }

            if (Champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                // fast minion farming
                AbsoluteZero();
            }

            // special keybind pressed (32 = Space)
            if (Champion.menu.Item("LaughButton").GetValue<KeyBind>().Active)
            {
                // send Laugh every 4.20 seconds
                if (Environment.TickCount > Champion.LastLaugh + 4200)
                {
                    // create Laugh emote packet and send it
                    // disabled because packets broken with 4.21
                    // Packet.C2S.Emote.Encoded(new Packet.C2S.Emote.Struct((byte)Packet.Emotes.Laugh)).Send();

                    // save last time Laugh was send
                    Champion.LastLaugh = Environment.TickCount;
                }
            }
        }
*/
        /// <summary>
        /// Main Draw Method
        /// </summary> 
        private void Drawing_OnDraw(EventArgs args)
        {
            if (menu.Item("Draw_Disabled", true).GetValue<bool>())
                return;

            if (menu.Item("Draw_Q", true).GetValue<bool>())
                if (Q.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Q.IsReady() ? Color.MediumBlue : Color.Red);

            if (menu.Item("Draw_W", true).GetValue<bool>())
                if (W.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, W.Range, W.IsReady() ? Color.MediumBlue : Color.Red);

            if (menu.Item("Draw_R", true).GetValue<bool>())
                if (R.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, R.Range, R.IsReady() ? Color.MediumBlue : Color.Red);
        }


        private void UseSpells(bool useQ, bool useW, bool useE, bool useR, string source)
        {
            
//            if (source == "Harass" && !ManaManager.HasMana(source))
//                return;

            var range = E.IsReady() ? E.Range : R.Range;
            var focusSelected = menu.Item("selected", true).GetValue<bool>();

            var target = TargetSelector.GetTarget(range, TargetSelector.DamageType.Magical);

       //     if (target != null)
     //          throw new NotImplementedException() ;
            if (TargetSelector.GetSelectedTarget() != null)
                if (focusSelected && TargetSelector.GetSelectedTarget().Distance(Player.ServerPosition) < range)
                    target = TargetSelector.GetSelectedTarget();

            bool hasMana = ManaManager.FullManaCast();
            bool waitW = menu.Item("waitW", true).GetValue<bool>();

            var dmg = GetComboDamage(target);
            ItemManager.Target = target;

            //see if killable
            if (dmg > target.Health - 50)
                ItemManager.KillableTarget = true;

            ItemManager.UseTargetted = true;

            if (useW && target != null && Player.Distance(target.Position) <= W.Range && W.IsReady())
            {
                PredictionOutput pred = Prediction.GetPrediction(target, 1.25f);
                if (pred.Hitchance >= HitChance.High && W.IsReady())
                    W.Cast(pred.CastPosition);
            }

            if (useE && target != null && E.IsReady() && Player.Distance(target.Position) < E.Range)
            {
                if (!waitW || W.IsReady())
                {
                    CastE(target);
                    return;
                }
            }

            if (useQ && Q.IsReady() && Player.Distance(target.Position) <= Q.Range)
            {
                Notifications.AddNotification("sdgdfgdsgsdfg", 100);
                Q.CastOnUnit(target);
            }

            //R
            if (target != null && R.IsReady())
            {
                useR = rTarget(target) && useR;
                if (useR)
                {
                    CastR(target, dmg);
                }
            }

        }

        private static bool rTarget(Obj_AI_Hero target)
        {
            if ((menu.Item("DontUlt" + target.BaseSkinName, true) != null &&
                 menu.Item("DontUlt" + target.BaseSkinName, true).GetValue<bool>() == false))
                return true;
            return false;
        }

        private void CastE(Obj_AI_Hero target)
        {
            PredictionOutput pred = Prediction.GetPrediction(target, E.Delay);
            Vector2 castVec = pred.UnitPosition.To2D() -
                              Vector2.Normalize(pred.UnitPosition.To2D() - Player.Position.To2D()) * E.Width;

            if (pred.Hitchance >= HitChance.High && E.IsReady())
            {
                E.Cast(castVec);

                if (menu.Item("castWonTopOfE", true).GetValue<bool>())
                    Utility.DelayAction.Add(50, () => W.Cast(pred.UnitPosition));

            }
        }

        private void CastR(Obj_AI_Hero target, float dmg)
        {
            if (menu.Item("overKill", true).GetValue<bool>() && Player.GetSpellDamage(target, SpellSlot.Q) > target.Health)
                return;

            if (Player.Distance(target.Position) > R.Range)
                return;

            if (dmg > target.Health + 20 && R.IsReady())
            {
                R.CastOnUnit(target);
            }
        }


        private float GetComboDamage(Obj_AI_Base target)
        {
            double comboDamage = 0;

            if (Q.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.Q);

            if (W.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.W);

            if (R.IsReady())
                comboDamage += Player.GetSpellDamage(target, SpellSlot.R);

            comboDamage = ItemManager.CalcDamage(target, comboDamage);

            return (float)(comboDamage + Player.GetAutoAttackDamage(target));
        }

        private void Combo()
        {
            UseSpells(menu.Item("UseQCombo", true).GetValue<bool>(),
                menu.Item("UseECombo", true).GetValue<bool>(), menu.Item("UseWCombo", true).GetValue<bool>(), menu.Item("UseRCombo", true).GetValue<bool>(), "Combo");
        }

        private void Harass()
        {
            UseSpells(menu.Item("UseQHarass", true).GetValue<bool>(), menu.Item("UseWHarass", true).GetValue<bool>(),
                menu.Item("UseEHarass", true).GetValue<bool>(), false, "Harass");
        }


        private void Farm()
        {
            if (!ManaManager.HasMana("Farm"))
                return;

            var useQ = menu.Item("UseQFarm", true).GetValue<bool>();
            var useW = menu.Item("UseWFarm", true).GetValue<bool>();

            if (useQ)
                SpellCastManager.CastBasicFarm(Q);

            if (useW && W.IsReady())
                SpellCastManager.CastBasicFarm(W);
        }

        private void LastHit()
        {
            if (!Orbwalking.CanMove(40)) return;

            List<Obj_AI_Base> allMinions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);

            if (Q.IsReady())
            {
                foreach (Obj_AI_Base minion in allMinions)
                {
                    if (minion.IsValidTarget() &&
                        HealthPrediction.GetHealthPrediction(minion, (int)((minion.Distance(Player.Position) / 1500) * 1000 + .25f * 1000), 100) <
                        Player.GetSpellDamage(minion, SpellSlot.Q) - 35)
                    {
                        if (Q.IsReady())
                        {
                            Q.Cast(minion);
                            return;
                        }
                    }
                }
            }
        }


        /*        protected virtual void Drawing_OnDraw(EventArgs args)
                {
                    // dont draw stuff while dead
                    if (Player.IsDead)
                        return;

                    // check if E ready
                    if (Champion.E.IsReady())
                    {
                        // draw Aqua circle around the player
                        Utility.DrawCircle(Player.Position, Champion.Q.Range, Color.Aqua);
                    }
                    else
                    {
                        // draw DarkRed circle around the player while on cd
                        Utility.DrawCircle(Player.Position, Champion.Q.Range, Color.DarkRed);
                    }
                }
        */

        /*
                /// <summary>
                /// Consume logic
                /// </summary>
                protected virtual void Consume()
                {
                    // check if the player wants to use Q
                    if (!Champion.menu.Item("useQ").GetValue<bool>())
                        return;

                    // check if Q ready
                    if (Champion.Q.IsReady())
                    {
                        // get sliders value of 'ConsumeHealth'
                        int sliderValue = Champion.menu.Item("ConsumeHealth").GetValue<Slider>().Value;

                        // calc current percent hp
                        float healthPercent = Player.Health / Player.MaxHealth * 100;

                        // check if we should heal
                        if (healthPercent < sliderValue)
                        {
                            // get first minion in Q range
                            Obj_AI_Base minion = MinionManager.GetMinions(Player.Position, Champion.Q.Range).FirstOrDefault();

                            // check if we found a minion to consume
                            if (minion.IsValidTarget())
                            {
                                Champion.Q.CastOnUnit(minion); // nom nom nom
                            }
                        }
                    }
                }

                /// <summary>
                /// Bloodboil logic
                /// </summary>
                private static void Bloodboil()
                {
                    // check if the player wants to use W
                    if (!Champion.menu.Item("useW").GetValue<bool>())
                        return;

                    // check if W ready
                    if (Champion.W.IsReady())
                    {
                        // gets best target in a range of 800 units
                        Obj_AI_Hero target = TargetSelector.GetTarget(800, TargetSelector.DamageType.Magical);

                        // check if there is an ally in range to buff, be nice :>
                        Obj_AI_Hero ally =
                            ObjectManager.Get<Obj_AI_Hero>()
                                // only get ally + not dead + in W range
                                .Where(hero => hero.IsAlly && !hero.IsDead && Player.Distance(hero) < Champion.W.Range)
                                // get the ally with the most AttackDamage
                                .OrderByDescending(hero => hero.FlatPhysicalDamageMod).FirstOrDefault();

                        // check if we found an ally
                        if (ally != null)
                        {
                            // check if there is a target in our AttackRange or in our ally AttackRange
                            if (target.IsValidTarget(Player.AttackRange + 100) ||
                                ally.CountEnemysInRange((int)ally.AttackRange + 100) > 0)
                            {
                                // buff your ally and yourself
                                Champion.W.CastOnUnit(ally);
                            }
                        }

                        // no ally in range to buff, selcast!
                        // checks if your target is valid (not dead, not too far away, not in zhonyas etc.)
                        // we add +100 to our AttackRange to catch up to the target
                        if (target.IsValidTarget(Player.AttackRange + 100))
                        {
                            // buff yourself
                            Champion.W.CastOnUnit(Player);
                        }
                    }
                }

                /// <summary>
                /// Iceblast logic
                /// </summary>
                private static void Iceblast()
                {
                    // check if the player wants to use E
                    if (!Champion.menu.Item("useE").GetValue<bool>())
                        return;

                    // gets best target in Dfg(750) / E(550)
                    Obj_AI_Hero target = TargetSelector.GetTarget(750, TargetSelector.DamageType.Magical);

                    // check if dfg ready
                    if (Champion.Dfg.IsReady())
                    {
                        // check if we found a valid target in range
                        if (target.IsValidTarget(Champion.Dfg.Range))
                        {
                            // use dfg on him
                            Champion.Dfg.Cast(target);
                        }
                    }

                    // check if E ready
                    if (Champion.E.IsReady())
                    {
                        // check if we found a valid target in range
                        if (target.IsValidTarget(Champion.E.Range))
                        {
                            // blast him
                            Champion.E.CastOnUnit(target);
                        }
                    }
                }

                private static void AbsoluteZero()
                {
                    // check if the player wants to use R
                    if (!Champion.menu.Item("useR").GetValue<bool>())
                        return;

                    // fast lane clear
                    // use Nunu R to clear the lane faster
                    if (Champion.R.IsReady()) // check if R ready
                    {
                        // get the amount of enemy minions in Ultimate range
                        int minionsInUltimateRange = MinionManager.GetMinions(Player.Position, Champion.R.Range).Count;

                        if (minionsInUltimateRange > 10)
                        {
                            // cast Ultimate, gold incomming
                            Champion.R.CastOnUnit(Player);
                        }
                    }


                }

        */
    }

}