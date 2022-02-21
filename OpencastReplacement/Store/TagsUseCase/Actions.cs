using OpencastReplacement.Models;

namespace OpencastReplacement.Store.TagsUseCase
{
    public record AddTagAction(Tag tag);
    public record DeleteTagAction(Tag tag);
}
