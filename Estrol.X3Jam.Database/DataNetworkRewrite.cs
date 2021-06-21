using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Database.Providers;
using Estrol.X3Jam.Database.SQLResult;

using System;
using System.IO;
using System.Reflection;
using Estrol.X3Jam.Utility.Parser;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility.Data;

namespace Estrol.X3Jam.Database {
    public class DataNetworkRewrite {
        private Configuration config;
        private ProviderManager managers;
        private ProviderBase client;
        private bool m_ready;
        private ItemList[] m_ItemLists;

        public DataNetworkRewrite() {
            Log.Write("Itemlist Database Loaded!");
            managers = new();
            config = new();

            string db_name = config.ini.IniReadValue("DATABASE", "SQLProviderName");
            Type asm = managers.Get(db_name);
            if (asm == null) {
                throw new InvalidDataException($"Database provider with name '{db_name}' is not registered");
            }

            ConstructorInfo ctor = asm.GetConstructor(new[] { typeof(ItemList[]), typeof(Configuration) });
            client = (ProviderBase)ctor.Invoke(new object[] { m_ItemLists, config });

            m_ItemLists = ItemListParser.LoadData(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "Itemdata.dat"));
        }

        public void Start() {
            Log.Write("Database Loaded!");
            m_ready = true;
            client.Start();
        }

        public void Close() {
            Log.Write("Database Manager Closing!");
            client.Stop();
        }

        public int PlayerCount {
            get {
                if (!m_ready) {
                    throw new Exception("DataNetwork is not ready! Please invoke .Intialized first!");
                }

                return client.QueryPlayerCount();
            }
        }

        public ExistResult Exists(string username, string email) {
            ExistsInformation result = client.QueryExistsUser(username, email);

            return new() {
                IsExist = result.IsExist,
                Reason = result.Message
            };
        }

        public void Register(string username, string password, string email) {
            username = username.ToLower();

            RegisterInformation result = client.RegisterAccount(username, username, "Female", password, email);
            if (!result.IsSuccess) {
                throw new Exception(result.Message);
            }
        }

        public User Login(string username, string password) {
            username = username.ToLower();

            LoginInformation result = client.QueryLoginInfo(username, password);
            if (result.IsExist) {
                Character character = GetChar(username);

                return new(new string[] { result.Username, result.Nickname }, character);
            }

            return null;
        }

        public Item[] GetInventory(string username) {
            InventoryInformation result = client.QueryInventory(username);

            Item[] inventory = new Item[35];
            for (int i = 0; i < result.Data.Length; i++) {
                int[] data = ProviderBase.ParseSQLRows(result.Data[i]);
                ItemList item_info = Array.Find(m_ItemLists, itr => itr.Id == data[0]);

                Item item = new() {
                    ItemId = data[0],
                    ItemCount = data[1],
                    Function = item_info.ItemFunction
                };

                inventory[i] = item;
            }

            return inventory;
        }

        public void SetInventory(string username, int slot, int itemId, int amount) {
            client.InsertInventory(username, slot, itemId, amount);
        }

        public Character GetChar(string username) {
            CharacterInformation result = client.QueryCharacter(username);

            Character character = new() {
                Username = result.Username,
                Nickname = result.Nickname,
                Rank = result.Rank,
                Level = result.Level,
                Gender = result.Gender,
                MCash = result.MCash,
                Gold = result.Gold,
                Wins = result.Wins,
                Loses = result.Loses,
                Scores = result.Scores,
                
                // Avatar
                Instrument = result.Data[0],
                Hair = result.Data[1],
                Accessory = result.Data[2],
                Glove = result.Data[3],
                Necklace = result.Data[4],
                Cloth = result.Data[5],
                Pant = result.Data[6],
                Glass = result.Data[7],
                Earring = result.Data[8],
                Shoe = result.Data[9],
                Face = result.Data[10],
                Wing = result.Data[11],
                HairAccessory = result.Data[12],
                InstrumentAccessory = result.Data[13],
                ClothAccessory = result.Data[14],
                Pet = result.Data[15],
            };

            return character;
        }

        public class ExistResult {
            public bool IsExist { set; get; }
            public string Reason { set; get; }
        }
    }
}
