using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineView.Models;

namespace WineView.DataAccess.Repository.IRepository
{
    public interface IWineRepository : IRepository<Wine>
    {
        void Update(Wine obj);
    }
}
