﻿namespace ProjectComp1640.Model
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public DayOfWeek Day { get; set; } //0: Sunday -> 6: Saturday
        public string Slot { get; set; }
        public string LinkMeeting { get; set; }
        public int? ClassId { get; set; }
        public virtual Class? Class { get; set; }
        public int? ClassroomId { get; set; }
        public virtual Classroom? Classroom { get; set; }
    }
}
