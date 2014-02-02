
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Romero.Windows.Screens
{
    internal class LobbyScreen : MenuScreen
    {
        private readonly MenuEntry _characterMenuEntry;
        private static readonly string[] Character =
        {
            "Knight Fraser", "Lady Rebecca", "Sire Benjamin",
            "Cleric Diakonos"
        };

        Dictionary<long, string> names = new Dictionary<long, string>();
        public NetClient Client;
        readonly CancellationTokenSource _serverTaskCancelSource = new CancellationTokenSource();
        private MenuEntry secondPlayer;
        private MenuEntry thirdPlayer;
        private MenuEntry fourthPlayer;

        public LobbyScreen()
            : base("Lobby")
        {
            _characterMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();
            var play = new MenuEntry("Play");
            var back = new MenuEntry("Back");
            secondPlayer = new MenuEntry("Second Player: Not Connected");
            thirdPlayer = new MenuEntry("Third Player: Not Connected");
            fourthPlayer = new MenuEntry("Fourth Player: Not Connected");
            _characterMenuEntry.Selected += _characterMenuEntry_Selected;
            back.Selected += back_Selected;
            play.Selected += play_Selected;
            MenuEntries.Add(_characterMenuEntry);
            MenuEntries.Add(secondPlayer);
            MenuEntries.Add(thirdPlayer);
            MenuEntries.Add(fourthPlayer);
            MenuEntries.Add(play);
            MenuEntries.Add(back);

            var serverTask = new Task(ConnectToServer, _serverTaskCancelSource.Token);

            StartClient(14242, "romero");
            serverTask.Start();

        }

        private void SendDataToServer()
        {
            var om = Client.CreateMessage();
            om.Write("Player One");
            Client.SendMessage(om, NetDeliveryMethod.Unreliable);
        }

        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            _serverTaskCancelSource.Cancel();
            ExitScreen();
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
                SendDataToServer();
                NetIncomingMessage msg;
                while ((msg = Client.ReadMessage()) != null)
                {
                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryResponse:
                            // just connect to first server discovered
                            Client.Connect(msg.SenderEndpoint);
                            break;
                        case NetIncomingMessageType.Data:
                            long who = msg.ReadInt64();
                            string p2 = msg.ReadString();
                            names[who] = p2;

                            break;

                    }
                }
                if (names.Count > 1)
                {
                    secondPlayer.Text = names.ToArray()[1].ToString();
                }
                if (names.Count > 2)
                {
                    thirdPlayer.Text = names.ToArray()[2].ToString();
                }
                if (names.Count > 3)
                {
                    fourthPlayer.Text = names.ToArray()[3].ToString();
                }


            }

        }



        void play_Selected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
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
