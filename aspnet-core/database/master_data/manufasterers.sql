INSERT INTO [dbo].[AppManufacturers]
([Id]
    ,[Name]
    ,[Code]
    ,[Slug]
    ,[CoverPicture]
    ,[Visibility]
    ,[IsActive]
    ,[Country]
    ,[ExtraProperties]
    ,[ConcurrencyStamp]
    ,[CreationTime]
    ,[CreatorId])
VALUES
    (newid()
        ,N'Apple'
        ,'M1'
        ,'apple'
        ,null
        ,1
        ,1
        ,'US'
        ,null
        ,null
        ,getdate()
        ,null)

INSERT INTO [dbo].[AppManufacturers]
([Id]
    ,[Name]
    ,[Code]
    ,[Slug]
    ,[CoverPicture]
    ,[Visibility]
    ,[IsActive]
    ,[Country]
    ,[ExtraProperties]
    ,[ConcurrencyStamp]
    ,[CreationTime]
    ,[CreatorId])
VALUES
    (newid()
        ,N'Microsoft'
        ,'M2'
        ,'microsoft'
        ,null
        ,1
        ,1
        ,'US'
        ,null
        ,null
        ,getdate()
        ,null)
    71
    aspnet-core/database/master_data/product_categories.sql
    @@ -0,0 +1,71 @@
    USE [TeduEcommerce]
    GO

INSERT INTO [dbo].[AppProductCategories]
([Id]
    ,[Name]
    ,[Code]
    ,[Slug]
    ,[SortOrder]
    ,[CoverPicture]
    ,[Visibility]
    ,[IsActive]
    ,[ParentId]
    ,[SeoMetaDescription]
    ,[ExtraProperties]
    ,[ConcurrencyStamp]
    ,[CreationTime]
    ,[CreatorId])
VALUES
    (newid()
        ,N'Điện thoại'
        ,'C1'
        ,'dien-thoai'
        ,1
        ,null
        ,1
        ,1
        ,null
        ,N'Danh mục điện thoại'
        ,null
        ,null
        ,getdate()
        ,null)
    GO

INSERT INTO [dbo].[AppProductCategories]
([Id]
    ,[Name]
    ,[Code]
    ,[Slug]
    ,[SortOrder]
    ,[CoverPicture]
    ,[Visibility]
    ,[IsActive]
    ,[ParentId]
    ,[SeoMetaDescription]
    ,[ExtraProperties]
    ,[ConcurrencyStamp]
    ,[CreationTime]
    ,[CreatorId])
VALUES
    (newid()
        ,N'Laptop'
        ,'C2'
        ,'laptop'
        ,1
        ,null
        ,1
        ,1
        ,null
        ,N'Máy tính xách tay'
        ,null
        ,null
        ,getdate()
        ,null)
GO