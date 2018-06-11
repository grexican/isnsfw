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
    [Migration(20180605)]
    public class _20180605_AddScreenshotUrl : FluentMigrator.Migration
    {
        public override void Up()
        {
            using(var db = HostContext.Resolve<IDbConnectionFactory>().OpenDbConnection())
            {
                using(var trans = db.OpenTransaction())
                {
                    if(!db.ColumnExists<Link>(m => m.ScreenshotUrl))
                        db.AddColumn<Link>(x => x.ScreenshotUrl);
                }
            }
        }

        public override void Down()
        {
            using(var db = HostContext.Resolve<IDbConnectionFactory>().OpenDbConnection())
            {
                using(var trans = db.OpenTransaction())
                {
                    if(db.ColumnExists<Link>(m => m.ScreenshotUrl))
                        db.DropColumn<Link>(x => x.ScreenshotUrl);
                }
            }
        }
    }
}
