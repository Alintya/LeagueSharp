using System;
using LeagueSharp.Common;

namespace ChampAssembly.Manager
{
    public static class HitChanceManager
    {
        private static Menu _menuCombo;
        private static Menu _menuHarass;

        private static readonly HitChance[] MyHitChances = { HitChance.Low, HitChance.Medium, HitChance.High, HitChance.VeryHigh };

        public static Menu AddHitChanceMenuCombo(Boolean q, Boolean w, Boolean e, Boolean r)
        {
            _menuCombo = new Menu("Hitchance", "Hitchance");

            _menuCombo.AddItem(new MenuItem("ComboMovementCheck", "Only Cast When Enemy is moving(More Accurate)", true).SetValue(true));

            if(q)
                _menuCombo.AddItem(new MenuItem("qHitCombo", "Q HitChance", true).SetValue(new StringList(new[] { "Low", "Med", "High", "Very High" }, 2)));
            if(w)
                _menuCombo.AddItem(new MenuItem("wHitCombo", "W HitChance", true).SetValue(new StringList(new[] { "Low", "Med", "High", "Very High" }, 2)));
            if(e)
                _menuCombo.AddItem(new MenuItem("eHitCombo", "E HitChance", true).SetValue(new StringList(new[] { "Low", "Med", "High", "Very High" }, 2)));
            if(r)
                _menuCombo.AddItem(new MenuItem("rHitCombo", "R HitChance", true).SetValue(new StringList(new[] { "Low", "Med", "High", "Very High" }, 2)));

            return _menuCombo;
        }

        public static Menu AddHitChanceMenuHarass(Boolean q, Boolean w, Boolean e, Boolean r)
        {
            _menuHarass = new Menu("Hitchance", "Hitchance");

            _menuHarass.AddItem(new MenuItem("HarassMovementCheck", "Only Cast When Enemy is moving(More Accurate)", true).SetValue(true));

            if (q)
                _menuHarass.AddItem(new MenuItem("qHitHarass", "Q HitChance", true).SetValue(new StringList(new[] { "Low", "Med", "High", "Very High" }, 2)));
            if (w)
                _menuHarass.AddItem(new MenuItem("wHitHarass", "W HitChance", true).SetValue(new StringList(new[] { "Low", "Med", "High", "Very High" }, 2)));
            if (e)
                _menuHarass.AddItem(new MenuItem("eHitHarass", "E HitChance", true).SetValue(new StringList(new[] { "Low", "Med", "High", "Very High" }, 2)));
            if (r)
                _menuHarass.AddItem(new MenuItem("rHitHarass", "R HitChance", true).SetValue(new StringList(new[] { "Low", "Med", "High", "Very High" }, 2)));

            return _menuHarass;
        }

        public static HitChance GetQHitChance(string source)
        {
            if (source == "Combo")
            {
                return MyHitChances[_menuCombo.Item("qHitCombo", true).GetValue<StringList>().SelectedIndex];
            }
            else if(source == "Null")
            {
                return HitChance.Low;
            }
            return MyHitChances[_menuHarass.Item("qHitHarass", true).GetValue<StringList>().SelectedIndex];
        }

        public static HitChance GetWHitChance(string source)
        {
            if (source == "Combo")
            {
                return MyHitChances[_menuCombo.Item("wHitCombo", true).GetValue<StringList>().SelectedIndex];
            }
            else if (source == "Null")
            {
                return HitChance.Low;
            }
            return MyHitChances[_menuHarass.Item("wHitHarass", true).GetValue<StringList>().SelectedIndex];
        }

        public static HitChance GetEHitChance(string source)
        {
            if (source == "Combo")
            {
                return MyHitChances[_menuCombo.Item("eHitCombo", true).GetValue<StringList>().SelectedIndex];
            }
            else if (source == "Null")
            {
                return HitChance.Low;
            }
            return MyHitChances[_menuHarass.Item("eHitHarass", true).GetValue<StringList>().SelectedIndex];
        }

        public static HitChance GetRHitChance(string source)
        {
            if (source == "Combo")
            {
                return MyHitChances[_menuCombo.Item("rHitCombo", true).GetValue<StringList>().SelectedIndex];
            }
            else if (source == "Null")
            {
                return HitChance.Low;
            }
            return MyHitChances[_menuHarass.Item("rHitHarass", true).GetValue<StringList>().SelectedIndex];
        }

    }
}
