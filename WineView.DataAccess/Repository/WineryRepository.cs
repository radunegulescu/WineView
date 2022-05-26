using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineView.DataAccess.Repository.IRepository;
using WineView.Models;

namespace WineView.DataAccess.Repository
{
    public class WineryRepository : Repository<Winery>, IWineryRepository
    {
        private ApplicationDbContext _db;

        public WineryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Winery obj)
        {
            _db.Wineries.Update(obj);
        }
    }
}
