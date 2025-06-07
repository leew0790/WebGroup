﻿namespace ProjectComp1640.Dtos.Class
{
    public class CreateClassDto
    {
        public string TutorName { get; set; }
        public string SubjectName { get; set; }
        public string ClassName { get; set; }
        public int TotalSlot { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public List<string> StudentNames { get; set; } = new List<string>();
    }
}
