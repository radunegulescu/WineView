using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineView.DataAccess.Repository.IRepository;
using WineView.Models;

namespace WineView.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Winery = new WineryRepository(_db);
            Color = new ColorRepository(_db);
            Grape = new GrapeRepository(_db);
            Style = new StyleRepository(_db);
            Wine = new WineRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            Review = new ReviewRepository(_db);
            Body = new BodyRepository(_db);
        }

        public IWineryRepository Winery { get; private set; }
        public IColorRepository Color { get; private set; }
        public IGrapeRepository Grape { get; private set; }
        public IStyleRepository Style { get; private set; }
        public IWineRepository Wine { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IReviewRepository Review { get; private set; }
        public IBodyRepository Body { get; private set; }



        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
