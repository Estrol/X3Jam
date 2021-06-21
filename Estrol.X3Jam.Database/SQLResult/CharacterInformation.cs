using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Database.SQLResult {
    public class CharacterInformation {
        public bool IsSuccess { set; get; }
        public string Username { set; get; }
        public string Nickname { set; get; }
        public int Rank { set; get; }
        public int Level { set; get; }
        public int Gender { set; get; }
        public int MCash { set; get; }
        public int Gold { set; get; }
        public int Wins { set; get; }
        public int Loses { set; get; }
        public int Scores { set; get; }
        public int[] Data { set; get; }
    }
}
