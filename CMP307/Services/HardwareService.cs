using CMP307.Data;
using CMP307.Models;
using System.Collections.Generic;
using System.Linq;

namespace CMP307.Services
{
    public class HardwareService
    {
        private readonly ScottishGlenContext _context;

        public HardwareService(ScottishGlenContext context)
        {
            _context = context;
        }

        public void AddHardware(Hardware hardware)
        {
            _context.HardwareAssets.Add(hardware);
            _context.SaveChanges();
        }

        public void EditHardware(Hardware hardware)
        {
            _context.HardwareAssets.Update(hardware);
            _context.SaveChanges();
        }

        public void DeleteHardware(Hardware hardware)
        {
            _context.HardwareAssets.Remove(hardware);
            _context.SaveChanges();
        }

        public List<Hardware> GetAllHardware()
        {
            return _context.HardwareAssets.ToList();
        }
    }
}
