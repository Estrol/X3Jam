using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Website.Types {
    public class ShopResult {
        public bool IsSuccess { set; get; }
        public ShopItem[] Items { set; get; }
    }

    public class ShopItem {
        public string Name { set; get; }
        public int Amount { set; get; }
    }

    public class PaymentResult {
        public bool IsSuccess { set; get; }
        public string Message { set; get; }
    }

    public class PaymentPOST {
        public string Username = "";
        public string Password = "";
        public int[] items;
    }

    public class JSON_Version {
        public string O2JamServerVersion { set; get; }
        public string EstrolHTTPServerVersion { set; get; }
        public string GitVersionHash { set; get; }
    }

    public class JSON_MusicList {
        public JSON_OJN[] list { set; get; }
    }

    public class JSON_OJN {
        public string title { get; set; }
        public string artist { get; set; }
        public string ojn_name { get; set; }
        public float bpm { get; set; }
        public int id { get; set; }
        public int level_ex { get; set; }
        public int level_nx { get; set; }
        public int level_hx { get; set; }
        public int note_ex_count { get; set; }
        public int note_nx_count { get; set; }
        public int note_hx_count { get; set; }
        public int duration_ex { get; set; }
        public int duration_nx { get; set; }
        public int duration_hx { get; set; }
    }

    public class JSON_Userlist {
        public JSON_Channel[] channels { set; get; }
    }

    public class JSON_Channel {
        public string channel { set; get; }
        public int users_count { set; get; }
        public JSON_User[] users { set; get; }
    }

    public class JSON_User {
        public string username { set; get; }
        public int channel { set; get; }
        public int level { set; get; }
    }

    public class JSON_UserInfo {
        public string username { set; get; }
        public string nickname { set; get; }
        public int gender { set; get; }
        public int level { set; get; }
        public int mcash { set; get; }
        public int gold { set; get; }
        public int wins { set; get; }
        public int loses { set; get; }
        public int scores { set; get; }

    }

    public class JSON_RegisterPOST {
        public string username { set; get; }
        public string nickname { set; get; }
        public string password { set; get; }
        public string email { set; get; }
        public string invite_code { set; get; }
    }

    public class JSON_RegisterResponse {
        public bool success { set; get; }
        public string message { set; get; }
    }

    public class JSON_GenerateInviteKey {
        public string username { set; get; }
    }

    public class JSON_GenerateResponse {
        public string invite_code { set; get; }
    }
}
