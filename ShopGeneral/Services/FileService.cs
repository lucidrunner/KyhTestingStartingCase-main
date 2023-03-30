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
        public void SaveJson(string path, object classToSave)
        {
            var json = JsonConvert.SerializeObject(classToSave, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }

    public interface IFileService
    {
        void SaveJson(string path, object classToSave);
    }
}
