using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatailleNavale.Model
{
    public class IAModel
    {
        public Difficulty Difficulty_;


        public IAModel(Difficulty difficulty)
        {
            this.Difficulty_ = difficulty;
        }

        public enum Difficulty
        {
            None,
            Easy,
            Normal,
            Hard
        }
    }
}