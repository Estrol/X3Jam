using System;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Security.Cryptography;

using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Server.CData;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Estrol.X3Jam.Server.CNetwork {
    public class DataNetwork {
        private SQLiteConnection m_db;
        private SHA256Managed m_sha256;
        private RNGCryptoServiceProvider m_rng;

        private bool m_ready;

        public DataNetwork() {
            SQLiteConnectionStringBuilder config = new() {
                DataSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "O2JamDatabase.db"),
                Version = 3,
                PageSize = 4096,
                CacheSize = 10000,
                JournalMode = SQLiteJournalModeEnum.Wal,
                Pooling = true,
                LegacyFormat = false,
                DefaultTimeout = 500
            };

            m_db = new SQLiteConnection(config.ToString());
            m_rng = new RNGCryptoServiceProvider();
            m_sha256 = new SHA256Managed();
            m_ready = false;

            m_db.Open();
        }

        public void Close() {
            Log.Write("Database Manager Closing!");
            m_db.Close();
        }

        public void Intialize() {
            var cm = new SQLiteCommand(m_db) {
                CommandText = "CREATE TABLE IF NOT EXISTS users(id INTEGER PRIMARY KEY, username TEXT, password TEXT, email TEXT, salt TEXT, nickname TEXT, master BOOLEAN)"
            };
            cm.ExecuteNonQuery();

            cm.CommandText = "CREATE TABLE IF NOT EXISTS users_info(id INTEGER PRIMARY KEY," +
                "Username TEXT," +
                "Nickname TEXT," +
                "Level INTEGER," +
                "Rank INTEGER," +
                "Gender INTEGER," +
                "MCash INTEGER," +
                "Gold INTEGER," +
                "Wins INTEGER," +
                "Loses INTEGER," +
                "Scores INTEGER," +
                "Instrument INTEGER," +
                "Hair INTEGER," +
                "Accessory INTEGER," +
                "Glove INTEGER," +
                "Necklace INTEGER," +
                "Cloth INTEGER," +
                "Pant INTEGER," +
                "Glass INTEGER," +
                "Earring INTEGER," +
                "Shoe INTEGER," +
                "Face INTEGER," +
                "Wing INTEGER, " +
                "HairAccessory INTEGER," +
                "InstrumentAccessory INTEGER," +
                "ClothAccessory INTEGER," +
                "Pet INTEGER)";
            cm.ExecuteNonQuery();

            string InvCMD = "CREATE TABLE IF NOT EXISTS users_inventory(id INTEGER PRIMARY KEY, Username TEXT";
            for (int i = 0; i < 35; i++) {
                InvCMD += $", Inventory{i} TEXT";
            }

            InvCMD += ")";
            cm.CommandText = InvCMD;
            cm.ExecuteNonQuery();

#if DEBUG
            cm.CommandText = "INSERT OR IGNORE INTO users(id, username, password, email, salt, nickname, master) " +
                "VALUES(1, \"test\", \"eSiv8purm6ri2LXKk1B4aNIiRUv9JVdOx/DH8pbSztM=\", \"test@localhost\", \"FFAABBCCDDEE\", \"test\", true)";
            cm.ExecuteNonQuery();

            cm.CommandText = "INSERT OR IGNORE INTO users(id, username, password, email, salt, nickname, master) " +
                "VALUES(2, \"test2\", \"ELcELOpVeTXUA4+w2N0SwD1CbcU5l+wNRPxQhWfXzaY=\", \"test2@localhost\", \"FFAABBBBBBEE\", \"test\", true)";
            cm.ExecuteNonQuery();
#endif

            m_ready = true;
            Log.Write("Database Manager Loaded!");
        }

        public int PlayerCount {
            get {
                if (!m_ready) {
                    throw new Exception("DataNetwork is not ready! Please invoke .Intialized first!");
                }

                var cm = new SQLiteCommand(m_db) {
                    CommandText = "SELECT COUNT(id) FROM users"
                };

                int RowCount = Convert.ToInt32(cm.ExecuteScalar());
                return RowCount;
            }
        }

        public ExistResult Exists(string username, string email) {
            if (!m_ready) {
                throw new Exception("DataNetwork is not ready! Please invoke .Intialized first!");
            }

            username = username.ToLower();

            var cm = new SQLiteCommand(m_db) {
                CommandText = "SELECT * FROM users WHERE username = ?"
            };

            cm.Parameters.Add(new SQLiteParameter("username", username));
            var dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read()) {
                return new() { 
                    IsExist = true,
                    Reason = "Another User with that username already exist!"
                };
            }

            try {
                MailAddress mail = new(email);
                Dns.GetHostAddresses(mail.Host);
            } catch (Exception e) {
                if (e is FormatException) {
                    return new() {
                        IsExist = true,
                        Reason = "Invalid Email Format!"
                    };
                }

                if (e is SocketException) {
                    return new() {
                        IsExist = true,
                        Reason = "Unable to resolve Email Domain!"
                    };
                }

                throw;
            }

            cm = new SQLiteCommand(m_db) {
                CommandText = "SELECT * FROM users WHERE email = ?"
            };

            cm.Parameters.Add(new SQLiteParameter("email", email));
            var dr2 = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr2.Read()) {
                return new() {
                    IsExist = true,
                    Reason = "Another User with that email already exist!"
                };
            }

            return new() {
                IsExist = false,
                Reason = "<null>"
            };
        }

        public void Register(string username, string password, string email) {
            if (!m_ready) {
                throw new Exception("DataNetwork is not ready! Please invoke .Intialized first!");
            }

            username = username.ToLower();

            byte[] salt = new byte[6];
            m_rng.GetBytes(salt);

            byte[] pswd = Encoding.UTF8.GetBytes(password);
            pswd = pswd.Concat(salt).ToArray();

            byte[] hashed_password = m_sha256.ComputeHash(pswd);

            var cm = new SQLiteCommand(m_db) {
                CommandText = "INSERT INTO users(username, password, email, salt, nickname, master) VALUES(?,?,?,?,?,?)",
            };

            SQLiteParameter[] parameters = {
                new SQLiteParameter("username", username),
                new SQLiteParameter("password", Convert.ToBase64String(hashed_password)),
                new SQLiteParameter("email", email),
                new SQLiteParameter("salt", P(salt)),
                new SQLiteParameter("nickname", username),
                new SQLiteParameter("master", false)
            };

            for (int i = 0; i < parameters.Length; i++) {
                cm.Parameters.Add(parameters[i]);
            }
            cm.ExecuteNonQuery();
        }

        public User Login(string username, string password) {
            if (!m_ready) {
                throw new Exception("DataNetwork is not ready! Please invoke .Intialized first!");
            }

            username = username.ToLower();

            var cm = new SQLiteCommand(m_db) {
                CommandText = "SELECT * FROM users WHERE username = ?"
            };

            cm.Parameters.Add(new SQLiteParameter("username", username));
            var dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read()) {
                byte[] pswd = Encoding.UTF8.GetBytes(password);
                pswd = pswd.Concat(S((string)dr["salt"])).ToArray();
                byte[] hashed = m_sha256.ComputeHash(pswd);
                string stred = Convert.ToBase64String(hashed);

                if (stred == (string)dr["password"]) {
                    Character character = GetChar(username);

                    User user = new(new string[] {
                        username,
                        (string)dr["nickname"]
                    }, character);

                    return user;
                }
            }

            return null;
        }

        public Item[] GetInventory(string username) {
            var cm = new SQLiteCommand(m_db) {
                CommandText = "SELECT * FROM users_inventory where Username = ?"
            };

            cm.Parameters.Add(new("Username", username));
            var dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read()) {
                Item[] inventoryData = new Item[35];
                for (int i = 0; i < 35; i++) {
                    dynamic[] data = ParseSQLRow((string)dr[$"Inventory{i}"]);

                    Item item = new() {
                        ItemId = data[0],
                        ItemCount = data[1]
                    };

                    var ring = ItemIdRings.Get(item.ItemId);
                    if (ring != null) {
                        item.IsRing = true;
                        item.RingName = ring.Ring;
                    } else {
                        item.IsRing = false;
                        item.RingName = 0;
                    }

                    inventoryData[i] = item;
                }

                return inventoryData;
            }

            var cm2 = new SQLiteCommand(m_db) {
                CommandText = "INSERT INTO users_inventory(Username"
            };

            for (int i = 0; i < 35; i++) {
                cm2.CommandText += $", Inventory{i}";
            }

            cm2.CommandText += ") VALUES(?";

            for (int i = 0; i < 35; i++) {
                switch (i) {
                    case 0: {
                        cm2.CommandText += $",\"|Id{ItemIdRings.Mirror[0]}|Count9999|INV{i}|true|\"";
                        break;
                    }

                    case 1: {
                        cm2.CommandText += $",\"|Id{ItemIdRings.Random[0]}|Count9999|INV{i}|true|\"";
                        break;
                    }

                    case 2: {
                        cm2.CommandText += $",\"|Id{ItemIdRings.Panic[0]}|Count9999|INV{i}|true|\"";
                        break;
                    }

                    case 3: {
                        cm2.CommandText += $",\"|Id{ItemIdRings.Hidden[0]}|Count9999|INV{i}|true|\"";
                        break;
                    }

                    case 4: {
                        cm2.CommandText += $",\"|Id{ItemIdRings.Sudden[0]}|Count9999|INV{i}|true|\"";
                        break;
                    }

                    case 5: {
                        cm2.CommandText += $",\"|Id{ItemIdRings.Dark[0]}|Count9999|INV{i}|true|\"";
                        break;
                    }

                    default: {
                        cm2.CommandText += $",\"|Id0|Count0|INV{i}|false|\"";
                        break;
                    }
                }
            }

            cm2.CommandText += ")";

            cm2.Parameters.Add(new("Username", username));

            cm2.ExecuteNonQuery();

            var result = new Item[35];
            result[0] = new() {
                ItemId = ItemIdRings.Mirror[0],
                ItemCount = 9999,
                RingName = RoomRing.Mirror,
                IsRing = true
            };
            result[1] = new() {
                ItemId = ItemIdRings.Random[0],
                ItemCount = 9999,
                RingName = RoomRing.Random,
                IsRing = true
            };
            result[2] = new() {
                ItemId = ItemIdRings.Panic[0],
                ItemCount = 9999,
                RingName = RoomRing.Panic,
                IsRing = true
            };
            result[3] = new() {
                ItemId = ItemIdRings.Hidden[0],
                ItemCount = 9999,
                RingName = RoomRing.Hidden,
                IsRing = true
            };
            result[4] = new() {
                ItemId = ItemIdRings.Sudden[0],
                ItemCount = 9999,
                RingName = RoomRing.Sudden,
                IsRing = true
            };
            result[5] = new() {
                ItemId = ItemIdRings.Dark[0],
                ItemCount = 9999,
                RingName = RoomRing.Dark,
                IsRing = true
            };

            return result;
        }

        public void UpdateInventory(string username, int slot, int item_id, int amount) {
            var ring = ItemIdRings.Get(item_id);

            string rowFormat = "|Id{0}|Count{1}|Inv{2}|{3}|"; // 0 = ItemId, 1 = ItemCount, 2 = slot, 3 = RingId (if known ring)
            int item_slot = -1;
            string item_value = "";

            if (ring != null) {
                var cm_check = new SQLiteCommand(m_db) {
                    CommandText = "SELECT * FROM users_inventory where USERNAME = ?"
                };

                cm_check.Parameters.Add(new("Username", username));
                var dr = cm_check.ExecuteReader();

                Regex regex = new(string.Format("/Id{0}/g", item_id));

                for (var i = 0; i < dr.FieldCount; i++) {
                    var value = dr.GetString(i);
                    if (regex.IsMatch(value)) {
                        item_slot = i;
                        item_value = value;

                        dr.Close();
                        break;
                    }
                }

                if (item_slot != -1) {
                    goto caseModify;
                } else {
                    goto caseDefault;
                }

            } else {
                goto caseDefault;
            }

            caseModify: {
                dynamic[] data = ParseSQLRow(item_value);
                string parsed = string.Format(rowFormat, data[0], data[1] + amount, string.Format("INV{0}", data[2]), data[3]);

                var cm = new SQLiteCommand(m_db) {
                    CommandText = $"UPDATE table SET Inventory{data[2]} = ? WHERE Username = ?"
                };

                cm.Parameters.Add(new("Username", username));
                cm.Parameters.Add(new($"Inventory{data[2]}", parsed));

                cm.ExecuteNonQuery();
            }

            caseDefault: {
                string parsed = string.Format(rowFormat, item_id, amount, string.Format("INV{0}", slot), ring != null);

                var cm = new SQLiteCommand(m_db) {
                    CommandText = $"UPDATE table SET Inventory{slot} = ? WHERE Username = ?"
                };

                cm.Parameters.Add(new("Username", username));
                cm.Parameters.Add(new($"Inventory{slot}", parsed));

                cm.ExecuteNonQuery();
            } 
        }

        // WARNING: This is ugly af please find better solution
        public dynamic[] ParseSQLRow(string stringy_data) {
            string[] parsed = stringy_data.Split('|').Where(x => !string.IsNullOrEmpty(x)).ToArray();
            return new dynamic[] {
                int.Parse(parsed[0].Replace("Id", "").Trim()),
                int.Parse(parsed[1].Replace("Count", "").Trim()),
                int.Parse(parsed[2].Replace("INV", "").Trim()),
                bool.Parse(parsed[3])
            };
        }

        public Character GetChar(string username) {
            var check = new SQLiteCommand(m_db) {
                CommandText = "SELECT * FROM users WHERE username = ?"
            };

            check.Parameters.Add(new SQLiteParameter("username", username));
            var dr2 = check.ExecuteReader(CommandBehavior.CloseConnection);
            bool IsFound = false;
            while (dr2.Read()) {
                IsFound = true;
            }

            if (!IsFound) return null;

            var cm = new SQLiteCommand(m_db) {
                CommandText = "SELECT * FROM users_info where Username = ?"
            };

            cm.Parameters.Add(new SQLiteParameter("Username", username));
            var dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read()) {
                Character character = new();
                character.Username = (string)dr["Username"];
                character.Nickname = (string)dr["Nickname"];
                character.Level = DBGetI(dr, "Level");
                character.Rank = DBGetI(dr, "Rank");
                character.Gender = DBGetI(dr, "Gender");
                character.MCash = DBGetI(dr, "MCash");
                character.Gold = DBGetI(dr, "Gold");
                character.Wins = DBGetI(dr, "Wins");
                character.Loses = DBGetI(dr, "Loses");
                character.Scores = DBGetI(dr, "Scores");
                character.Instrument = DBGetI(dr, "Instrument");
                character.Hair = DBGetI(dr, "Hair");
                character.Accessory = DBGetI(dr, "Accessory");
                character.Glove = DBGetI(dr, "Glove");
                character.Necklace = DBGetI(dr, "Necklace");
                character.Cloth = DBGetI(dr, "Cloth");
                character.Pant = DBGetI(dr, "Pant");
                character.Glass = DBGetI(dr, "Glass");
                character.Earring = DBGetI(dr, "Earring");
                character.Shoe = DBGetI(dr, "Shoe");
                character.Face = DBGetI(dr, "Face");
                character.Wing = DBGetI(dr, "Wing");
                character.HairAccessory = DBGetI(dr, "HairAccessory");
                character.InstrumentAccessory = DBGetI(dr, "InstrumentAccessory");
                character.ClothAccessory = DBGetI(dr, "ClothAccessory");
                character.Pet = DBGetI(dr, "Pet");

                return character;
            }

            var cm2 = new SQLiteCommand(m_db) {
                CommandText = "INSERT INTO users_info("
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
                    + "Pet) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)"
            };

            SQLiteParameter[] parameters = {
                new SQLiteParameter("Username", username),
                new SQLiteParameter("Nickname", username),
                new SQLiteParameter("Level", 0),
                new SQLiteParameter("Rank", 0),
                new SQLiteParameter("Gender", 0),
                new SQLiteParameter("MCash", 0),
                new SQLiteParameter("Gold", 0),
                new SQLiteParameter("Wins", 0),
                new SQLiteParameter("Loses", 0),
                new SQLiteParameter("Scores", 0),
                new SQLiteParameter("Instrument", 0),
                new SQLiteParameter("Hair", 0),
                new SQLiteParameter("Accessory", 0),
                new SQLiteParameter("Glove", 0),
                new SQLiteParameter("Necklace", 0),
                new SQLiteParameter("Cloth", 0),
                new SQLiteParameter("Pant", 0),
                new SQLiteParameter("Glass", 0),
                new SQLiteParameter("Earring", 0),
                new SQLiteParameter("Shoe", 0),
                new SQLiteParameter("Face", 36),
                new SQLiteParameter("Wing", 0),
                new SQLiteParameter("HairAccessory", 0),
                new SQLiteParameter("InstrumentAccessory", 0),
                new SQLiteParameter("ClothAccessory", 0),
                new SQLiteParameter("Pet", 0),
            };

            for (int i = 0; i < parameters.Length; i++) {
                cm2.Parameters.Add(parameters[i]);
            }

            cm2.ExecuteNonQuery();
            return new Character() {
                Username = username,
                Nickname = username
            };
        }

        public static int DBGetI(SQLiteDataReader dr, string name) {
            var dbval = dr[name];
            int val = dbval is DBNull ? 0 : Convert.ToInt32(dbval);
            return val;
        }

        public static byte[] S(string hex) {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string P(byte[] ba) {
            return BitConverter.ToString(ba).Replace("-", "");
        }

        public class ExistResult {
            public bool IsExist { set; get; }
            public string Reason { set; get; }
        }
    }
}
