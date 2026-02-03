using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Menu
{
    public record MenuDto
    {
        public int Id { get; init; }

        public required string Title { get; init; } 

        public string? Path { get; init; }

        public string? Icon { get; init; }

        public List<MenuDto> Children { get; init; } = [];
    }
}
