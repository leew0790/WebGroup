﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectComp1640.Data;
using ProjectComp1640.Dtos.Schedule;
using ProjectComp1640.Model;

namespace ProjectComp1640.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly ApplicationDBContext _dbContext;
        public ScheduleController(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost("create-schedule")]
        public async Task<IActionResult> CreateSchedule(ScheduleDto scheduleDto)
        {
            var cls = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == scheduleDto.ClassId);
            if (cls == null)
            {
                return NotFound("Cannot find this class");
            }
            var clsrm = await _dbContext.Classrooms.AnyAsync(c => c.Id == scheduleDto.ClassroomId);
            if (!clsrm)
            {
                return NotFound("Cannot find this classroom.");
            }
            if (scheduleDto.ScheduleDate < cls.StartDate ||  scheduleDto.ScheduleDate > cls.EndDate)
            {
                return BadRequest($"Schedule must be created between the class time: From {cls.StartDate} to {cls.EndDate}");
            }
            var dupSchedule = await _dbContext.Schedules.FirstOrDefaultAsync(s =>
                s.ScheduleDate == scheduleDto.ScheduleDate &&
                s.Day == scheduleDto.Day &&
                s.Slot == scheduleDto.Slot &&
                s.ClassroomId == scheduleDto.ClassroomId
                );
            if (dupSchedule != null)
            {
                return BadRequest($"There is a duplicate schedule of Class {dupSchedule.ClassId} in classroom {dupSchedule.ClassroomId} at ({dupSchedule.ScheduleDate:yyyy-MM-dd}) at slot {dupSchedule.Slot}.");
            }
            var schedule = new Schedule
            {
                ScheduleDate = scheduleDto.ScheduleDate,
                Day = scheduleDto.Day,
                Slot = scheduleDto.Slot,
                LinkMeeting = scheduleDto.LinkMeeting,
                ClassId = scheduleDto.ClassId,
                ClassroomId = scheduleDto.ClassroomId
            };
            _dbContext.Schedules.Add(schedule);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSchedule), new { id = schedule.Id }, new { message = "Create schedule succesfully", scheduleDto });
        }
        [HttpGet("get-all-schedules")]
        public async Task<ActionResult<IEnumerable<GetScheduleDto>>> GetSchedules()
        {
            var schedules = await _dbContext.Schedules
                .Include(s => s.Class).ThenInclude(c => c.Tutor)
                .Include(s => s.Classroom)
                .ToListAsync();
            var scheduleList = schedules.Select(s => new GetScheduleDto
            {
                Id = s.Id,
                ScheduleDate = s.ScheduleDate,
                Day = s.Day,
                Slot = s.Slot,
                LinkMeeting = s.LinkMeeting,
                TutorId = s.Class.TutorId,
                ClassId = s.ClassId,
                ClassroomId = s.ClassroomId,
            });
            return Ok(scheduleList);
        }
        [HttpGet("get-schedule/{id}")]
        public async Task<ActionResult<GetScheduleDto>> GetSchedule(int id)
        {
            var s = await _dbContext.Schedules
                .Include(s => s.Class).ThenInclude(c => c.Tutor)
                .Include(s => s.Classroom)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (s == null)
            {
                return NotFound();
            }
            var dto = new GetScheduleDto
            {
                Id = s.Id,
                ScheduleDate = s.ScheduleDate,
                Day = s.Day,
                Slot = s.Slot,
                LinkMeeting = s.LinkMeeting,
                TutorId = s.Class.TutorId,
                ClassId = s.ClassId,
                ClassroomId = s.ClassroomId,
            };
            return Ok(dto);
        }
        [HttpPut("update-schedule/{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, ScheduleDto scheduleDto)
        {
            var schedule = await _dbContext.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound("Cannot find this schedule");
            }
            var cls = await _dbContext.Classes.FindAsync(scheduleDto.ClassId);
            if (cls == null) {
                return NotFound("Cannot find this class.");
            }
            var clsrm = await _dbContext.Classrooms.FindAsync(scheduleDto.ClassroomId);
            if (clsrm == null)
            {
                return NotFound("Cannot find this classroom.");
            }
            var dupSchedule = await _dbContext.Schedules.FirstOrDefaultAsync(s =>
                s.ScheduleDate == scheduleDto.ScheduleDate &&
                s.Day == scheduleDto.Day &&
                s.Slot == scheduleDto.Slot &&
                s.ClassroomId == scheduleDto.ClassroomId &&
                s.Id != id
                );
            if (dupSchedule != null)
            {
                return BadRequest($"There is a duplicate schedule of Class {dupSchedule.ClassId} in classroom {dupSchedule.ClassroomId} at ({dupSchedule.ScheduleDate:yyyy-MM-dd}) at slot {dupSchedule.Slot}.");
            }
            schedule.ScheduleDate = scheduleDto.ScheduleDate;
            schedule.Day = scheduleDto.Day;
            schedule.Slot = scheduleDto.Slot;
            schedule.LinkMeeting = scheduleDto.LinkMeeting;
            schedule.ClassId = scheduleDto.ClassId;
            schedule.ClassroomId = scheduleDto.ClassroomId;
            await _dbContext.SaveChangesAsync();
            return Ok(schedule);
        }
        [HttpDelete("delete-schedule/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _dbContext.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound(new { message = "Cannot find this schedule." });
            }
            _dbContext.Schedules.Remove(schedule);
            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Delete schedule succesfully." });
        }
        [HttpPost("create-recurring-schedules")]

        public async Task<IActionResult> CreateRecurringSchedule(ScheduleDto scheduleDto)
        {
            var cls = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == scheduleDto.ClassId);
            if (cls == null)
            { 
                return NotFound("Cannot find this class");
            }
            var clsrmExists = await _dbContext.Classrooms.AnyAsync(c => c.Id == scheduleDto.ClassroomId);
            if (!clsrmExists)
            {
                return NotFound("Cannot find this classroom");
            }
            if (scheduleDto.ScheduleDate < cls.StartDate || scheduleDto.ScheduleDate > cls.EndDate)
            {
                return BadRequest("Recurring start date must be inside the class study time");
            }
            var scheduleDates = new List<DateTime>();
            var current = scheduleDto.ScheduleDate.Date;

            while (current <= cls.EndDate.Date)
            {
                scheduleDates.Add(current);
                current = current.AddDays(7);
            }
            var dupSchedule = await _dbContext.Schedules
                .Where(s => scheduleDates.Contains(s.ScheduleDate) &&
                            s.Day == scheduleDto.Day &&
                            s.Slot == scheduleDto.Slot &&
                            s.ClassId == scheduleDto.ClassId &&
                            s.ClassroomId == scheduleDto.ClassroomId)
                .Select(s => s.ScheduleDate)
                .ToListAsync();
            if (dupSchedule.Any())
            {
                return BadRequest($"There is a duplicate schedule of Class {scheduleDto.ClassId} in classroom {scheduleDto.ClassroomId} at ({scheduleDto.ScheduleDate:yyyy-MM-dd}) at slot {scheduleDto.Slot}.");
            }
            var schedules = scheduleDates.Select(date => new Schedule
            {
                ScheduleDate = date,
                Day = scheduleDto.Day,
                Slot = scheduleDto.Slot,
                LinkMeeting = scheduleDto.LinkMeeting,
                ClassId = scheduleDto.ClassId,
                ClassroomId = scheduleDto.ClassroomId
            }).ToList();
            _dbContext.Schedules.AddRange(schedules);
            await _dbContext.SaveChangesAsync();
            return Ok(new
            {
                Message = "Create new schedule successfully.",
                Created = schedules.Select(s => new
                {
                    s.Id,
                    s.ScheduleDate,
                    s.Day,
                    s.Slot,
                    s.LinkMeeting,
                    s.ClassId,
                    s.ClassroomId
                })
            });
        }


    }
}
