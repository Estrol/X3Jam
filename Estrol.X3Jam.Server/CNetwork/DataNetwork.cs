using System;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Security.Cryptography;

using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Server.CData;
using System.Collections.Generic;
using System.Data;

namespace Estrol.X3Jam.Server.CNetwork {
    public class DataNetwork {
        private SQLiteConnection m_db;
        private SHA256Managed m_sha256;
        private RNGCryptoServiceProvider m_rng;

        private bool m_ready;

        public DataNetwork() {
            m_db = new SQLiteConnection($"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\conf\\o2jam_users.db");
            m_rng = new RNGCryptoServiceProvider();
            m_sha256 = new SHA256Managed();
            m_ready = false;

            m_db.Open();
        }

        public void Intialize() {
            Log.Write("Preparing Database tables!");
            var cm = new SQLiteCommand(m_db);

            cm.CommandText = @"CREATE TABLE IF NOT EXISTS users(id INTEGER PRIMARY KEY, username TEXT, password TEXT, salt TEXT, nickname TEXT, master BOOLEAN)";
            cm.ExecuteNonQuery();

            cm.CommandText = @"CREATE TABLE IF NOT EXISTS users_character(id INTEGER PRIMARY KEY, username TEXT,"
                + "gender INTEGER,"
                + "instrument INTEGER,"
                + "hair INTEGER,"
                + "accessory INTEGER,"
                + "glove INTEGER,"
                + "necklace INTEGER,"
                + "top INTEGER,"
                + "pant INTEGER,"
                + "glass INTEGER,"
                + "earring INTEGER,"
                + "shoe INTEGER,"
                + "face INTEGER,"
                + "wing INTEGER,"
                + "hairaccessory INTEGER,"
                + "instrumentaccessory INTEGER,"
                + "clothaccessory INTEGER,"
                + "pet INTEGER)";
            cm.ExecuteNonQuery();

#if DEBUG
            cm.CommandText = "INSERT OR IGNORE INTO users(id, username, password, salt, nickname, master) " +
                "VALUES(1, \"test\", \"eSiv8purm6ri2LXKk1B4aNIiRUv9JVdOx/DH8pbSztM=\", \"FFAABBCCDDEE\", \"test\", true)";
            cm.ExecuteNonQuery();

            cm.CommandText = "INSERT OR IGNORE INTO users(id, username, password, salt, nickname, master) " +
                "VALUES(2, \"test2\", \"ELcELOpVeTXUA4+w2N0SwD1CbcU5l+wNRPxQhWfXzaY=\", \"FFAABBBBBBEE\", \"test\", true)";
            cm.ExecuteNonQuery();
#endif

            m_ready = true;
            Log.Write("Database finish prepared!");
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

        public bool Exists(string username) {
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
                return true;
            }

            return false;
        } 

        public void Register(string username, string password) {
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
                CommandText = "INSERT INTO users(username, password, salt, nickname, master) VALUES(?,?,?,?,?)",
            };

            SQLiteParameter[] parameters = {
                new SQLiteParameter("username", username),
                new SQLiteParameter("password", Convert.ToBase64String(hashed_password)),
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

                    User user = new User(new string[] {
                        username,
                        (string)dr["nickname"]
                    }, character);

                    return user;
                }
            }

            return null;
        }

        public Character GetChar(string username) {
            var cm = new SQLiteCommand(m_db) {
                CommandText = "SELECT * FROM users_character where username = ?"
            };

            cm.Parameters.Add(new SQLiteParameter("username", username));
            var dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read()) {
                Character character = new Character() {
                    Gender = (int)dr["gender"],
                    Instrument = (int)dr["instrument"],
                    Hair = (int)dr["hair"],
                    Accessory = (int)dr["accessory"],
                    Glove = (int)dr["glove"],
                    Necklace = (int)dr["necklace"],
                    Top = (int)dr["top"],
                    Pant = (int)dr["pant"],
                    Glass = (int)dr["glass"],
                    Earring = (int)dr["earring"],
                    Shoe = (int)dr["shoe"],
                    Face = (int)dr["face"],
                    Wing = (int)dr["wing"],
                    HairAccessory = (int)dr["hairaccessory"],
                    InstrumentAccessory = (int)dr["instrumentaccessory"],
                    ClothAccessory = (int)dr["clothaccessory"],
                    Pet = (int)dr["pet"]
                };

                return character;
            }

            var cm2 = new SQLiteCommand(m_db) {
                CommandText = "INSERT INTO users_character(username,instrument," +
                "hair,accessory,glove,necklace,top,pant,glass,earring," +
                "shoe,face,wing,hairaccessory,instrumentaccessory,cl" +
                "othaccessory,pet) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?," +
                "?,?,?)"
            };

            SQLiteParameter[] parameters = {
                new SQLiteParameter("username", username),
                new SQLiteParameter("instrument", 0),
                new SQLiteParameter("hair", 0),
                new SQLiteParameter("accessory", 0),
                new SQLiteParameter("glove", 0),
                new SQLiteParameter("necklace", 0),
                new SQLiteParameter("top", 0),
                new SQLiteParameter("pant", 0),
                new SQLiteParameter("glass", 0),
                new SQLiteParameter("earring", 0),
                new SQLiteParameter("shoe", 0),
                new SQLiteParameter("face", 36),
                new SQLiteParameter("wing", 0),
                new SQLiteParameter("hairaccessory", 0),
                new SQLiteParameter("instrumentaccessory", 0),
                new SQLiteParameter("clothaccessory", 0),
                new SQLiteParameter("pet", 0),
            };

            for (int i = 0; i < parameters.Length; i++) {
                cm2.Parameters.Add(parameters[i]);
            }

            cm2.ExecuteNonQuery();
            return new Character();
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
    }
}
