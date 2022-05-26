using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineView.DataAccess.Repository.IRepository;
using WineView.Models;

namespace WineView.DataAccess.Repository
{
    public class GrapeRepository : Repository<Grape>, IGrapeRepository
    {
        private ApplicationDbContext _db;

        public GrapeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Grape obj)
        {
            _db.Grapes.Update(obj);
        }
    }
}
