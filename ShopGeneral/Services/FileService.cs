using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShopGeneral.Services
{
    public class FileService: IFileService
    {
        private const string BaseFolder = "outfiles";

        public FileService()
        {
            if (!Directory.Exists(BaseFolder))
                Directory.CreateDirectory(BaseFolder);
        }

        public void SaveJson(string folder, string fileName, object classToSave)
        {
            if (!Directory.Exists($"{BaseFolder}\\{folder}"))
                Directory.CreateDirectory($"{BaseFolder}\\{folder}");

            var path = $"{BaseFolder}\\{folder}\\{fileName}";
            var json = JsonConvert.SerializeObject(classToSave, Formatting.Indented);
            try
            {
                File.WriteAllText(path, json);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
