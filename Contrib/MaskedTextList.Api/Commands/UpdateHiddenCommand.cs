using System.ComponentModel.DataAnnotations;
namespace RecAll.Contrib.MaskedTextList.Api.Commands;

public class UpdateHiddenCommand
{
    [Required] public int Id { get; set; }
}