using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Attendance;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Enums;
using Chronos.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Chronos.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Chỉ Inject IUnitOfWork, không Inject DbContext nữa
        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. Lấy thông tin chấm công hôm nay
        public async Task<AttendanceDto?> GetTodayAttendance(Guid employeeId)
        {
            var today = DateTime.Now.Date;

            var entity = (await _unitOfWork.Attendance.GetAllAsync(includeProperties: "Employee")).FirstOrDefault(a => a.EmployeeId == employeeId && a.Date == today);
            if (entity == null)
            {
                return null;
            }
            var attendanceDto = new AttendanceDto
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                // 👇 Map dữ liệu từ bảng Employee sang
                EmployeeName = entity.Employee != null ? entity.Employee.FullName : "N/A",
                EmployeeCode = entity.Employee != null ? entity.Employee.EmployeeCode : "N/A",

                Date = entity.Date,
                // Format giờ cho đẹp
                CheckInTime = entity.CheckInTime?.ToString(@"hh\:mm"),
                CheckOutTime = entity.CheckOutTime?.ToString(@"hh\:mm"),
                WorkingHours = entity.WorkingHours
            };
            return attendanceDto;
        }

        public async Task<Attendance?> GetByDateAsync(Guid employeeId, DateTime date)
        {
            return await _unitOfWork.Attendance.GetByDateAsync( employeeId,date.Date);
        }


        // 2. Xử lý Check In
        public async Task<ServiceResponse<AttendanceDto>> CheckIn(Guid employeeId)
        {
            try
            {
                var today = DateTime.Now.Date;

                // 1. Kiểm tra xem đã Check-in chưa (Dùng hàm tối ưu ở Repo)
                var existingRecord = await _unitOfWork.Attendance.GetByDateAsync(employeeId, today);

                if (existingRecord != null)
                {
                    return ServiceResponse<AttendanceDto>.ErrorResponse("Bạn đã thực hiện Check-in ngày hôm nay rồi!");
                }

                // 2. Tạo bản ghi mới
                var newRecord = new Attendance
                {
                    EmployeeId = employeeId,
                    Date = today,
                    CheckInTime = DateTime.Now.TimeOfDay, // Lấy giờ hiện tại
                    CheckOutTime = null,
                    WorkingHours = 0,

                    // 👇 QUAN TRỌNG: Đánh dấu là Chờ duyệt & Nguồn từ Web
                    Status = AttendanceStatus.Pending,
                    Source = AttendanceSource.Web,
                    Note = "Check-in từ xa qua Web"
                };

                await _unitOfWork.Attendance.AddAsync(newRecord);
                await _unitOfWork.SaveChangesAsync();

                // 3. Map sang DTO để trả về cho Frontend hiển thị ngay lập tức
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
            catch (Exception ex)
            {
                return ServiceResponse<AttendanceDto>.ErrorResponse("Lỗi hệ thống: " + ex.Message);
            }
        }

        // 🔴 HÀM CHECK-OUT (Tan ca)
        public async Task<ServiceResponse<AttendanceDto>> CheckOut(Guid employeeId)
        {
            try
            {
                var today = DateTime.Now.Date;

                // 1. Tìm bản ghi hôm nay
                var record = await _unitOfWork.Attendance.GetByDateAsync(employeeId, today);

                if (record == null)
                {
                    return ServiceResponse<AttendanceDto>.ErrorResponse("Bạn chưa Check-in nên không thể Check-out!");
                }

                // 2. Cập nhật giờ ra
                record.CheckOutTime = DateTime.Now.TimeOfDay;

                // 3. Tính tổng giờ làm (Công thức đơn giản: Ra - Vào)
                if (record.CheckInTime.HasValue)
                {
                    var duration = record.CheckOutTime.Value - record.CheckInTime.Value;
                    record.WorkingHours = Math.Round(duration.TotalHours, 2); // Làm tròn 2 số lẻ
                }

                // Lưu ý: Check-out xong vẫn để Status là Pending (hoặc giữ nguyên status cũ)
                // Vì sếp duyệt là duyệt cả ngày công.

                _unitOfWork.Attendance.Update(record);
                await _unitOfWork.SaveChangesAsync();

                // 4. Trả về kết quả
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
            catch (Exception ex)
            {
                return ServiceResponse<AttendanceDto>.ErrorResponse("Lỗi hệ thống: " + ex.Message);
            }
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
           return  ServiceResponse<List<AttendanceRequestDto>>.SuccessResponse(result, "Get Pendung Request Successfully");
        }

        // 2. Hành động Duyệt/Từ chối
        public async Task<string> ApproveRequest(Guid managerUserId, ApproveAttendanceDto request)
        {
            // A. Tìm bản ghi chấm công
            var attendance = await _unitOfWork.Attendance.GetByIdAsync(request.AttendanceId);
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
        // AttendanceService.cs
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