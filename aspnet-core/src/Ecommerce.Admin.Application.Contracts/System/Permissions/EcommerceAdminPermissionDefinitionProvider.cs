using Ecommerce.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Ecommerce.Admin.Permissions;

public class EcommerceAdminPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        //Catalog
        var catalogGroup = context.AddGroup(EcommerceAdminPermissions.CatalogGroupName, L("Permission:Catalog"));

        //Manufacture
        var manufacturerPermission = catalogGroup.AddPermission(EcommerceAdminPermissions.Manufacturer.Default, L("Permission:Catalog.Manufacturer"));
        manufacturerPermission.AddChild(EcommerceAdminPermissions.Manufacturer.Create, L("Permission:Catalog.Manufacturer.Create"));
        manufacturerPermission.AddChild(EcommerceAdminPermissions.Manufacturer.Update, L("Permission:Catalog.Manufacturer.Update"));
        manufacturerPermission.AddChild(EcommerceAdminPermissions.Manufacturer.Delete, L("Permission:Catalog.Manufacturer.Delete"));

        //Product Category
        var productCategoryPermission = catalogGroup.AddPermission(EcommerceAdminPermissions.ProductCategory.Default, L("Permission:Catalog.ProductCategory"));
        productCategoryPermission.AddChild(EcommerceAdminPermissions.ProductCategory.Create, L("Permission:Catalog.ProductCategory.Create"));
        productCategoryPermission.AddChild(EcommerceAdminPermissions.ProductCategory.Update, L("Permission:Catalog.ProductCategory.Update"));
        productCategoryPermission.AddChild(EcommerceAdminPermissions.ProductCategory.Delete, L("Permission:Catalog.ProductCategory.Delete"));

        //Add product
        var productPermission = catalogGroup.AddPermission(EcommerceAdminPermissions.Product.Default, L("Permission:Catalog.Product"));
        productPermission.AddChild(EcommerceAdminPermissions.Product.Create, L("Permission:Catalog.Product.Create"));
        productPermission.AddChild(EcommerceAdminPermissions.Product.Update, L("Permission:Catalog.Product.Update"));
        productPermission.AddChild(EcommerceAdminPermissions.Product.Delete, L("Permission:Catalog.Product.Delete"));
        productPermission.AddChild(EcommerceAdminPermissions.Product.AttributeManage, L("Permission:Catalog.Product.AttributeManage"));

        //Add attribute
        var attributePermission = catalogGroup.AddPermission(EcommerceAdminPermissions.Attribute.Default, L("Permission:Catalog.Attribute"));
        attributePermission.AddChild(EcommerceAdminPermissions.Attribute.Create, L("Permission:Catalog.Attribute.Create"));
        attributePermission.AddChild(EcommerceAdminPermissions.Attribute.Update, L("Permission:Catalog.Attribute.Update"));
        attributePermission.AddChild(EcommerceAdminPermissions.Attribute.Delete, L("Permission:Catalog.Attribute.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<EcommerceResource>(name);
    }
}
