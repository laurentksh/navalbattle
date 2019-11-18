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

         public Vector2 Position { get; set; }
        public int Size { get; set; }
        public Orientation Orientation_ { get; set; }
        public int BoatTypeId { get; set; }

        public BoatModel(Vector2 pos, int size, Orientation orientation)
        {
            Position = pos;
            Size = size;
            Orientation_ = orientation;
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

        public static List<BoatPresets> GetBoatPresets()
        {
            List<BoatPresets> boatPresets = new List<BoatPresets>();

            boatPresets.Add(new BoatPresets
            {
                boatId = 0,
                boatSize = 2

            });
            boatPresets.Add(new BoatPresets
            {
                boatId = 1,
                boatSize = 3

            });
            boatPresets.Add(new BoatPresets
            {
                boatId = 2,
                boatSize = 3

            });
            boatPresets.Add(new BoatPresets
            {
                boatId = 3,
                boatSize = 4

            });
            boatPresets.Add(new BoatPresets
            {
                boatId = 4,
                boatSize = 5

            });


            return boatPresets;


        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }
    }

    public struct BoatPresets
    {
        public int boatId;
        public int boatSize;
    }
}
