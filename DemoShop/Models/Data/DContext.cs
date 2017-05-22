using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DemoShop.Models.Data
{
    public class DContext : DbContext
    {
        public DbSet<PageDTO> Pages { get; set; }
    }
}