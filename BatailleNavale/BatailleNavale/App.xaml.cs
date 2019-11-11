using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BatailleNavale.Controller;
using BatailleNavale.View;

namespace BatailleNavale
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ShutdownMode = ShutdownMode.OnLastWindowClose;

            MainMenuController mainMenuController = new MainMenuController();
        }
    }
}
