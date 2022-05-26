﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WineView.DataAccess.Repository.IRepository;
using WineView.Models;

namespace WineView.DataAccess.Repository
{
    public class ColorRepository : Repository<Color>, IColorRepository
    {
        private ApplicationDbContext _db;

        public ColorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Color obj)
        {
            _db.Colors.Update(obj);
        }
    }
}
