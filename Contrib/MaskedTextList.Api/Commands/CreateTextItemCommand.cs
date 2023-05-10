﻿using System.ComponentModel.DataAnnotations;

namespace RecAll.Contrib.MaskedTextList.Api.Commands;

public class CreateTextItemCommand {
    [Required]
    public string Content { get; set; }
    [Required]
    public string MaskedContent { get; set; }
}