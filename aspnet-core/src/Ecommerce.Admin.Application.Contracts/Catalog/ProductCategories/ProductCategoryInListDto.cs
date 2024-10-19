﻿using System;
using Volo.Abp.Application.Dtos;

namespace Ecommerce.Admin.ProductCategories;

public class ProductCategoryInListDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public int SortOrder { get; set; }
    public string CoverPicture { get; set; }
    public bool Visibility { get; set; }
    public bool IsActive { get; set; }
}