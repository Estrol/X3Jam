using Estrol.X3Jam.Server;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Database.SQLResult;

using System;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using Estrol.X3Jam.Utility.Parser;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Utility.ClientData.Enums;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// The purpose of this class to make my implementation of local database using SQLite
/// and using universal database connection (or Odbc or whatever called it) to make it easier
/// </summary>

namespace Estrol.X3Jam.Database.Providers {
    public class SQLiteProvider : ProviderBase {
        public SQLiteConnection SQLiteClient { get; private set; }
        public SQLiteProvider(ItemList[] list, Configuration config) : base(list, config) {
            SQLiteConnectionStringBuilder stringBuilder = new() {
                DataSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "O2JamDatabase.db"),
                Version = 3,
                PageSize = 4096,
                CacheSize = 10000,
                JournalMode = SQLiteJournalModeEnum.Wal,
                Pooling = true,
                LegacyFormat = false,
                DefaultTimeout = 500
            };

            SQLiteClient = new(stringBuilder.ConnectionString);
        }

        public override void Start() {
            SQLiteClient.Open();

            SQLiteCommand cm = new(SQLiteClient);

            cm.CommandText = "CREATE TABLE IF NOT EXISTS X3JAM_users_invite(Id INTEGER PRIMARY KEY, InviteCode TEXT)";
            cm.ExecuteNonQuery();

            cm.CommandText = "CREATE TABLE IF NOT EXISTS X3JAM_users_info(Id INTEGER PRIMARY KEY, Username TEXT, Nickname TEXT, Email TEXT, Gender TEXT, Password TEXT)";
            cm.ExecuteNonQuery();

            cm.CommandText = "CREATE TABLE IF NOT EXISTS X3JAM_users_inventory(Id INTEGER PRIMARY KEY, Username TEXT";
            for (int i = 0; i < 35; i++)
                cm.CommandText += $", Inventory{i} TEXT";

            cm.CommandText += ")";
            cm.ExecuteNonQuery();

            cm.CommandText = "CREATE TABLE IF NOT EXISTS X3JAM_users_character(Id INTEGER PRIMARY KEY," +
                "Username TEXT," + "Nickname TEXT," + "Level INTEGER," +
                "Rank INTEGER," + "Gender INTEGER," + "MCash INTEGER," +
                "Gold INTEGER," + "Wins INTEGER," + "Loses INTEGER," +
                "Scores INTEGER," + "Instrument INTEGER," + "Hair INTEGER," +
                "Accessory INTEGER," + "Glove INTEGER," + "Necklace INTEGER," +
                "Cloth INTEGER," + "Pant INTEGER," + "Glass INTEGER," +
                "Earring INTEGER," + "Shoe INTEGER," + "Face INTEGER," +
                "Wing INTEGER, " + "HairAccessory INTEGER," + "InstrumentAccessory INTEGER," +
                "ClothAccessory INTEGER," + "Pet INTEGER)";
            cm.ExecuteNonQuery();
        }

        public override void Stop() {
            SQLiteClient.Close();
        }

        public override RegisterInformation RegisterAccount(string username, string nickname, string gender, string password, string email) {
            if (password.Length > 65) {
                return new() {
                    IsSuccess = false,
                    Message = "Password length cannot more than 65 characters"
                };
            }

            string[] Genders = { "Male", "Female" };

            if (!Genders.Contains(gender)) {
                return new() {
                    IsSuccess = false,
                    Message = "Invalid character gender"
                };
            }

            password = BCrypt.Net.BCrypt.HashPassword(password);
            SQLiteCommand cmd = new(SQLiteClient);

            cmd.CommandText = "INSERT INTO X3JAM_users_info(Username, Nickname, Gender, Email, Password) VALUES(?,?,?,?,?)";
            cmd.Parameters.AddWithValue("Username", username);
            cmd.Parameters.AddWithValue("Nickname", nickname);
            cmd.Parameters.AddWithValue("Gender", gender);
            cmd.Parameters.AddWithValue("Email", email);
            cmd.Parameters.AddWithValue("Password", password);

            cmd.ExecuteNonQuery();
            return new() {
                IsSuccess = true,
                Message = "Account registered, you may login now!"
            };
        }

        public override LoginInformation QueryLoginInfo(string username, string password) {
            if (password.Length > 65) {
                return new() {
                    IsExist = false,
                    Message = "Password length cannot more than 65 characters"
                };
            }

            LoginInformation info = new();
            info.IsExist = false;

            SQLiteCommand cmd = new(SQLiteClient);
            cmd.CommandText = "SELECT * FROM X3JAM_users_info WHERE Username = ?";
            cmd.Parameters.AddWithValue("Username", username);
            var reader = cmd.ExecuteReader();

            while (reader.Read()) {
                bool result = BCrypt.Net.BCrypt.Verify(password, (string)reader["Password"]);
                if (result) {
                    info.IsExist = true;
                    info.Username = (string)reader["Username"];
                    info.Nickname = (string)reader["Nickname"];
                }
            }

            return info;
        }

        public override GetUsersInformation GetUsers() {
            SQLiteCommand cmd = new(SQLiteClient);
            cmd.CommandText = "SELECT * FROM X3JAM_users_info";

            List<User> users = new();
            var reader = cmd.ExecuteReader();

            while (reader.Read()) {
                var characterInfo = QueryCharacter((string)reader["Username"]);
                Character character = new() {
                    Username = (string)reader["Username"],
                    Nickname = (string)reader["Nickname"],
                    Level = characterInfo.Level,
                    Wins = characterInfo.Wins,
                    Loses = characterInfo.Loses,
                    MCash = characterInfo.MCash,
                    Gold = characterInfo.Gold
                };

                users.Add(new User(new string[] { (string)reader["Username"], (string)reader["Nickname"] }, character, null));
            }

            return new() {
                Users = users.ToArray()
            };
        }

        public override int QueryPlayerCount() {
            SQLiteCommand cmd = new(SQLiteClient);
            cmd.CommandText = "SELECT COUNT(Id) FROM X3JAM_users_info";

            int RowCount = Convert.ToInt32(cmd.ExecuteScalar());
            return RowCount;
        }

        public override ExistsInformation QueryExistsUser(string username, string email) {
            SQLiteCommand cmd = new(SQLiteClient);

            username = username.ToLower();
            cmd.CommandText = "SELECT * FROM X3JAM_users_info WHERE Username = ?";
            SQLiteCommand cm = (SQLiteCommand)cmd;
            cm.Parameters.AddWithValue("Username", username);

            var reader = cmd.ExecuteReader();
            while (reader.Read()) {
                return new() {
                    IsExist = true,
                    Message = "Another user with this username already exist"
                };
            }

            reader.Close();
            if (email.Length == 0) {
                try {
                    MailAddress mail = new(email);
                    Dns.GetHostAddresses(mail.Host);
                } catch (Exception e) {
                    if (e is FormatException) {
                        return new() {
                            IsExist = true,
                            Message = "Invalid Email Format!"
                        };
                    }

                    if (e is SocketException) {
                        return new() {
                            IsExist = true,
                            Message = "Unable to resolve Email Domain!"
                        };
                    }

                    throw;
                }

                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT * FROM X3JAM_users_info WHERE Email = ?";
                cmd.Parameters.AddWithValue("Email", email);

                reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    return new() {
                        IsExist = true,
                        Message = "Another user with this email already exist"
                    };
                }
            }

            return new() {
                IsExist = false,
                Message = "<null>"
            };
        }

        public override CharacterInformation QueryCharacter(string username) {
            CharacterInformation item = new();
            item.Data = new int[16];

            SQLiteCommand cmd = new(SQLiteClient);

            LoginInformation login = null;
            cmd.CommandText = "SELECT * FROM X3JAM_users_info WHERE Username = ?";
            cmd.Parameters.AddWithValue("Username", username);

            DbDataReader reader0 = cmd.ExecuteReader();
            while (reader0.Read()) {
                login = new() {
                    IsExist = true,
                    Username = (string)reader0["Username"],
                    Nickname = (string)reader0["Nickname"],
                    Gender = (string)reader0["Gender"],
                };
            }

            if (login == null) {
                return new() {
                    IsSuccess = false,
                };
            }

            reader0.Close();
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT * FROM X3JAM_users_character WHERE Username = ?";
            cmd.Parameters.AddWithValue("Username", username);

            DbDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                for (int i = 0; i < 16; i++) {
                    item.Data[i] = reader.GetInt32((i + 10) + 1);
                }

                item.IsSuccess = true;
                item.Username = username;
                item.Nickname = (string)reader["Nickname"];
                item.Rank = Convert.ToInt32((long)reader["Rank"]);
                item.Level = Convert.ToInt32((long)reader["Level"]);
                item.Gender = Convert.ToInt32((long)reader["Gender"]);
                item.MCash = Convert.ToInt32((long)reader["MCash"]);
                item.Gold = Convert.ToInt32((long)reader["Gold"]);
                item.Wins = Convert.ToInt32((long)reader["Wins"]);
                item.Loses = Convert.ToInt32((long)reader["Loses"]);
                item.Scores = Convert.ToInt32((long)reader["Scores"]);

                return item;
            }

            reader.Close();
            cmd.Parameters.Clear();
            cmd.CommandText = "INSERT INTO X3JAM_users_character("
                    + "Username,"
                    + "Nickname,"
                    + "Rank,"
                    + "Level,"
                    + "Gender,"
                    + "MCash,"
                    + "Gold,"
                    + "Wins,"
                    + "Loses,"
                    + "Scores,"
                    + "Instrument,"
                    + "Hair,"
                    + "Accessory,"
                    + "Glove,"
                    + "Necklace,"
                    + "Cloth,"
                    + "Pant,"
                    + "Glass,"
                    + "Earring,"
                    + "Shoe,"
                    + "Face,"
                    + "Wing,"
                    + "HairAccessory,"
                    + "InstrumentAccessory,"
                    + "ClothAccessory,"
                    + "Pet) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

            int gender = 0;
            if (login.Gender == "Male") gender = 1;

            int mcash = int.Parse(config.ini.IniReadValue("USER", "MCash"));
            int gold = int.Parse(config.ini.IniReadValue("USER", "Gold"));
            int[] chars = GetDefaultAvatar(login.Gender);

            cmd.Parameters.AddWithValue("Username", login.Username);
            cmd.Parameters.AddWithValue("Nickname", login.Nickname);
            cmd.Parameters.AddWithValue("Rank", 0);
            cmd.Parameters.AddWithValue("Level", 1);
            cmd.Parameters.AddWithValue("Gender", gender);
            cmd.Parameters.AddWithValue("MCash", mcash);
            cmd.Parameters.AddWithValue("Gold", gold);
            cmd.Parameters.AddWithValue("Wins", 0);
            cmd.Parameters.AddWithValue("Loses", 0);
            cmd.Parameters.AddWithValue("Scores", 0);
            cmd.Parameters.AddWithValue("Instrument", chars[0]);
            cmd.Parameters.AddWithValue("Hair", chars[1]);
            cmd.Parameters.AddWithValue("Accessory", chars[2]);
            cmd.Parameters.AddWithValue("Glove", chars[3]);
            cmd.Parameters.AddWithValue("Necklace", chars[4]);
            cmd.Parameters.AddWithValue("Cloth", chars[5]);
            cmd.Parameters.AddWithValue("Pant", chars[6]);
            cmd.Parameters.AddWithValue("Glass", chars[7]);
            cmd.Parameters.AddWithValue("Earring", chars[8]);
            cmd.Parameters.AddWithValue("Shoe", chars[9]);
            cmd.Parameters.AddWithValue("Face", chars[10]);
            cmd.Parameters.AddWithValue("Wing", chars[11]);
            cmd.Parameters.AddWithValue("HairAccessory", chars[12]);
            cmd.Parameters.AddWithValue("InstrumentAccessory", chars[13]);
            cmd.Parameters.AddWithValue("ClothAccessory", chars[14]);
            cmd.Parameters.AddWithValue("Pet", chars[15]);

            cmd.ExecuteNonQuery();

            item.IsSuccess = true;
            item.Username = username;
            item.Nickname = login.Nickname;
            item.Rank = 0;
            item.Level = 1;
            item.Gender = gender;
            item.MCash = mcash;
            item.Gold = gold;
            item.Wins = 0;
            item.Loses = 0;
            item.Scores = 0;

            item.Data = chars;

            return item;
        }

        public override bool UpdateCharacter(string username, int itemId, CharacterRenderSlot pos) {
            string ColumnName = Enum.GetName(typeof(CharacterRenderSlot), pos);

            SQLiteCommand cmd = new(SQLiteClient);
            cmd.CommandText = $"UPDATE X3JAM_users_character set {ColumnName} = :{ColumnName} WHERE Username = :Username";
            cmd.Parameters.AddWithValue("Username", username);
            cmd.Parameters.AddWithValue(ColumnName, itemId);

            cmd.ExecuteNonQuery();
            return true;
        }

        public override string GenerateInviteCode() {
            SQLiteCommand cmd = new(SQLiteClient);
            cmd.CommandText = "INSERT INTO X3JAM_users_invite(InviteCode) VALUES(?)";

            RNGCryptoServiceProvider crypto = new();
            byte[] data = new byte[15];
            crypto.GetBytes(data);

            string cryptString = Convert.ToBase64String(data);
            cmd.Parameters.AddWithValue("InviteCode", cryptString);
            cmd.ExecuteNonQuery();

            return cryptString;
        }

        public override bool VerifyInviteCode(string key) {
            SQLiteCommand cmd = new(SQLiteClient);

            cmd.CommandText = "SELECT InviteCode FROM X3JAM_users_invite WHERE InviteCode = ?";
            cmd.Parameters.AddWithValue("InviteCode", key);

            var reader = cmd.ExecuteReader();
            if (reader.Read()) {
                reader.Close();

                cmd.CommandText = "DELETE FROM X3JAM_users_invite WHERE InviteCode = ?";
                cmd.ExecuteNonQuery();

                return true;
            }

            return false;
        }

        public override InventoryInformation QueryInventory(string username) {
            InventoryInformation item = new();
            item.Data = new string[35];

            SQLiteCommand cmd = new(SQLiteClient);

            cmd.CommandText = "SELECT * FROM X3JAM_users_inventory WHERE Username = ?";
            cmd.Parameters.AddWithValue("Username", username);

            DbDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                for (int i = 0; i < 35; i++) {
                    item.Data[i] = (string)reader[$"Inventory{i}"];
                }

                return item;
            }

            reader.Close();
            cmd.Parameters.Clear();
            cmd.CommandText = "INSERT INTO X3JAM_users_inventory(Username";
            for (int i = 0; i < 35; i++) {
                cmd.CommandText += $", Inventory{i}";
            }

            cmd.CommandText += ") VALUES(?";
            for (int i = 0; i < 35; i++) {
                cmd.CommandText += $", ?";
            }

            cmd.CommandText += ")";

            int[][] defaultItems = GetDefaultInventoryItem();
            cmd.Parameters.AddWithValue("Username", username);
            for (int i = 0; i < 35; i++) {
                int[] itr = defaultItems.ElementAtOrDefault(i);
                if (itr != null) {
                    cmd.Parameters.AddWithValue($"Inventory{i}", $"|Id{itr[0]}|Count{itr[1]}|INV{i}|");
                } else {
                    cmd.Parameters.AddWithValue($"Inventory{i}", $"|Id0|Count0|INV{i}|");
                }
            }

            cmd.ExecuteNonQuery();
            item.IsSuccess = true;

            for (int i = 0; i < 35; i++) {
                int[] itr = defaultItems.ElementAtOrDefault(i);
                if (itr != null) {
                    item.Data[i] = $"|Id{itr[0]}|Count{itr[1]}|INV{i}|";
                } else {
                    item.Data[i] = $"|Id0|Count0|INV{i}|";
                }
            }

            return item;
        }

        public override void InsertInventory(string username, int slot, int itemId, int amount) {
            SQLiteCommand cmd = new SQLiteCommand(SQLiteClient);
            string format = "|Id{0}|Count{1}|INV{2}|"; // 0 = ItemId, 1 = ItemCount, 2 = slot
            string parsed = string.Format(format, itemId, amount, slot);
            cmd.CommandText = $"UPDATE X3JAM_users_inventory SET Inventory{slot} = :inv{slot} WHERE Username = :name";
            cmd.Parameters.AddWithValue("name", username);
            cmd.Parameters.AddWithValue($"inv{slot}", parsed);

            cmd.ExecuteNonQuery();
        }

        private (int, string) FindSlot(string username, int itemId) {
            SQLiteCommand cmd = new(SQLiteClient);

            Regex regex = new(string.Format("/Id{0}/g", itemId));
            int slot = -1;
            string value = "";

            cmd.CommandText = "SELECT * FROM X3JAM_users_inventory WHERE Username = ?";
            cmd.Parameters.AddWithValue("Username", username);

            DbDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                for (var i = 0; i < 35; i++) {
                    var _value = (string)reader[$"Inventory{i}"];
                    if (regex.IsMatch(_value)) {
                        slot = i;
                        value = _value;
                        break;
                    }
                }
            }

            return (slot, value);
        }
    }
}
