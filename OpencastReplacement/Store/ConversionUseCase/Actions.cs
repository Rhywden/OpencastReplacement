namespace OpencastReplacement.Store.ConversionUseCase
{
    public record SetConversionProgressAction(int Progress, Guid ConversionId); 
    public record ConversionCompleteAction(Guid ConversionId);
    public record CancelConversionAction(Guid ConversionId);
    public record ConversionFailedAction(Guid ConversionId);
}
