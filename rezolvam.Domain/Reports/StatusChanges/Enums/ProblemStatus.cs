using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Domain.Reports.StatusChanges.Enums
{
    public enum ReportStatus
    {   
        Unverified = -2,
        Open = 0,
        InProgress = 1,
        Resolved = 2,
        Rejected = -1
    }
}
