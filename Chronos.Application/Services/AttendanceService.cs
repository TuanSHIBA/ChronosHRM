using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Attendance;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Enums;
using Chronos.Domain.Interfaces;

namespace Chronos.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResponse<AttendanceDto>> GetTodayAttendance(Guid employeeId)
        {
            var today = DateTime.Now.Date;

            var entity = (await _unitOfWork.Attendance.GetAllAsync(includeProperties: "Employee")).FirstOrDefault(a => a.EmployeeId == employeeId && a.Date == today);
            if (entity == null)
            {
                ServiceResponse<AttendanceDto> response = ServiceResponse<AttendanceDto>.ErrorResponse("Không có dữ liệu");
            }
            var attendanceDto = new AttendanceDto
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                // 👇 Map dữ liệu từ bảng Employee sang
                EmployeeName = entity.Employee != null ? entity.Employee.FullName : "N/A",
                EmployeeCode = entity.Employee != null ? entity.Employee.EmployeeCode : "N/A",

                Date = entity.Date,
                CheckInTime = entity.CheckInTime?.ToString(@"hh\:mm"),
                CheckOutTime = entity.CheckOutTime?.ToString(@"hh\:mm"),
                WorkingHours = entity.WorkingHours
            };
            return ServiceResponse<AttendanceDto>.SuccessResponse(attendanceDto);
        }

        public async Task<Attendance?> GetByDateAsync(Guid employeeId, DateTime date)
        {
            return await _unitOfWork.Attendance.GetByDateAsync(employeeId, date.Date);
        }


  
        public async Task<ServiceResponse<AttendanceDto>> CheckIn(Guid employeeId)
        {
           
                var today = DateTime.Now.Date;

                var existingRecord = await _unitOfWork.Attendance.GetByDateAsync(employeeId, today);

                if (existingRecord != null)
                {
                    return ServiceResponse<AttendanceDto>.ErrorResponse("Bạn đã thực hiện Check-in ngày hôm nay rồi!");
                }

                var newRecord = new Attendance
                {
                    EmployeeId = employeeId,
                    Date = today,
                    CheckInTime = DateTime.Now.TimeOfDay, 
                    CheckOutTime = null,
                    WorkingHours = 0,

            
                    Status = AttendanceStatus.Pending,
                    Source = AttendanceSource.Web,
                    Note = "Check-in từ xa qua Web"
                };

                await _unitOfWork.Attendance.AddAsync(newRecord);
                await _unitOfWork.SaveChangesAsync();
                var resultDto = new AttendanceDto
                {
                    Id = newRecord.Id,
                    Date = newRecord.Date,
                    CheckInTime = newRecord.CheckInTime?.ToString(@"hh\:mm"),
                    Status = newRecord.Status.ToString(),
                    EmployeeId = newRecord.EmployeeId
                };

                return ServiceResponse<AttendanceDto>.SuccessResponse(resultDto, "Check-in thành công! Vui lòng chờ quản lý duyệt.");
            
           
        }

        public async Task<ServiceResponse<AttendanceDto>> CheckOut(Guid employeeId)
        {

            var today = DateTime.Now.Date;

            var record = await _unitOfWork.Attendance.GetByDateAsync(employeeId, today);

            if (record == null)
            {
                return ServiceResponse<AttendanceDto>.ErrorResponse("Bạn chưa Check-in nên không thể Check-out!");
            }
            if (record.CheckOutTime.HasValue)
            {
                return ServiceResponse<AttendanceDto>
                    .ErrorResponse("Bạn đã Check-out rồi!");
            }
            //  Cập nhật giờ ra
            record.CheckOutTime = DateTime.Now.TimeOfDay;
            var duration = record.CheckOutTime.Value - record.CheckInTime.Value;
            record.WorkingHours = Math.Round(duration.TotalHours, 2); 
            
            _unitOfWork.Attendance.Update(record);
            await _unitOfWork.SaveChangesAsync();

            var result = new AttendanceDto
            {
                Id = record.Id,
                Date = record.Date,
                CheckInTime = record.CheckInTime?.ToString(@"hh\:mm"),
                CheckOutTime = record.CheckOutTime?.ToString(@"hh\:mm"),
                WorkingHours = record.WorkingHours,
                Status = record.Status.ToString()
            };

            return ServiceResponse<AttendanceDto>.SuccessResponse(result, "Check-out thành công! Hẹn gặp lại.");

        }
        public async Task<ServiceResponse<List<AttendanceRequestDto>>> GetPendingRequests()
        {
            var list = await _unitOfWork.Attendance.GetPendingListAsync();

            // Map sang DTO thủ công (hoặc dùng AutoMapper nếu bạn đã cấu hình)
            var result = list.Select(x => new AttendanceRequestDto
            {
                Id = x.Id,
                EmployeeName = x.Employee?.FullName ?? "N/A",
                EmployeeCode = x.Employee?.EmployeeCode ?? "N/A",
                Date = x.Date,
                CheckInTime = x.CheckInTime?.ToString(@"hh\:mm") ?? "",
                Note = x.Note,
                Status = x.Status.ToString()
            }).ToList();
            return ServiceResponse<List<AttendanceRequestDto>>.SuccessResponse(result, "Get Pendung Request Successfully");
        }

        public async Task<string> ApproveRequest(Guid managerUserId,Guid attendanceId, ApproveAttendanceDto request)
        {
            // A. Tìm bản ghi chấm công
            var attendance = await _unitOfWork.Attendance.GetByIdAsync(attendanceId);
            if (attendance == null) return "Không tìm thấy yêu cầu chấm công.";

            // B. (Tùy chọn) Kiểm tra xem đã duyệt chưa để tránh duyệt lại
            if (attendance.Status != AttendanceStatus.Pending) return "Yêu cầu này đã được xử lý trước đó.";

            // C. Cập nhật trạng thái
            attendance.Status = request.IsApproved ? AttendanceStatus.Approved : AttendanceStatus.Rejected;
            attendance.ManagerNote = request.ManagerNote;
            attendance.ApprovedAt = DateTime.Now;

            // Lưu vết người duyệt (Sếp)
            attendance.ApproverId = managerUserId; // Bạn có thể map sang EmployeeId của sếp nếu cần

            // D. Lưu vào DB
            _unitOfWork.Attendance.Update(attendance);
            await _unitOfWork.SaveChangesAsync();

            return request.IsApproved ? "Đã DUYỆT thành công!" : "Đã TỪ CHỐI yêu cầu.";
        }
        public async Task<ServiceResponse<List<AttendanceDto>>> GetMyHistory(Guid employeeId, int month, int year)
        {
            var list = await _unitOfWork.Attendance.GetMyAttendanceHistoryAsync(employeeId, month, year);

            var dtoList = list.Select(x => new AttendanceDto
            {
                Id = x.Id,
                EmployeeName = x.Employee?.FullName ?? "",
                EmployeeCode = x.Employee?.EmployeeCode ?? "", // Có thể null nếu không include
                Date = x.Date,
                CheckInTime = x.CheckInTime?.ToString(@"hh\:mm"),
                CheckOutTime = x.CheckOutTime?.ToString(@"hh\:mm"),
                WorkingHours = x.WorkingHours,
                Status = x.Status.ToString()
            }).ToList();

            return ServiceResponse<List<AttendanceDto>>.SuccessResponse(dtoList);
        }
    }
}