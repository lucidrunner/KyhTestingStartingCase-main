using Microsoft.EntityFrameworkCore;
using ShopGeneral.Data;

namespace ShopGeneral.Services
{
    public class ManufacturerService : IManufacturerService
    {
        private readonly ApplicationDbContext _context;

        public ManufacturerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Manufacturer> GetAllManufacturers()
        {
            return _context.Manufacturers;
        }
    }
}
