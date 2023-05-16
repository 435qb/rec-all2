namespace RecAll.Contrib.MaskedTextList.Api.Models;

public class MaskedTextItem
{
    public static string MaskedString = "内容已隐藏";
    
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public string Content { get; set; }
    
    public string MaskedContent { get; set; }
    
    public bool IsHidden {get; set;}

    public string UserIdentityGuid { get; set; }

    public bool IsDeleted { get; set; }
}