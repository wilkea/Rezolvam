using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rezolvam.Domain.Reports.Enums
{
    public enum ProblemStatus
    {
        Open = 0,
        InProgress = 1,
        Resolved = 2,
        Rejected = -1
    }
}
