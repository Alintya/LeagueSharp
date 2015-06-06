using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace MissedCreeps
{
    class Program
    {

        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }
        private static Menu _menu;

        private static int _minionsMissed;
        private static int _minionsDied;
        private static int _lastMinioncount;
        private static int _lastSMinions;
        private static List<GameObject> _checkedMinions = new List<GameObject>();

        private static int _x, _y;
        private static Render.Text _text, _text2, text2;


        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }


        private static void OnGameLoad(EventArgs args)
        {

            _menu = new Menu("MissedCs", "mCs", true);
            _menu.AddItem(new MenuItem("drawMissed", "Display missed Creeps").SetValue(true));
            _menu.AddToMainMenu();

            _text = new Render.Text(_x, _y, "Minions Missed: " + _minionsMissed, 30, Color.Gold);
            _text.Add();
            text2 = new Render.Text(_x, _y+200, Player.NetworkId.ToString(), 30, Color.Gold);
            text2.Add();
            _text2 = new Render.Text(_x, _y+100, "Minions Missed: " + _minionsMissed, 30, Color.Gold);
            _text2.Add();

            _lastMinioncount = Player.MinionsKilled;

//            AttackableUnit.OnDamage += OnUnitDamage;
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += OnDraw;
            GameObject.OnDelete += OnUnitDelete;

            Game.PrintChat("MissedCS loaded");


        }

        private static void OnUnitDelete(GameObject sender, EventArgs args)
        {
            if (sender.Type == GameObjectType.obj_AI_Minion)
            {
                if (_checkedMinions.Contains(sender))
                {
                    Notifications.AddNotification("object found", 500);
                    _checkedMinions.Remove(sender);
                }
//                else
//                    Notifications.AddNotification("object was not in list", 500);
            }
        }

        private static void OnDraw(EventArgs args)
        {
            _text.text = "Minions Missed: " + _minionsMissed;
//            _text2.text = _minionsDied.ToString();
        }

        private static void OnUnitDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {   
//            Notifications.AddNotification(args.SourceNetworkId.ToString(), 10);
//            _minionsMissed++;
            if (sender.Type == GameObjectType.obj_AI_Minion)
            {
                Notifications.AddNotification("call is minion", 100);
                if (args.Damage > sender.Health)
                    _minionsDied++; //Notifications.AddNotification("ops", 100);
//                if (args.SourceNetworkId == Player.NetworkId) Notifications.AddNotification("minion is attacked by player", 100);
            }
            else Notifications.AddNotification("call is not a minion", 100);


            if (sender.Type == GameObjectType.obj_AI_Minion && args.SourceNetworkId == Player.NetworkId && args.Damage > sender.Health)
            {
                
//                minionsMissed++;
            }
        }
        
        private static void OnGameUpdate(EventArgs args)
        {   
            _minionsDied = 0;

            if (MinionManager.GetMinions(900).Count == 0 || Player.IsDead)
            {
               
//                Notifications.AddNotification("No minion in range", 1000);
                return;
            }


            var minions = ObjectManager.Get<Obj_AI_Minion>().Where(h => h.IsValid && h.IsEnemy && h.IsMinion && h.IsDead && h.IsVisible);
//            var minions =
//                MinionManager.GetMinions(700, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.None)
//                    .Where(h => h.IsDead);

            
//            _minionsDied = minions2.Count();
            foreach (var unit in minions)
            {
                if (!_checkedMinions.Contains((GameObject)unit))
                {
                    int hasgoldgiven = Convert.ToInt32(unit.ScriptGoldGiven);
                    if (hasgoldgiven > 0)
                    {
                        text2.text = hasgoldgiven.ToString();
                        Notifications.AddNotification(hasgoldgiven.ToString(), 500);
                    }
                    
                    _minionsDied++;
                    _checkedMinions.Add((GameObject)unit);
//                    Notifications.AddNotification(" added", 20);
                }
            }
/*           
            foreach (var unit in minions2)
            {
                if (unit.IsMinion && unit.IsEnemy)
                {
                    if(unit.IsDead) _minionsDied++;
                }

            }
*/
/*            
            if (_minionsDied <= _lastSMinions)
            {
                _lastSMinions = _minionsDied;
                return;
            }
*/                


            _text2.text = _minionsDied.ToString();
            _minionsMissed -= Player.MinionsKilled - _lastMinioncount - _minionsDied;
            
            _lastMinioncount = Player.MinionsKilled;

        }

        
    }
}
