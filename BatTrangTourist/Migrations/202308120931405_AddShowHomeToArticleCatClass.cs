namespace BatTrangTourist.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddShowHomeToArticleCatClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ArticleCategories", "ShowHome", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ArticleCategories", "ShowHome");
        }
    }
}
