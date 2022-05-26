using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WineView.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IWineryRepository Winery { get; }
        IColorRepository Color { get; }
        IGrapeRepository Grape { get; }
        IStyleRepository Style { get; }
        IWineRepository Wine { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IBodyRepository Body { get; }
        IReviewRepository Review { get; }

        void Save();
    }
}
