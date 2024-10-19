﻿namespace Ecommerce.Admin.Manufacturers;

public class CreateUpdateManufacturerDto
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Slug { get; set; }
    public string CoverPicture { get; set; }
    public bool Visibility { get; set; }
    public bool IsActive { get; set; }
    public string Country { get; set; }
}