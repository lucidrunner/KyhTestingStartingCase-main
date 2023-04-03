using ShopGeneral.Data;

namespace ShopGeneral.Services;

public interface IManufacturerService 
{
    public IEnumerable<Manufacturer> GetAllManufacturers();
}