using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ProjectTags.Models
{
    public class EFContext : DbContext
    {
        public EFContext() : base("EFConnection")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<EFContext>());
            //Database.SetInitializer<EFContext>(new CreateDatabaseIfNotExists<EFContext>());
        }
        
        /// <summary>
        /// 用户信息
        /// </summary>
        public DbSet<Users> Users { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<Processes> Processes { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Teams> Teams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //指定单数形式的表名
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //物理表名添加xx前綴
            //modelBuilder.Types().Configure(f => f.ToTable("zl" + f.ClrType.Name));
            base.OnModelCreating(modelBuilder);
        }
    }
}