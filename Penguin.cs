// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Penguin.cs" company="">
//   
// </copyright>
// <summary>
//   The penguin.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PenguinSdk
{
    using System;
    using System.IO;

    using Newtonsoft.Json;

    using EEPhysics;

    using PlayerIOClient;

    /// <summary>
    /// The penguin.
    /// </summary>
    public static class Penguin
    {
        #region Static Fields

        /// <summary>
        /// The world.
        /// </summary>
        private static readonly PhysicsWorld world;

        /// <summary>
        /// The map.
        /// </summary>
        private static PenguinMap map;

        /// <summary>
        /// The tokenizer.
        /// </summary>
        private static Tokenizer tokenizer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="Penguin"/> class.
        /// </summary>
        static Penguin()
        {
            world = new PhysicsWorld();
            world.AddBotPlayer = false;
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The on message for user.
        /// </summary>
        public static event UserMessageHandler OnMessageForUser;

        /// <summary>
        /// The on message parsed.
        /// </summary>
        public static event MessageParsedHandler OnMessageParsed;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The handle message.
        /// </summary>
        /// <param name="m">
        /// The m.
        /// </param>
        public static void HandleMessage(Message m)
        {
            world.HandleMessage(m);
            switch (m.Type)
            {
                case "init":
                    
                    HandleInit(m);
                    tokenizer.Load(map, world);
                    break;
                case "b":
                    if (map != null)
                    {
                        var block = new PenguinBlock(m.GetInt(1), m.GetInt(2), m.GetInt(0), m.GetInt(3), 0);
                        map.AddBlock(block);
                    }

                    break;
                case "say":
                    if (tokenizer != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Processing: " + m.GetString(1));
                        Console.ForegroundColor = ConsoleColor.Gray;

                        tokenizer.HandlePhrase(m.GetInt(0), m.GetString(1), OnMessageForUser);
                    }

                    break;
            }
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public static void Initialize(Config config)
        {
            string jsonRaw = File.ReadAllText("terms.txt");
            var descriptions = JsonConvert.DeserializeObject<KeywordDescription[]>(jsonRaw);
            for (int i = 0; i < descriptions.Length; i++)
            {
                Array.Sort(descriptions[i].BlockIds);
            }

            tokenizer = new Tokenizer(config, descriptions);
            tokenizer.OnMessageParsed += OnMessageParsed;
        }

        /// <summary>
        /// The release.
        /// </summary>
        public static void Release()
        {
            if (tokenizer != null)
            {
                tokenizer.Dispose();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The derot.
        /// </summary>
        /// <param name="arg1">
        /// The arg 1.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string Derot(string arg1)
        {
            int num = 0;
            string str = string.Empty;
            for (int i = 0; i < arg1.Length; i++)
            {
                num = arg1[i];
                if ((num >= 0x61) && (num <= 0x7a))
                {
                    if (num > 0x6d)
                    {
                        num -= 13;
                    }
                    else
                    {
                        num += 13;
                    }
                }
                else if ((num >= 0x41) && (num <= 90))
                {
                    if (num > 0x4d)
                    {
                        num -= 13;
                    }
                    else
                    {
                        num += 13;
                    }
                }

                str = str + ((char)num);
            }

            return str;
        }

        /// <summary>
        /// Handle the initialization of the map.
        /// </summary>
        /// <param name="m">
        /// The initialization message.
        /// </param>
        /// <exception cref="PenguinException">
        /// </exception>
        private static void HandleInit(Message m)
        {
            map = new PenguinMap(m.GetInt(18u), m.GetInt(19u));

            if (m[5u] is string)
            {
                map.WorldKey = Derot(m.GetString(5u));
            }
            else
            {
                map.WorldKey = "b";
            }


            try
            {
                // And now replace empty blocks with the ones that already exist.
                uint messageIndex = 38u;

                // Iterate through each internal set of messages.
                while (messageIndex < m.Count)
                {
                    // If it is a string, exit.
                    if (m[messageIndex] is string)
                    {
                        break;
                    }

                    // The ID is first.
                    int blockId = m.GetInteger(messageIndex);
                    messageIndex++;

                    // Then the z.
                    int z = m.GetInteger(messageIndex);
                    messageIndex++;

                    // Then the list of all X coordinates of given block
                    byte[] xa = m.GetByteArray(messageIndex);
                    messageIndex++;

                    // Then the list of all Y coordinates of given block
                    byte[] ya = m.GetByteArray(messageIndex);
                    messageIndex++;

                    int rotation = 0, note = 0, type = 0, portalId = 0, destination = 0, coins = 0;
                    bool isVisible = false;
                    string roomDestination = string.Empty;
                    string signMessage = string.Empty;

                    // Get the variables that are unique to the current block
                    switch (blockId)
                    {
                        case PenguinIds.Action.Portals.Invisible:
                        case PenguinIds.Action.Portals.Normal:
                            rotation = m.GetInteger(messageIndex);
                            messageIndex++;
                            portalId = m.GetInteger(messageIndex);
                            messageIndex++;
                            destination = m.GetInteger(messageIndex);
                            messageIndex++;
                            isVisible = blockId != PenguinIds.Action.Portals.Invisible;
                            break;
                        case PenguinIds.Action.Portals.World:
                            roomDestination = m.GetString(messageIndex);
                            messageIndex++;
                            break;
                        case PenguinIds.Action.Sign.Textsign:
                            signMessage = m.GetString(messageIndex);
                            break;
                        case PenguinIds.Action.Gates.Coin:
                        case PenguinIds.Action.Doors.Coin:
                            coins = m.GetInteger(messageIndex);
                            messageIndex++;
                            break;
                        case PenguinIds.Action.Music.Percussion:
                            type = m.GetInteger(messageIndex);
                            messageIndex++;
                            break;
                        case PenguinIds.Action.Music.Piano:
                            note = m.GetInteger(messageIndex);
                            messageIndex++;
                            break;
                        case PenguinIds.Action.Hazards.Spike:
                        case PenguinIds.Decorative.SciFi2013.Orangestraight:
                        case PenguinIds.Decorative.SciFi2013.Orangebend:
                        case PenguinIds.Decorative.SciFi2013.Greenstraight:
                        case PenguinIds.Decorative.SciFi2013.Greenbend:
                        case PenguinIds.Decorative.SciFi2013.Bluestraight:
                        case PenguinIds.Decorative.SciFi2013.Bluebend:
                            rotation = m.GetInteger(messageIndex);
                            messageIndex++;
                            break;
                    }

                    // Some variables to simplify things.
                    for (int pos = 0; pos < ya.Length; pos += 2)
                    {
                        // Extract the X and Y positions from the array.
                        int x = (xa[pos] * 256) + xa[pos + 1];
                        int y = (ya[pos] * 256) + ya[pos + 1];

                        // Ascertain the block from the ID.
                        // Add block accordingly.
                        switch (blockId)
                        {
                            case PenguinIds.Action.Portals.Invisible:
                            case PenguinIds.Action.Portals.Normal:
                                map.AddBlock(new PenguinPortal(x, y, rotation, portalId, destination, isVisible));
                                break;
                            case PenguinIds.Action.Portals.World:
                                map.AddBlock(new PenguinRoomPortal(x, y, roomDestination));
                                break;
                            case PenguinIds.Action.Gates.Coin:
                                map.AddBlock(new PenguinCoinGate(x, y, coins));
                                break;
                            case PenguinIds.Action.Doors.Coin:
                                map.AddBlock(new PenguinCoinDoor(x, y, coins));
                                break;
                            case PenguinIds.Action.Music.Percussion:
                                map.AddBlock(new PenguinPercussion(x, y, type));
                                break;
                            case PenguinIds.Action.Music.Piano:
                                map.AddBlock(new PenguinPiano(x, y, note));
                                break;
                            case PenguinIds.Action.Sign.Textsign:
                                map.AddBlock(new PenguinText(x, y, signMessage));
                                break;
                            default:
                                map.AddBlock(new PenguinBlock(x, y, z, blockId, rotation));
                                break;
                        }
                    }
                }
            }
            catch
            {
                throw new PenguinException(
                    "Init deserialization is out of date. Try getting the newest version of Penguin.");
            }
        }

        #endregion
    }
}