using BatailleNavale.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace BatailleNavale
{
    public static class UserDataLoader
    {
        /// <summary>
        /// Parse a file as a UserDataModel.
        /// </summary>
        /// <param name="filePath">Path of the file to load.</param>
        /// <returns></returns>
        public static UserDataModel Load(string filePath)
        {
            byte[] content = Convert.FromBase64String(File.ReadAllText(filePath));

            return JsonConvert.DeserializeObject<UserDataModel>(Encoding.Default.GetString(content));
        }

        /// <summary>
        /// Serialize a UserDataModel object and save it to the specified file path.
        /// </summary>
        /// <param name="filePath">Where to save the file.</param>
        /// <param name="model">UserDataModel to serialize.</param>
        public static void Save(string filePath, UserDataModel model)
        {
            string data = JsonConvert.SerializeObject(model, Formatting.None);
            
            File.WriteAllText(filePath, Convert.ToBase64String(Encoding.Default.GetBytes(data)));
        }
    }
}
