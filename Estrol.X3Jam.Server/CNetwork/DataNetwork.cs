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

            cm.CommandText = "CREATE TABLE IF NOT EXISTS users(id INTEGER PRIMARY KEY, username TEXT, password TEXT, salt TEXT, nickname TEXT, master BOOLEAN)";
            cm.ExecuteNonQuery();

            cm.CommandText = "CREATE TABLE IF NOT EXISTS users_info(id INTEGER PRIMARY KEY," +
                "Username TEXT," +
                "Nickname TEXT," +
                "Level INTEGER," +
                "Rank INTEGER," +
                "Gender INTEGER," +
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
                    + "Pet) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)"
            };

            SQLiteParameter[] parameters = {
                new SQLiteParameter("Username", username),
                new SQLiteParameter("Nickname", username),
                new SQLiteParameter("Level", 0),
                new SQLiteParameter("Rank", 0),
                new SQLiteParameter("Gender", 0),
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
            return new Character();
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
    }
}
