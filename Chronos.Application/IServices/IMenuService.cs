using Chronos.Application.DTOs.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface IMenuService
    {
        Task<List<MenuDto>> GetMenusForUserAsync(string userId);
    }
}
