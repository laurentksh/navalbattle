using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BatailleNavale.Model
{
    public class BoatModel
    {
        public const int MaxSize = 4;

        public string Name { get; set; }
        public Vector2 Position { get; set; }
        public int Size { get; set; }
        public Orientation Orientation_ { get; set; }
        public int BoatTypeId { get; set; }

        public BoatModel(Vector2 pos, int size, Orientation orientation, string name)
        {
            Position = pos;
            Size = size;
            Orientation_ = orientation;
            Name = name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static BitmapImage GetBoatImage(int typeId = -1)
        {
            switch (typeId) {
                default:
                    return new BitmapImage(new Uri("./Resources/BoatDefault.png", UriKind.RelativeOrAbsolute));
            }
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }
    }
}
