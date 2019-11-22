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

        public BoatModel(Vector2 pos, int size, Orientation orientation, int boatTypeId = -1)
        {
            Position = pos;
            Size = size;
            Orientation_ = orientation;
            BoatTypeId = boatTypeId;
        }

        public int GetDestroyedCases(List<Hit> hits)
        {
            int hitsCount = 0;

            for (int i = 0; i < Size; i++) {
                Vector2 v = Vector2.Zero;

                if (Orientation_ == BoatModel.Orientation.Horizontal)
                    v = new Vector2(Position.X + i, Position.Y);
                else
                    v = new Vector2(Position.X, Position.Y);

                foreach (Hit hit in hits)
                    if (hit.Position == v)
                        hitsCount++;
            }

            return hitsCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public static BitmapImage GetBoatImage(int typeId = -1)
        {
            if (!System.IO.File.Exists($"./Resources/BoatImages/Boat{typeId}.png"))
                typeId = -1;

            switch (typeId) {
                default:
                    return new BitmapImage(new Uri($"./Resources/BoatImages/Boat{typeId}.png", UriKind.RelativeOrAbsolute));
                case -1:
                    return new BitmapImage(new Uri("./Resources/BoatImages/BoatDefault.png", UriKind.RelativeOrAbsolute));
            }
        }

        public static List<BoatPreset> GetBoatPresets()
        {
            List<BoatPreset> boatPresets = new List<BoatPreset>
            {
                new BoatPreset
                {
                    boatId = 0,
                    boatSize = 2
                },

                new BoatPreset
                {
                    boatId = 1,
                    boatSize = 3
                },

                new BoatPreset
                {
                    boatId = 2,
                    boatSize = 3
                },

                new BoatPreset
                {
                    boatId = 3,
                    boatSize = 4
                },

                new BoatPreset
                {
                    boatId = 4,
                    boatSize = 5
                }
            };

            return boatPresets;
        }

        public enum Orientation
        {
            Horizontal,
            Vertical
        }
    }

    public struct BoatPreset
    {
        public int boatId;
        public int boatSize;
    }
}
