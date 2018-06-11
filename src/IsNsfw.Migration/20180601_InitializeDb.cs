using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;
using IsNsfw.Model;
using ServiceStack;
using ServiceStack.Data;
using ServiceStack.OrmLite;

namespace IsNsfw.Migration
{
    [Migration(20180601)]
    public class _20180601_InitializeDb : FluentMigrator.Migration
    {
        public override void Up()
        {
            using(var db = HostContext.Resolve<IDbConnectionFactory>().OpenDbConnection())
            {
                using(var trans = db.OpenTransaction())
                {
                    db.CreateTableIfNotExists<User>();
                    db.CreateTableIfNotExists<Link>();
                    db.CreateTableIfNotExists<Tag>();
                    db.CreateTableIfNotExists<LinkTag>();
                    db.CreateTableIfNotExists<LinkEvent>();

                    var tags = new []
                    {
                        new Tag() { Key = "O", Name = "Offensive", ShortDescription = "Content has a tendancy to offend", SortOrder = 1, IsDeleted = false },
                        new Tag() { Key = "G", Name = "Gore", ShortDescription = "Gory content lies within", SortOrder = 2, IsDeleted = false },
                        new Tag() { Key = "N", Name = "Nudity", ShortDescription = "You might see some body parts", SortOrder = 3, IsDeleted = false },
                        new Tag() { Key = "XXX", Name = "Pornography", ShortDescription = "Not a Vin Diesel movie", SortOrder = 4, IsDeleted = false },
                    };

                    db.SaveAll(tags);

                    trans.Commit();
                }
            }
        }

        public override void Down()
        {
            using(var db = HostContext.Resolve<IDbConnectionFactory>().OpenDbConnection())
            {
                using(var trans = db.OpenTransaction())
                {
                    db.DropTable<LinkEvent>();
                    db.DropTable<LinkTag>();
                    db.DropTable<Tag>();
                    db.DropTable<Link>();
                    db.DropTable<User>();

                    trans.Commit();
                }
            }
        }
    }
}
