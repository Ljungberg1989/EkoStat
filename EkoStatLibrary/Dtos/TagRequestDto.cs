﻿#pragma warning disable CS8618

using System.ComponentModel.DataAnnotations;

namespace EkoStatLibrary.Dtos;

public class TagRequestDto
{
    [Required]
    [MinLength(2)]
    public string Name { get; set; }
    
    [Required]
    public int UserId { get; set; }
}
