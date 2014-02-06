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
            var playerName = string.Empty;
            List<string> names = new List<string>();


            var config = new NetPeerConfiguration("romero");
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.Port = 14242;

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
                            server.SendDiscoveryResponse(null, msg.SenderEndpoint);
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
                                
                                Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
                                msg.SenderConnection.Tag = "Not yet named";

                            };


                            break;
                        case NetIncomingMessageType.Data:
                            //
                            // The client sent input to the server
                            //
                            playerName = msg.ReadString();
                            msg.SenderConnection.Tag = playerName;
                            // fancy movement logic goes here; we just append input to position

                            break;
                    }

                    //
                    // send position updates 30 times per second
                    //
                    double now = NetTime.Now;
                    if (now > nextSendUpdates)
                    {
                        // Yes, it's time to send position updates

                        // for each player...
                        foreach (NetConnection player in server.Connections)
                        {
                            // ... send information about every other player (actually including self)
                            foreach (NetConnection otherPlayer in server.Connections)
                            {
                                // send position update about 'otherPlayer' to 'player'
                                NetOutgoingMessage om = server.CreateMessage();

                                // write who this position is for
                                om.Write(otherPlayer.RemoteUniqueIdentifier);

                                om.Write(otherPlayer.Tag.ToString());


                                // send message
                                server.SendMessage(om, player, NetDeliveryMethod.Unreliable);
                            }
                        }

                        // schedule next update
                        nextSendUpdates += (1.0 / 30.0);
                    }
                }

                // sleep to allow other processes to run smoothly
                Thread.Sleep(1);
            }

            server.Shutdown("bye");
        }
    }
}
