﻿#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using OpenTK.Graphics.ES10;

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

        private bool host;

        Dictionary<long, string> _names = new Dictionary<long, string>();
        public NetClient Client;

        MenuEntry play = new MenuEntry("Play");
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

            host = true;
            _characterMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();

            var back = new MenuEntry("Back");
            _firstPlayer = new MenuEntry("First Player: Not Connected");
            _secondPlayer = new MenuEntry("Second Player: Not Connected");
            _thirdPlayer = new MenuEntry("Third Player: Not Connected");
            _fourthPlayer = new MenuEntry("Fourth Player: Not Connected");
            _characterMenuEntry.Selected += _characterMenuEntry_Selected;
            Global.GameInProgress.Exiting += GameInProgress_Exiting;
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

            StartClient(14242, "romero");

        }


        /// <summary>
        /// Joining an existing lobby
        /// </summary>
        /// <param name="client">Existing client (created in JoinScreen)</param>
        public LobbyScreen(NetClient client)
            : base("Lobby")
        {
            host = false;
            _characterMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();
            var playTest = new MenuEntry("Play");
            var back = new MenuEntry("Back");
            _firstPlayer = new MenuEntry("First Player: Not Connected");
            _secondPlayer = new MenuEntry("Second Player: Not Connected");
            _thirdPlayer = new MenuEntry("Third Player: Not Connected");
            _fourthPlayer = new MenuEntry("Fourth Player: Not Connected");
            _characterMenuEntry.Selected += _characterMenuEntry_Selected;
            Global.GameInProgress.Exiting += GameInProgress_Exiting;

            back.Selected += back_Selected;
            playTest.Selected += playTest_Selected;
            MenuEntries.Add(_characterMenuEntry);
            MenuEntries.Add(_firstPlayer);
            MenuEntries.Add(_secondPlayer);
            MenuEntries.Add(_thirdPlayer);
            MenuEntries.Add(_fourthPlayer);
            MenuEntries.Add(playTest);
            MenuEntries.Add(back);

            _menuEntryArray = new[] { _firstPlayer, _secondPlayer, _thirdPlayer, _fourthPlayer };

            Client = client;

        }

        void GameInProgress_Exiting(object sender, EventArgs e)
        {
            Client.Disconnect("Client shutdown");
            Client.Shutdown("Client shutdown");
        }

        void playTest_Selected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(_names, Client));
        }



        private void SendNameToServer()
        {
            var om = Client.CreateMessage();

            om.Write(Global.PlayerName);
            om.Write(0);
            om.Write(0);
            om.Write(0.0f);
            if (Client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                Client.SendMessage(om, NetDeliveryMethod.Unreliable);
            }

        }


        void back_Selected(object sender, PlayerIndexEventArgs e)
        {
            Client.Disconnect("Disconnect by user");
            Client.Shutdown("Disconnect by user");
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



        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            var names = new Dictionary<long, string>();
            SendNameToServer();

            NetIncomingMessage msg;
            while ((msg = Client.ReadMessage()) != null)
            {

                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.DiscoveryResponse:
                        //just connect to first server discovered
                        //is the server full
                        var canConnect = msg.ReadBoolean();
                        if (canConnect)
                        {
                            Client.Connect(msg.SenderEndpoint);

                        }
                        else
                        {
                            var confirmExitMessageBox = new MessageBoxScreen("Lobby is full.");
                            ScreenManager.AddScreen(confirmExitMessageBox, PlayerIndex.One);
                            confirmExitMessageBox.Accepted += confirmExitMessageBox_Accepted;
                            confirmExitMessageBox.Cancelled += confirmExitMessageBox_Cancelled;
                            Client.Disconnect("full server");
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        var who = msg.ReadInt64();
                        var p2 = msg.ReadString();

                        names[who] = p2;
                        //_names[who] = p2;
                        _names = names;
                        break;
                    case NetIncomingMessageType.WarningMessage:

                        break;

                }
            }

            var i = 0;

            ClearMenuEntries();

            play.Disabled = _names.Count < 2;

            foreach (var p in _names)
            {
                _menuEntryArray[i].Text = p.Value.ToString();
                i++;
            }


            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }



        private void ClearMenuEntries()
        {
            _firstPlayer.Text = "First Player: Not Connected";
            _secondPlayer.Text = "Second Player: Not Connected";
            _thirdPlayer.Text = "Third Player: Not Connected";
            _fourthPlayer.Text = "Fourth Player: Not Connected";
        }

        void confirmExitMessageBox_Cancelled(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }

        void confirmExitMessageBox_Accepted(object sender, PlayerIndexEventArgs e)
        {
            ExitScreen();
        }


        void play_Selected(object sender, PlayerIndexEventArgs e)
        {

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(_names, Client));
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
