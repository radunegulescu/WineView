using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineView.DataAccess.Repository.IRepository;
using WineView.Models;

namespace WineView.DataAccess.Repository
{
    public class BodyRepository : Repository<Body>, IBodyRepository
    {
        private ApplicationDbContext _db;

        public BodyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Body obj)
        {
            _db.Bodies.Update(obj);
        }
    }
}
