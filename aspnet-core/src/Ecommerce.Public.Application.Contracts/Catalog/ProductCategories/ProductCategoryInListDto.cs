﻿using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace Ecommerce.Public.ProductCategories;

public class ProductCategoryInListDto : EntityDto<Guid>
{
    public string Name { get; set; }
    public string Code { get; set; }
    public int SortOrder { get; set; }
    public string CoverPicture { get; set; }
    public bool Visibility { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentId { get; set; }
    public List<ProductCategoryInListDto> Children { get; set; } = [];
}
