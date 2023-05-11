﻿using EkoStatLibrary.Enums;
using System.ComponentModel.DataAnnotations;

namespace EkoStatRp.Models;

public class ReportSettingsViewModel
{
    [Display(Name = "Cost limit")]
    public double? CostLimit { get; set; }

    [Display(Name = "Type")]
    public LimitType CostLimitType { get; set; }
}
