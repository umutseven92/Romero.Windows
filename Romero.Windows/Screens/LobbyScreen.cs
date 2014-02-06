#region Using Statements

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Xna.Framework;

#endregion

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

        private int playerCount = 1;

        readonly Dictionary<long, string> _names = new Dictionary<long, string>();
        public NetClient Client;
        readonly CancellationTokenSource _serverTaskCancelSource = new CancellationTokenSource();
        
        private readonly MenuEntry _secondPlayer;
        private readonly MenuEntry _thirdPlayer;
        private readonly MenuEntry _fourthPlayer;
        private readonly MenuEntry _firstPlayer;

        private readonly MenuEntry[] _menuEntryArray; 

        /// <summary>
        /// Creating a lobby screen
        /// </summary>
        public LobbyScreen()
            : base("Lobby")
        {
            _characterMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();
            var play = new MenuEntry("Play");
            var back = new MenuEntry("Back");
            _firstPlayer = new MenuEntry("First Player: Not Connected");
            _secondPlayer = new MenuEntry("Second Player: Not Connected");
            _thirdPlayer = new MenuEntry("Third Player: Not Connected");
            _fourthPlayer = new MenuEntry("Fourth Player: Not Connected");
            _characterMenuEntry.Selected += _characterMenuEntry_Selected;
            back.Selected += back_Selected;
            play.Selected += play_Selected;
            MenuEntries.Add(_characterMenuEntry);
            MenuEntries.Add(_firstPlayer);
            MenuEntries.Add(_secondPlayer);
            MenuEntries.Add(_thirdPlayer);
            MenuEntries.Add(_fourthPlayer);
            MenuEntries.Add(play);
            MenuEntries.Add(back);

            _menuEntryArray = new[] {_firstPlayer, _secondPlayer, _thirdPlayer, _fourthPlayer};

            var serverTask = new Task(ConnectToServer, _serverTaskCancelSource.Token);
            StartClient(14242, "romero");
            serverTask.Start();

        }

        /// <summary>
        /// Joining an existing lobby
        /// </summary>
        /// <param name="client"></param>
        public LobbyScreen(NetClient client)
            : base("Lobby")
        {
            _characterMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();
            var play = new MenuEntry("Play");
            var back = new MenuEntry("Back");
            _firstPlayer = new MenuEntry("First Player: Not Connected");
            _secondPlayer = new MenuEntry("Second Player: Not Connected");
            _thirdPlayer = new MenuEntry("Third Player: Not Connected");
            _fourthPlayer = new MenuEntry("Fourth Player: Not Connected");
            _characterMenuEntry.Selected += _characterMenuEntry_Selected;
            back.Selected += back_Selected;
            play.Selected += play_Selected;
            MenuEntries.Add(_characterMenuEntry);
            MenuEntries.Add(_firstPlayer);
            MenuEntries.Add(_secondPlayer);
            MenuEntries.Add(_thirdPlayer);
            MenuEntries.Add(_fourthPlayer);
            MenuEntries.Add(play);
            MenuEntries.Add(back);

            _menuEntryArray = new[] { _firstPlayer, _secondPlayer, _thirdPlayer, _fourthPlayer };

            Client = client;
            var serverTask = new Task(ConnectToServer, _serverTaskCancelSource.Token);
            serverTask.Start();
        }

        private void SendNameToServer()
        {
            var om = Client.CreateMessage();
            om.Write("Player " + playerCount);
            Client.SendMessage(om, NetDeliveryMethod.Unreliable);
        }

        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            Client.Shutdown("Disconnect by user");
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
                SendNameToServer();
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
                            var who = msg.ReadInt64();
                            var p2 = msg.ReadString();
                            _names[who] = p2;

                            break;
                        case NetIncomingMessageType.WarningMessage:

                            break;

                    }
                }

                playerCount = _names.Count;

                var i = 0;

                foreach (var p in _names)
                {
                    _menuEntryArray[i].Text = p.Value.ToString();
                    i++;
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
