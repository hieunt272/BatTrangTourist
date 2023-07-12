namespace BatTrangTourist.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        Password = c.String(nullable: false, maxLength: 60),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AlbumProducts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        TitleImage = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        Sort = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 200),
                        Description = c.String(maxLength: 500),
                        Image = c.String(),
                        Body = c.String(),
                        About = c.String(),
                        Note = c.String(),
                        Price = c.Decimal(precision: 18, scale: 2),
                        PriceSale = c.Decimal(precision: 18, scale: 2),
                        CreateDate = c.DateTime(nullable: false),
                        Sort = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Home = c.Boolean(nullable: false),
                        Hot = c.Boolean(nullable: false),
                        Favorite = c.Boolean(nullable: false),
                        ImageReview = c.String(),
                        Url = c.String(maxLength: 300),
                        TitleMeta = c.String(maxLength: 100),
                        DescriptionMeta = c.String(maxLength: 500),
                        ProductCategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductCategories", t => t.ProductCategoryId, cascadeDelete: true)
                .Index(t => t.ProductCategoryId);
            
            CreateTable(
                "dbo.ProductCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 80),
                        Url = c.String(maxLength: 500),
                        CategorySort = c.Int(nullable: false),
                        Description = c.String(maxLength: 500),
                        Body = c.String(),
                        CategoryActive = c.Boolean(nullable: false),
                        ParentId = c.Int(),
                        ShowMenu = c.Boolean(nullable: false),
                        ShowCategory = c.Boolean(nullable: false),
                        Hot = c.Boolean(nullable: false),
                        Home = c.Boolean(nullable: false),
                        TitleMeta = c.String(maxLength: 100),
                        DescriptionMeta = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductCategories", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionName = c.String(nullable: false, maxLength: 150),
                        Body = c.String(),
                        Sort = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Vote = c.Int(nullable: false),
                        Image = c.String(maxLength: 500),
                        Content = c.String(nullable: false),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Mobile = c.String(nullable: false, maxLength: 10),
                        Avatar = c.String(maxLength: 500),
                        CreateDate = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Color = c.String(),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Schedule = c.String(),
                        Include = c.String(),
                        Note = c.String(),
                        Terms = c.String(),
                        Manual = c.String(),
                        AdultPrice = c.Decimal(precision: 18, scale: 2),
                        ChildPrice = c.Decimal(precision: 18, scale: 2),
                        Active = c.Boolean(nullable: false),
                        Sort = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        Title = c.String(maxLength: 100),
                        Description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ArticleCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        Url = c.String(maxLength: 500),
                        CategorySort = c.Int(nullable: false),
                        CategoryActive = c.Boolean(nullable: false),
                        ParentId = c.Int(),
                        ShowMenu = c.Boolean(nullable: false),
                        TitleMeta = c.String(maxLength: 100),
                        DescriptionMeta = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        TypePost = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(nullable: false, maxLength: 150),
                        Description = c.String(nullable: false, maxLength: 500),
                        Body = c.String(),
                        Image = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        View = c.Int(nullable: false),
                        ArticleCategoryId = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Home = c.Boolean(nullable: false),
                        ShowFooter = c.Boolean(nullable: false),
                        Url = c.String(maxLength: 300),
                        TitleMeta = c.String(maxLength: 100),
                        DescriptionMeta = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ArticleCategories", t => t.ArticleCategoryId, cascadeDelete: true)
                .Index(t => t.ArticleCategoryId);
            
            CreateTable(
                "dbo.Banners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BannerName = c.String(nullable: false, maxLength: 100),
                        Slogan = c.String(maxLength: 500),
                        Image = c.String(maxLength: 500),
                        Active = c.Boolean(nullable: false),
                        GroupId = c.Int(nullable: false),
                        Url = c.String(maxLength: 500),
                        Sort = c.Int(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConfigSites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Facebook = c.String(maxLength: 500),
                        Youtube = c.String(maxLength: 500),
                        Twitter = c.String(maxLength: 500),
                        Instagram = c.String(maxLength: 500),
                        Telegram = c.String(maxLength: 500),
                        LiveChat = c.String(maxLength: 4000),
                        Image = c.String(),
                        Favicon = c.String(),
                        GoogleMap = c.String(maxLength: 4000),
                        GoogleAnalytics = c.String(maxLength: 4000),
                        Place = c.String(),
                        Title = c.String(maxLength: 200),
                        AboutImage = c.String(),
                        AboutText = c.String(),
                        AboutUrl = c.String(maxLength: 500),
                        Description = c.String(maxLength: 500),
                        Hotline = c.String(maxLength: 50),
                        Zalo = c.String(maxLength: 50),
                        Email = c.String(maxLength: 50),
                        InfoContact = c.String(),
                        InfoFooter = c.String(),
                        BankInfo = c.String(),
                        VideoUrl = c.String(maxLength: 500),
                        ListImage = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Mobile = c.String(nullable: false, maxLength: 10),
                        Email = c.String(nullable: false, maxLength: 100),
                        Body = c.String(maxLength: 4000),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Feedbacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(maxLength: 700),
                        Image = c.String(maxLength: 500),
                        Name = c.String(nullable: false, maxLength: 100),
                        Classify = c.String(maxLength: 100),
                        TourName = c.String(maxLength: 200),
                        Star = c.Int(nullable: false),
                        Sort = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ServiceId = c.Int(nullable: false),
                        AdultQuantity = c.Int(),
                        AdultPrice = c.Decimal(precision: 18, scale: 2),
                        ChildQuantity = c.Int(),
                        ChildPrice = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.OrderId)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderNumber = c.String(nullable: false, maxLength: 50),
                        CreateDate = c.DateTime(nullable: false),
                        Payment = c.Boolean(nullable: false),
                        TransportDate = c.DateTime(nullable: false),
                        AdultQuantity = c.Int(),
                        AdultPrice = c.Int(),
                        ChildQuantity = c.Int(),
                        ChildPrice = c.Int(),
                        Status = c.Int(nullable: false),
                        Viewed = c.Boolean(nullable: false),
                        DiscountCode = c.String(),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CustomerInfo_Fullname = c.String(nullable: false, maxLength: 50),
                        CustomerInfo_Address = c.String(nullable: false, maxLength: 200),
                        CustomerInfo_Mobile = c.String(nullable: false, maxLength: 11),
                        CustomerInfo_Email = c.String(nullable: false, maxLength: 50),
                        CustomerInfo_Body = c.String(maxLength: 200),
                        CustomerInfo_IsNewMember = c.Boolean(nullable: false),
                        OrderMemberId = c.Int(),
                        ServiceId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Services", t => t.ServiceId)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.PhotoLibraries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AlbumName = c.String(nullable: false),
                        ListImage = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        Sort = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        Body = c.String(),
                        Url = c.String(maxLength: 300),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Subcribes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 100),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Username = c.String(nullable: false),
                        Avatar = c.String(maxLength: 500),
                        Password = c.String(nullable: false, maxLength: 60),
                        PhoneNumber = c.String(nullable: false, maxLength: 10),
                        Email = c.String(nullable: false, maxLength: 50),
                        Token = c.String(maxLength: 50),
                        Active = c.Boolean(nullable: false),
                        Color = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TagServices",
                c => new
                    {
                        Tag_Id = c.Int(nullable: false),
                        Service_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Tag_Id, t.Service_Id })
                .ForeignKey("dbo.Tags", t => t.Tag_Id, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.Service_Id, cascadeDelete: true)
                .Index(t => t.Tag_Id)
                .Index(t => t.Service_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderDetails", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.Articles", "ArticleCategoryId", "dbo.ArticleCategories");
            DropForeignKey("dbo.TagServices", "Service_Id", "dbo.Services");
            DropForeignKey("dbo.TagServices", "Tag_Id", "dbo.Tags");
            DropForeignKey("dbo.Services", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Reviews", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Questions", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "ProductCategoryId", "dbo.ProductCategories");
            DropForeignKey("dbo.ProductCategories", "ParentId", "dbo.ProductCategories");
            DropForeignKey("dbo.AlbumProducts", "ProductId", "dbo.Products");
            DropIndex("dbo.TagServices", new[] { "Service_Id" });
            DropIndex("dbo.TagServices", new[] { "Tag_Id" });
            DropIndex("dbo.Orders", new[] { "ServiceId" });
            DropIndex("dbo.OrderDetails", new[] { "ServiceId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropIndex("dbo.Articles", new[] { "ArticleCategoryId" });
            DropIndex("dbo.Services", new[] { "ProductId" });
            DropIndex("dbo.Reviews", new[] { "ProductId" });
            DropIndex("dbo.Questions", new[] { "ProductId" });
            DropIndex("dbo.ProductCategories", new[] { "ParentId" });
            DropIndex("dbo.Products", new[] { "ProductCategoryId" });
            DropIndex("dbo.AlbumProducts", new[] { "ProductId" });
            DropTable("dbo.TagServices");
            DropTable("dbo.Users");
            DropTable("dbo.Subcribes");
            DropTable("dbo.PhotoLibraries");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.Feedbacks");
            DropTable("dbo.Contacts");
            DropTable("dbo.ConfigSites");
            DropTable("dbo.Banners");
            DropTable("dbo.Articles");
            DropTable("dbo.ArticleCategories");
            DropTable("dbo.Tags");
            DropTable("dbo.Services");
            DropTable("dbo.Reviews");
            DropTable("dbo.Questions");
            DropTable("dbo.ProductCategories");
            DropTable("dbo.Products");
            DropTable("dbo.AlbumProducts");
            DropTable("dbo.Admins");
        }
    }
}
