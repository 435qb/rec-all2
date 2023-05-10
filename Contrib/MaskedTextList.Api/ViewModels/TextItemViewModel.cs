namespace RecAll.Contrib.MaskedTextList.Api.ViewModels; 

public class TextItemViewModel {
    public int Id { get; set; }

    public int? ItemId { get; set; }

    public string Content { get; set; }
    
    public string MaskedContent { get; set; }
}