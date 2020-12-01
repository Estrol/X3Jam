using System;
using System.IO;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Estrol.X3Jam.Server.Data;
using System.Data;

namespace Estrol.X3Jam.Server {
    public class Database {
        public ServerMain main;
        public SQLiteConnection db_users;
        private RNGCryptoServiceProvider rng;

        public byte[] salt_pswd = new byte[] { 0xff, 0xaa, 0xbb, 0xcc, 0xdd, 0xee };

        public Database(ServerMain main) {
            this.main = main;
            rng = new RNGCryptoServiceProvider();

            db_users = new SQLiteConnection("Data source=" + AppDomain.CurrentDomain.BaseDirectory + @"\conf\o2jam_users.db");
            db_users.Open();
            SQLiteCommand cm = new SQLiteCommand(db_users);

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
            cm.CommandText = "INSERT OR IGNORE INTO users(id, username, password, salt, nickname, master) " +
                "VALUES(1, \"test\", \"eSiv8purm6ri2LXKk1B4aNIiRUv9JVdOx/DH8pbSztM=\", \"FFAABBCCDDEE\", \"test\", true)";

            cm.ExecuteNonQuery();

            cm.CommandText = "INSERT OR IGNORE INTO users(id, username, password, salt, nickname, master) " +
                "VALUES(2, \"test2\", \"ELcELOpVeTXUA4+w2N0SwD1CbcU5l+wNRPxQhWfXzaY=\", \"FFAABBBBBBEE\", \"test\", true)";

            cm.ExecuteNonQuery();

            Console.WriteLine("[Server] Database Intialized");
        }

        public void GetUserCharacter(string username) {
            
        }

        public int GetUserCount() {
            SQLiteCommand cm = new SQLiteCommand(db_users);
            cm.CommandText = "SELECT COUNT(id) FROM users";
            int RowCount = 0;

            RowCount = Convert.ToInt32(cm.ExecuteScalar());
            return RowCount;
        }

        public void CreateUser(string username, string password) {
            byte[] pswd = Encoding.UTF8.GetBytes(password);
            pswd = pswd.Concat(salt_pswd).ToArray();

            byte[] salt = new byte[6];
            rng.GetBytes(salt);

            byte[] hashed_password = new SHA256Managed().ComputeHash(pswd);
            SQLiteCommand cm = new SQLiteCommand(db_users) {
                CommandText = "INSERT INTO users(username, password, salt, nickname, master) VALUES(?,?,?,?,?)"
            };

            cm.Parameters.Add(new SQLiteParameter("username", username));
            cm.Parameters.Add(new SQLiteParameter("passowrd", Convert.ToBase64String(hashed_password)));
            cm.Parameters.Add(new SQLiteParameter("salt", P(salt)));
            cm.Parameters.Add(new SQLiteParameter("nickname", username));
            cm.Parameters.Add(new SQLiteParameter("master", false));
            cm.ExecuteNonQuery();
        }

        public User CredentialsLogin(string username, string password) {
            SQLiteCommand cm = new SQLiteCommand(db_users) {
                CommandText = "SELECT * FROM users WHERE username = ?"
            };
            cm.Parameters.Add(new SQLiteParameter("username", username));
            SQLiteDataReader dr = cm.ExecuteReader(CommandBehavior.CloseConnection);

            while (dr.Read()) {
                byte[] pswd = Encoding.UTF8.GetBytes(password);
                pswd = pswd.Concat(S((string)dr["salt"])).ToArray();
                byte[] hashed = new SHA256Managed().ComputeHash(pswd);
                string stred = Convert.ToBase64String(hashed);

                if (stred == (string)dr["password"]) {
                    User user = new User(new string[] {
                        username,
                        Convert.ToBase64String(hashed),
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

        public void CloseDatabase() {
            db_users.Close();
        }
    }
}
