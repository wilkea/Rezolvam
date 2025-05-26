using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rezolvam.Domain.Reports.Enums;

namespace rezolvam.Domain.Reports
{
    public class Report
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public ProblemStatus Status { get; private set; }
        public string PhotoUrl { get; private set; }

        private Report() { }

        private Report
            (
                Guid id,
                string title,
                string description,
                ProblemStatus status,
                string photoUrl
            )
        {
            Id = Id;
            Title = title;
            Description = description;
            Status = status;
            PhotoUrl = photoUrl;
        }
        public static Report CreateReport
            (
                string title,
                string description,
                string photoUrl
            )
        {
            return new Report
            (
                Guid.NewGuid(),
                title,
                description,
                0,
                photoUrl
            );
        }
        public void UpdateReport
            (
                string title,
                string description,
                string photoUrl
            )
        {
            Title = title;
            Description = description;
            PhotoUrl = photoUrl;
        }
        public void UpdateStatus(ProblemStatus status)
        {
            Status = status;
        }
    }

}
