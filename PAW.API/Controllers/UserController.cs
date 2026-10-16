using Microsoft.AspNetCore.Mvc;
using PAW.DataAccess.Repositories;
using PAW.Models;
using PAW.Models.DTO;

namespace PAW.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(
    ILogger<UserController> logger,
    IUserRepository userRepository) : ControllerBase
{
    [HttpGet("paged", Name = "GetUsersPaged")]
    public async Task<ActionResult<PagedResult<UserDTO>>> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25)
    {
        var users = await userRepository.ReadAsync() ?? [];
        var totalCount = users.Count();

        var pagedItems = users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(UserDTO.ConvertFrom);

        return Ok(new PagedResult<UserDTO>
        {
            Items = pagedItems,
            TotalCount = totalCount,
            PageNumber = page,
            PageSize = pageSize
        });
    }

    [HttpGet(Name = "GetUsers")]
    public async Task<IEnumerable<UserDTO>> GetAll()
    {
        var users = await userRepository.ReadAsync() ?? [];
        return users.Select(UserDTO.ConvertFrom);
    }

    [HttpGet("{id:int}", Name = "GetUserById")]
    public async Task<ActionResult<UserDTO>> GetById(int id)
    {
        var user = await userRepository.FindAsync(id);

        if (user is null)
            return NotFound();

        return UserDTO.ConvertFrom(user);
    }

    [HttpPost]
    public async Task<bool> Save([FromBody] IEnumerable<UserDTO> userDtos)
    {

        foreach (var dto in userDtos)
        {
            var entity = UserDTO.Convert(dto);

            if (entity.UserId == 0)
            {
                await userRepository.CreateAsync(entity);
            }
            else
            {
                await userRepository.UpdateAsync(entity);
            }
        }
        return true;
    }

    [HttpDelete("{id:int}")]
    public async Task<bool> Delete(int id)
    {
        var user = await userRepository.FindAsync(id);

        if (user is null)
            return false;

      return await userRepository.DeleteAsync(user);

        
    }
}