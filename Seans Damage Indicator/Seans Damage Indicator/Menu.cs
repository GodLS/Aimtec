using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aimtec.SDK.Menu.Components;

namespace Seans_Damage_Indicator
{
    class Menu
    {
        public static Aimtec.SDK.Menu.Menu _menu { get; set; }

        public static void Create()
        {
            _menu = new Aimtec.SDK.Menu.Menu("seans.damageindicator", "Seans Damage Indicator", true);
            _menu.Attach();

            _menu.Add(new MenuBool("enabled", "Enabled"));
            _menu.Add(new MenuSliderBool("autoattackcount", "Auto attacks to add", false, 1, 1, 10));
        }
    }
}
