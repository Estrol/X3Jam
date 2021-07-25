using Estrol.X3Jam.Database.SQLResult;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Utility.ClientData.Enums;
using Estrol.X3Jam.Utility.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Database.Providers {
    public abstract class ProviderBase {
        public Configuration config;
        public ItemList[] m_ItemList;

        public ProviderBase(ItemList[] list, Configuration conf) {
            config = conf;
            m_ItemList = list;
        }

        /// <summary>
        /// When overridden in a derived class, starts the database and executing table creating else throw exception
        /// </summary>
        public virtual void Start() {
            throw new NotImplementedException("Method 'Start' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, stop the database else throw exception
        /// </summary>
        public virtual void Stop() {
            throw new NotImplementedException("Method 'Stop' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, register the account else throw exception
        /// </summary>
        public virtual RegisterInformation RegisterAccount(string username, string nickname, string gender, string password, string email) {
            throw new NotImplementedException("Method 'RegisterAccount' not yet implemented");
        }

        /// <sumary>
        /// When overridden in a derived class, return all registered users information (without password)
        /// </sumary>
        public virtual GetUsersInformation GetUsers() {
            throw new NotImplementedException("Method 'GetUsers' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, get login credentials else throw exception
        /// </summary>
        public virtual LoginInformation QueryLoginInfo(string username, string password) {
            throw new NotImplementedException("Method 'QueryLoginInfo' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, check if user exists else throw exception
        /// </summary>
        public virtual ExistsInformation QueryExistsUser(string username, string email) {
            throw new NotImplementedException("Method 'QueryExistsUser' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, get the user's character else throw exception
        /// </summary>
        public virtual CharacterInformation QueryCharacter(string username) {
            throw new NotImplementedException("Method 'QueryCharacter' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, update the user's character in database else throw exception
        /// </summary>
        public virtual bool UpdateCharacter(string username, int itemId, CharacterRenderSlot pos) {
            throw new NotImplementedException("Method 'UpdateCharacter' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, get the user's inventory else throw exception
        /// </summary>
        public virtual InventoryInformation QueryInventory(string username) {
            throw new NotImplementedException("Method 'QueryInventory' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, modify user's inventory else throw exception
        /// </summary>
        public virtual void InsertInventory(string username, int slot, int itemId, int amount) {
            throw new NotImplementedException("Method 'InsertInventory' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, get account count in database else throw exception
        /// </summary>
        public virtual int QueryPlayerCount() {
            throw new NotImplementedException("Method 'QueryPlayerCount' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class, generate key to verify invitation code else throw exception
        /// </summary>
        public virtual string GenerateInviteCode() {
            throw new NotImplementedException("Method 'GenerateInvideCode' not yet implemented");
        }

        /// <summary>
        /// When overridden in a derived class,verify code if valid else throw exception
        /// </summary>
        public virtual bool VerifyInviteCode(string key) {
            throw new NotImplementedException("Method 'VerifyInviteCode' not yet implemented");
        }

        public virtual int[] GetDefaultAvatar(string gender) {
            if (gender == "Male") {
                gender = "MCharacters";
            } else {
                gender = "FCharacters";
            }

            string rawData = config.ini.IniReadValue("USER", gender);
            List<string> rawData2 = rawData.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            int[] data = rawData2.Select(x => int.Parse(x)).ToArray();

            return data;
        }

        public virtual int[][] GetDefaultInventoryItem() {
            string rawData = config.ini.IniReadValue("USER", "Items");
            string[] rawData2 = rawData.Split('|').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            List<int[]> data = new();
            foreach (string raw in rawData2) {
                string rawItr = raw.Replace("{", "")
                    .Replace("}", "")
                    .Replace("\n", "")
                    .Replace("\r", "");

                int[] itr = rawItr.Split(',')
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => int.Parse(x))
                    .ToArray();

                data.Add(itr);
            }

            return data.ToArray();
        }

        public static int[] ParseSQLRows(string data) {
            string[] parsed = data.Split('|').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return new int[] {
                int.Parse(parsed[0].Replace("Id", "").Trim()),
                int.Parse(parsed[1].Replace("Count", "").Trim()),
                int.Parse(parsed[2].Replace("INV", "").Trim())
            };
        }
    }
}
