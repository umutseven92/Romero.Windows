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
        readonly CancellationTokenSource _serverTaskCancelSource = new CancellationTokenSource();
        readonly Dictionary<long, string> _names = new Dictionary<long, string>();
       

        private readonly MenuEntry _characterMenuEntry;
        private static readonly string[] Character =
        {
            "Knight Fraser", "Lady Rebecca", "Sire Benjamin",
            "Cleric Diakonos"
        };


        public JoinScreen()
            : base("Join")
        {
            _characterMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();
            var local = new MenuEntry("Join Locally");
            var back = new MenuEntry("Back");
            _characterMenuEntry.Selected += _characterMenuEntry_Selected;
            back.Selected += back_Selected;
            local.Selected += local_Selected;

            MenuEntries.Add(_characterMenuEntry);
            MenuEntries.Add(local);
            MenuEntries.Add(back);

        }

        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            Client.Disconnect("Disconnect by user");
            _serverTaskCancelSource.Cancel();
            ExitScreen();
        }

        void local_Selected(object sender, PlayerIndexEventArgs e)
        {
            StartClient(14242, "romero");
            //var serverTask = new Task(ConnectToServer, _serverTaskCancelSource.Token);
            //serverTask.Start();
            ScreenManager.AddScreen(new LobbyScreen(Client), PlayerIndex.One);
        }

        private void SendNameToServer()
        {
            var om = Client.CreateMessage();
            om.Write("Player Two");
            Client.SendMessage(om, NetDeliveryMethod.Unreliable);
        }

        private void StartClient(int port, string configName)
        {
            var config = new NetPeerConfiguration(configName);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
            Client = new NetClient(config);
            Client.Start();
            Client.DiscoverLocalPeers(port);

        }

        private void ConnectToServer()
        {

            while (true)
            {
                SendNameToServer();

                NetIncomingMessage msg;
                while ((msg = Client.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryResponse:
                            // just connect to first server discovered
                            try
                            {
                                Client.Connect(msg.SenderEndpoint);
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                            
                            //Connected
                            ScreenManager.AddScreen(new CharacterSelectScreen(), PlayerIndex.One);
                            break;
                        case NetIncomingMessageType.Data:
                            var who = msg.ReadInt64();
                            var p2 = msg.ReadString();
                            _names[who] = p2;
                            break;
                    }
                }
            }

        }

        void _characterMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            switch (_characterMenuEntry.Text)
            {
                case "Character: Knight Fraser":
                    Global.SelectedCharacter = Global.Character.Becky;
                    break;
                case "Character: Lady Rebecca":
                    Global.SelectedCharacter = Global.Character.Ben;
                    break;
                case "Character: Sire Benjamin":
                    Global.SelectedCharacter = Global.Character.Deacon;
                    break;
                case "Character: Cleric Diakonos":
                    Global.SelectedCharacter = Global.Character.Fraser;
                    break;
            }
            SetMenuEntryText();
        }

        private void SetMenuEntryText()
        {
            switch (Global.SelectedCharacter)
            {
                case Global.Character.Fraser:
                    _characterMenuEntry.Text = "Character: " + Character[0];
                    break;
                case Global.Character.Becky:
                    _characterMenuEntry.Text = "Character: " + Character[1];
                    break;
                case Global.Character.Ben:
                    _characterMenuEntry.Text = "Character: " + Character[2];
                    break;
                case Global.Character.Deacon:
                    _characterMenuEntry.Text = "Character: " + Character[3];
                    break;
            }
        }
    }
}
