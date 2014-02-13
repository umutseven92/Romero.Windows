#region Using Statements

using System;
using System.Collections.Generic;
using System.Threading;
using Lidgren.Network;

#endregion

namespace Romero.Windows.Server
{
    class Program
    {
        static void Main()
        {
            var connectedPlayers = 0;

            var config = new NetPeerConfiguration("romero");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;

            //var xinput = 0;
            //var yinput = 0;
            var dummyName = string.Empty;
            float angle = 0;

            // create and start server
            var server = new NetServer(config);
            server.Start();
           
            // schedule initial sending of position updates
            var nextSendUpdates = NetTime.Now;

            // run until escape is pressed
            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage msg;
                while ((msg = server.ReadMessage()) != null)
                {


                    switch (msg.MessageType)
                    {
                        case NetIncomingMessageType.DiscoveryRequest:
                            //
                            // Server received a discovery request from a client; send a discovery response (with no extra data attached)
                            //
                            var om = server.CreateMessage();

                            if (connectedPlayers < 4)
                            {
                                om.Write(true);
                                server.SendDiscoveryResponse(om, msg.SenderEndpoint);

                            }
                            else
                            {
                                om.Write(false);
                                server.SendDiscoveryResponse(om, msg.SenderEndpoint);
                            }

                            break;
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            //
                            // Just print diagnostic messages to console
                            //
                            Console.WriteLine(msg.ReadString());
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            var status = (NetConnectionStatus)msg.ReadByte();

                            if (status == NetConnectionStatus.Connected)
                            {
                                //
                                // A new player just connected!
                                //

                                Console.WriteLine(msg.SenderConnection.RemoteUniqueIdentifier + " connected.");
                                connectedPlayers++;
                                Console.WriteLine(connectedPlayers + " players connected.");
                            };
                            if (status == NetConnectionStatus.Disconnected)
                            {
                                Console.WriteLine(msg.SenderConnection.RemoteUniqueIdentifier + " disconnected.");

                                connectedPlayers--;
                            }

                            break;
                        case NetIncomingMessageType.Data:
                            //
                            // The client sent input to the server
                            //

                            //Get name here - bind it to tag

                            //  dummyName = msg.ReadString();

                            // fancy movement logic goes here

                            var xinput = msg.ReadInt32();
                            var yinput = msg.ReadInt32();
                            var playerAngle = msg.ReadFloat();
                            angle = playerAngle;
                            var pos = msg.SenderConnection.Tag as int[];
                            pos[0] = xinput;
                            pos[1] = yinput;
                            break;

                    }

                    //
                    // send position updates 30 times per second
                    //
                    var now = NetTime.Now;
                    if (now > nextSendUpdates)
                    {
                        // Yes, it's time to send position updates

                        // for each player...


                        foreach (var player in server.Connections)
                        {
                            // ... send information about every other player (actually including self)
                            foreach (var otherPlayer in server.Connections)
                            {
                                // send position update about 'otherPlayer' to 'player'
                                var om = server.CreateMessage();
                                if (otherPlayer != null)
                                {
                                    // write who this position is for
                                    om.Write(otherPlayer.RemoteUniqueIdentifier);

                                    om.Write(otherPlayer.RemoteUniqueIdentifier.ToString());

                                    if (otherPlayer.Tag == null)
                                        otherPlayer.Tag = new int[2];

                                    var pos = otherPlayer.Tag as int[];
                                    om.Write(pos[0]);
                                    om.Write(pos[1]);
                                    om.Write(angle);
                                    //om.Write(xinput);
                                    //om.Write(yinput);



                                    // send message
                                    server.SendMessage(om, player, NetDeliveryMethod.Unreliable);
                                }

                            }
                        }

                        // schedule next update
                        nextSendUpdates += (1.0 / 60.0);
                    }
                }

                // sleep to allow other processes to run smoothly
                Thread.Sleep(1);
            }

            server.Shutdown("bye");
        }
    }
}
