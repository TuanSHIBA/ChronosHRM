using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.Common.Models
{
    namespace Chronos.Application.Common.Models
    {
        public class ServiceResponse<T>
        {
            public T? Data { get; set; }
            public bool Success { get; set; } = true;
            public string Message { get; set; } = string.Empty;

            public static ServiceResponse<T> SuccessResponse(T data, string message = "Thành công")
            {
                return new ServiceResponse<T> { Success = true, Data = data, Message = message };
            }

            public static ServiceResponse<T> ErrorResponse(string message)
            {
                return new ServiceResponse<T> { Success = false, Message = message };
            }
        }
    }
}
