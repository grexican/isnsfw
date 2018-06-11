using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace IsNsfw.Migration
{
    [Migration(20180611)]
    public class _20180611_RenameLinkEventToLinkEvents : FluentMigrator.Migration
    {
        public override void Up()
        {
            Rename.Table("LinkEvent").InSchema("public").To("LinkEvents");
        }

        public override void Down()
        {
            Rename.Table("LinkEvents").InSchema("public").To("LinkEvent");
        }
    }
}
