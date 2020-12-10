using System;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Security.Cryptography;

using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Server.Data;
using System.Collections.Generic;
using System.Data;

namespace Estrol.X3Jam.Server {
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
                + "instrument INTEGER,"
                + "hair INTEGER,"
                + "accessories INTEGER,"
                + "globes INTEGER,"
                + "necklaces INTEGER,"
                + "shirts INTEGER,"
                + "pants INTEGER,"
                + "glasses INTEGER,"
                + "earrings INTEGER,"
                + "clothesaccs INTEGER,"
                + "shoes INTEGER,"
                + "faces INTEGER,"
                + "wings INTEGER,"
                + "instrumentaccs INTEGER,"
                + "pets INTEGER,"
                + "hairaccs INTEGER)";
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

        public void Register(string username, string password) {
            if (!m_ready) {
                throw new Exception("DataNetwork is not ready! Please invoke .Intialized first!");
            }

            byte[] salt = new byte[6];
            m_rng.GetBytes(salt);

            byte[] pswd = Encoding.UTF8.GetBytes(password);
            pswd = pswd.Concat(salt).ToArray();

            byte[] hashed_password = m_sha256.ComputeHash(pswd);

            var cm = new SQLiteCommand(m_db) {
                CommandText = "INSERT INTO users(username, password, salt, nickname, master) VALUES(?,?,?,?,?)",
            };

            List<SQLiteParameter> list = new List<SQLiteParameter>() {
                new SQLiteParameter("username", username),
                new SQLiteParameter("password", Convert.ToBase64String(hashed_password)),
                new SQLiteParameter("salt", P(salt)),
                new SQLiteParameter("nickname", username),
                new SQLiteParameter("master", false)
            };

            cm.Parameters.Add(list);
            cm.ExecuteNonQuery();
        }

        public User Login(string username, string password) {
            if (!m_ready) {
                throw new Exception("DataNetwork is not ready! Please invoke .Intialized first!");
            }

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
                    User user = new User(new string[] {
                        username,
                        (string)dr["nickname"]
                    });

                    return user;
                }
            }

            return null;
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
