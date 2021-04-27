using Estrol.X3Jam.Server.CData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CManager {
    public class CharacterExperiences {
        private O2JamServer Main;

        public CharacterExperiences(O2JamServer main) {
            Main = main;
        }

        public void SubmitScore(Character user, int[] scores) {

        }

        public int CalculateLvlBasedOnScore(int[] scores) {
            double lvl = 0;

            int score = scores[0];
            int kool = scores[1];
            int good = scores[2];
            int bad = scores[3];

            if (score > 2500) {
                if (score >= 1000000) {
                    lvl += 10;
                } else if (score >= 100000) {
                    lvl += 8;
                } else if (score >= 10000) {
                    lvl += 4;
                } else {
                    lvl += 2;
                }
            }

            if (good > 50) {
                if (good > 100 && lvl >= 5) {
                    lvl -= 4;
                } else if (good <= 100) {
                    lvl += 2;
                }
            }

            if (bad > 5 && lvl > 0) {
                if (lvl >= 2) {
                    lvl -= 1;
                } else if (lvl >= 10) {
                    lvl -= 9;
                }
            }

            return (int)Math.Round(lvl);
        }
    }
}
