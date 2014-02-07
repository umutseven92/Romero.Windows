#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Xna.Framework;

#endregion

namespace Romero.Windows.Screens
{
    internal class JoinScreen : MenuScreen
    {

        public NetClient Client;

        public JoinScreen()
            : base("Join")
        {

            var local = new MenuEntry("Join Locally");
            var back = new MenuEntry("Back");

            back.Selected += back_Selected;
            local.Selected += local_Selected;

            MenuEntries.Add(local);
            MenuEntries.Add(back);

        }

        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }

        void local_Selected(object sender, PlayerIndexEventArgs e)
        {
            StartClient(14242, "romero");
            ScreenManager.AddScreen(new LobbyScreen(Client), PlayerIndex.One);

        }

        private void StartClient(int port, string configName)
        {
            var config = new NetPeerConfiguration(configName);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            Client = new NetClient(config);
            Client.Start();
            Client.DiscoverLocalPeers(port);


        }




    }
}
