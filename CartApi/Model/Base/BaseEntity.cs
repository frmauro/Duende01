﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CartApi.Model.Base;

public class BaseEntity
{
    [Key]
    [Column]
    public long Id { get; set; }
}
