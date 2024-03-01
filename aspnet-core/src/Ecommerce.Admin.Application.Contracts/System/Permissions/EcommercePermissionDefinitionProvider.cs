using Ecommerce.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Ecommerce.Admin.Permissions;

public class EcommercePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        //Catalog
        var catalogGroup = context.AddGroup(EcommercePermissions.CatalogGroupName, L("Permission:Catalog"));

        //Manufacture
        var manufacturerPermission = catalogGroup.AddPermission(EcommercePermissions.Manufacturer.Default, L("Permission:Catalog.Manufacturer"));
        manufacturerPermission.AddChild(EcommercePermissions.Manufacturer.Create, L("Permission:Catalog.Manufacturer.Create"));
        manufacturerPermission.AddChild(EcommercePermissions.Manufacturer.Update, L("Permission:Catalog.Manufacturer.Update"));
        manufacturerPermission.AddChild(EcommercePermissions.Manufacturer.Delete, L("Permission:Catalog.Manufacturer.Delete"));

        //Product Category
        var productCategoryPermission = catalogGroup.AddPermission(EcommercePermissions.ProductCategory.Default, L("Permission:Catalog.ProductCategory"));
        productCategoryPermission.AddChild(EcommercePermissions.ProductCategory.Create, L("Permission:Catalog.ProductCategory.Create"));
        productCategoryPermission.AddChild(EcommercePermissions.ProductCategory.Update, L("Permission:Catalog.ProductCategory.Update"));
        productCategoryPermission.AddChild(EcommercePermissions.ProductCategory.Delete, L("Permission:Catalog.ProductCategory.Delete"));

        //Add product
        var productPermission = catalogGroup.AddPermission(EcommercePermissions.Product.Default, L("Permission:Catalog.Product"));
        productPermission.AddChild(EcommercePermissions.Product.Create, L("Permission:Catalog.Product.Create"));
        productPermission.AddChild(EcommercePermissions.Product.Update, L("Permission:Catalog.Product.Update"));
        productPermission.AddChild(EcommercePermissions.Product.Delete, L("Permission:Catalog.Product.Delete"));
        productPermission.AddChild(EcommercePermissions.Product.AttributeManage, L("Permission:Catalog.Product.AttributeManage"));

        //Add attribute
        var attributePermission = catalogGroup.AddPermission(EcommercePermissions.Attribute.Default, L("Permission:Catalog.Attribute"));
        attributePermission.AddChild(EcommercePermissions.Attribute.Create, L("Permission:Catalog.Attribute.Create"));
        attributePermission.AddChild(EcommercePermissions.Attribute.Update, L("Permission:Catalog.Attribute.Update"));
        attributePermission.AddChild(EcommercePermissions.Attribute.Delete, L("Permission:Catalog.Attribute.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<EcommerceResource>(name);
    }
}
