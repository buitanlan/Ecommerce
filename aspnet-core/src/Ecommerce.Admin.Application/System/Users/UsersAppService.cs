using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace Ecommerce.Admin.System.Users;

public class UsersAppService(IRepository<IdentityUser, Guid> repository, IdentityUserManager identityUserManager)
    : CrudAppService<
        IdentityUser,
        UserDto,
        Guid,
        PagedResultRequestDto,
        CreateUserDto,
        UpdateUserDto>(repository), IUsersAppService
{
    public async Task DeleteMultipleAsync(IEnumerable<Guid> ids)
    {
        await Repository.DeleteManyAsync(ids);
        await UnitOfWorkManager.Current.SaveChangesAsync();
    }

    public async Task<List<UserInListDto>> GetListAllAsync(string filterKeyword)
    {
        var query = await Repository.GetQueryableAsync();
        if (!string.IsNullOrEmpty(filterKeyword))
        {
            query = query.Where(o => o.Name.ToLower().Contains(filterKeyword)
                                     || o.Email.ToLower().Contains(filterKeyword)
                                     || o.PhoneNumber.ToLower().Contains(filterKeyword));
        }

        var data = await AsyncExecuter.ToListAsync(query);
        return ObjectMapper.Map<List<IdentityUser>, List<UserInListDto>>(data);
    }

    public async Task<PagedResultDto<UserInListDto>> GetListWithFilterAsync(BaseListFilterDto input)
    {
        var query = await Repository.GetQueryableAsync();

        if (!input.Keyword.IsNullOrWhiteSpace())
        {
            input.Keyword = input.Keyword.ToLower();
            query = query.Where(o => o.Name.ToLower().Contains(input.Keyword)
                                     || o.Email.ToLower().Contains(input.Keyword)
                                     || o.PhoneNumber.ToLower().Contains(input.Keyword));
        }

        query = query.OrderByDescending(x => x.CreationTime);

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.Skip(input.SkipCount).Take(input.MaxResultCount);
        var data = await AsyncExecuter.ToListAsync(query);
        var users = ObjectMapper.Map<List<IdentityUser>, List<UserInListDto>>(data);
        return new PagedResultDto<UserInListDto>(totalCount, users);
    }

    public override async Task<UserDto> CreateAsync(CreateUserDto input)
    {
        var query = await Repository.GetQueryableAsync();
        var isUserNameExisted = query.Any(x => x.UserName == input.UserName);
        if (isUserNameExisted)
        {
            throw new UserFriendlyException("Tài khoản đã tồn tại");
        }

        var isUserEmailExisted = query.Any(x => x.Email == input.Email);
        if (isUserEmailExisted)
        {
            throw new UserFriendlyException("Email đã tồn tại");
        }

        var userId = Guid.NewGuid();
        var user = new IdentityUser(userId, input.UserName, input.Email);
        user.Name = input.Name;
        user.Surname = input.Surname;
        user.SetPhoneNumber(input.PhoneNumber, true);
        var result = await identityUserManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
        {
            var errorList = result.Errors.ToList();
            var errors = errorList.Aggregate("", (current, error) => current + error.Description.ToString());

            throw new UserFriendlyException(errors);
        }

        return ObjectMapper.Map<IdentityUser, UserDto>(user);
    }

    public override async Task<UserDto> UpdateAsync(Guid id, UpdateUserDto input)
    {
        var user = await identityUserManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            throw new EntityNotFoundException(typeof(IdentityUser), id);
        }

        user.Name = input.Name;
        user.SetPhoneNumber(input.PhoneNumber, true);
        user.Surname = input.Surname;
        var result = await identityUserManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errorList = result.Errors.ToList();
            var errors = errorList.Aggregate("", (current, error) => current + error.Description.ToString());

            throw new UserFriendlyException(errors);
        }

        return ObjectMapper.Map<IdentityUser, UserDto>(user);
    }

    public override async Task<UserDto> GetAsync(Guid id)
    {
        var user = await identityUserManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            throw new EntityNotFoundException(typeof(IdentityUser), id);
        }

        var userDto = ObjectMapper.Map<IdentityUser, UserDto>(user);
        var roles = await identityUserManager.GetRolesAsync(user);
        userDto.Roles = roles;
        return userDto;
    }

    public async Task AssignRolesAsync(Guid userId, string[] roleNames)
    {
        var user = await identityUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new EntityNotFoundException(typeof(IdentityUser), userId);
        }

        var currentRoles = await identityUserManager.GetRolesAsync(user);
        var removedResult = await identityUserManager.RemoveFromRolesAsync(user, currentRoles);
        var addedResult = await identityUserManager.AddToRolesAsync(user, roleNames);
        if (!addedResult.Succeeded || !removedResult.Succeeded)
        {
            var addedErrorList = addedResult.Errors.ToList();
            var removedErrorList = removedResult.Errors.ToList();
            var errorList = new List<Microsoft.AspNetCore.Identity.IdentityError>();
            errorList.AddRange(addedErrorList);
            errorList.AddRange(removedErrorList);
            var errors = errorList.Aggregate("", (current, error) => current + error.Description.ToString());

            throw new UserFriendlyException(errors);
        }
    }

    public async Task SetPasswordAsync(Guid userId, SetPasswordDto input)
    {
        var user = await identityUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new EntityNotFoundException(typeof(IdentityUser), userId);
        }
        var token = await identityUserManager.GeneratePasswordResetTokenAsync(user);
        var result = await identityUserManager.ResetPasswordAsync(user, token, input.NewPassword);
        if (!result.Succeeded)
        {
            var errorList = result.Errors.ToList();
            var errors = errorList.Aggregate("", (current, error) => current + error.Description.ToString());
            throw new UserFriendlyException(errors);
        }
    }
}
