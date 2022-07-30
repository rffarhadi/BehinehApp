using DLLCore.DBContext;
using DLLCore.DBContext.Entities.StockHistoy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLLCore.Repositories.PriceHistory
{
    public class PriceRepository
    {
        ApplicationDbContext db;

        public PriceRepository()
        {
            db = new ApplicationDbContext();
        }

        public List<SecurityHistory> GetAll()
        {
            var list = db.SecurityTbl.ToList();
            return list;
        }
    }
}
